﻿using Ionic.Zip;
using log4net;
using MissionPlanner.ArduPilot.Mavlink;
using MissionPlanner.Utilities;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MissionPlanner.Controls
{
    public partial class MavFTPUI : UserControl
    {
        private static readonly ILog log =
            LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private MAVLinkInterface _mav;
        private MAVFtp _mavftp;

        public MavFTPUI() : this(MainV2.comPort)
        {
        }

        public MavFTPUI(MAVLinkInterface mav)
        {
            _mav = mav;
            _mavftp = new MAVFtp(_mav, (byte)_mav.sysidcurrent, (byte)mav.compidcurrent);
            DateTime nextupdate = DateTime.UtcNow;
            _mavftp.Progress += (message, percent) =>
            {
                try
                {
                    if (this.IsDisposed)
                    {
                        _mavftp = null;
                        return;
                    }

                    if (nextupdate < DateTime.UtcNow)
                        this.BeginInvokeIfRequired(() =>
                        {
                            try
                            {
                                nextupdate = DateTime.UtcNow.AddMilliseconds(100);
                                if (percent >= 0)
                                    toolStripProgressBar1.Value = percent;
                                toolStripStatusLabel1.Text = message;
                                statusStrip1.Refresh();
                            }
                            catch (Exception ex)
                            {
                                log.Error(ex);
                            }
                        });
                }
                catch (Exception ex)
                {
                    log.Error(ex);
                }
            };
            InitializeComponent();

            ThemeManager.ApplyThemeTo(this);

            treeView1.PathSeparator = "/";
        }

        private async void PopulateTreeView()
        {
            toolStripStatusLabel1.Text = "Updating Folders";
            toolStripProgressBar1.ProgressBar.Style = ProgressBarStyle.Marquee;

            treeView1.BeginUpdate();

            treeView1.Enabled = false;

            treeView1.Nodes.Clear();

            TreeNode rootNode = null;

            DirectoryInfo info = new DirectoryInfo(@"/", _mavftp);
            if (info.Exists)
            {
                rootNode = new TreeNode(info.Name, 0, 0);
                rootNode.Tag = info;
                await PopulateDirectories(await info.GetDirectories().ConfigureAwait(true), rootNode).ConfigureAwait(true);
                treeView1.Nodes.Add(rootNode);
            }
            /*
            info = new DirectoryInfo(@"@ROMFS/", _mavftp);
            if (info.Exists)
            {
                rootNode = new TreeNode("@ROMFS", 0, 0);
                rootNode.Tag = info;
                await PopulateDirectories(await info.GetDirectories().ConfigureAwait(true), rootNode).ConfigureAwait(true);
                treeView1.Nodes.Add(rootNode);
            }
            */
            info = new DirectoryInfo(@"@SYS/", _mavftp);
            if (info.Exists)
            {
                rootNode = new TreeNode("@SYS", 0, 0);
                rootNode.Tag = info;
                await PopulateDirectories(await info.GetDirectories().ConfigureAwait(true), rootNode).ConfigureAwait(true);
                treeView1.Nodes.Add(rootNode);
            }
            

            toolStripStatusLabel1.Text = "Ready";

            treeView1.Enabled = true;
            
            treeView1.EndUpdate();

            toolStripProgressBar1.ProgressBar.Style = ProgressBarStyle.Blocks;

            treeView1.SelectedNode = rootNode;

            TreeView1_NodeMouseClick(this, new TreeNodeMouseClickEventArgs(rootNode, MouseButtons.Left, 1, 1, 1));
        }

        private async Task PopulateDirectories(DirectoryInfo[] subDirs,
            TreeNode nodeToAddTo)
        {
            List<TreeNode> info = new List<TreeNode>();
            TreeNode aNode;
            foreach (DirectoryInfo subDir in subDirs)
            {
                aNode = new TreeNode(subDir.Name, 0, 0);
                aNode.Tag = subDir;
                aNode.ImageKey = "folder";
                nodeToAddTo.Nodes.Add(aNode);
                info.Add(aNode);
            }
        }

        private async void TreeView1_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Node == null)
                return;

            TreeNode newSelected = e.Node;
            listView1.Items.Clear();
            DirectoryInfo nodeDirInfo = (DirectoryInfo)newSelected.Tag;
            ListViewItem.ListViewSubItem[] subItems;
            ListViewItem item = null;

            var dirs = await nodeDirInfo.GetDirectories().ConfigureAwait(true);

            newSelected.Nodes.Clear();

            await PopulateDirectories(dirs, newSelected).ConfigureAwait(true);

            foreach (DirectoryInfo dir in dirs)
            {
                item = new ListViewItem(dir.Name, 0);
                subItems = new ListViewItem.ListViewSubItem[]
                {
                    new ListViewItem.ListViewSubItem(item, "Directory"),
                    new ListViewItem.ListViewSubItem(item, "".ToString())
                };
                item.Tag = nodeDirInfo;
                item.SubItems.AddRange(subItems);
                listView1.Items.Add(item);
            }

            foreach (var file in await nodeDirInfo.GetFiles())
            {
                item = new ListViewItem(file.Name, 1);
                subItems = new ListViewItem.ListViewSubItem[]
                {
                    new ListViewItem.ListViewSubItem(item, "File"),
                    new ListViewItem.ListViewSubItem(item, file.Size.ToSizeUnits())
                };
                item.Tag = nodeDirInfo;
                item.SubItems.AddRange(subItems);
                listView1.Items.Add(item);
            }

            try
            {
                listView1.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
            }
            catch
            {
            }
        }

        [Serializable]
        public class DirectoryInfo : FileSystemInfo
        {
            private readonly MAVFtp _mavftp;
            private List<MAVFtp.FtpFileInfo> cache;

            public DirectoryInfo(string dir, MAVFtp mavftp)
            {
                _mavftp = mavftp;
                FullPath = dir;
            }

            public override string Name
            {
                get { return Path.GetFileName(FullPath); }
            }

            public override bool Exists {
                get { return true; }
            }

            public override void Delete()
            {
                _mavftp.kCmdRemoveDirectory(FullPath, new CancellationTokenSource());
            }

            public async Task<DirectoryInfo[]> GetDirectories()
            {
                try
                {
                    // rerequest every time
                    await Task.Run(() =>
                    {
                        lock (_mavftp)
                        {
                            cache = _mavftp.kCmdListDirectory(FullPath, new CancellationTokenSource());
                        }
                    }).ConfigureAwait(true);
                    return cache.Where(a => a.isDirectory && a.Name != "." && a.Name != "..")
                        .Select(a => new DirectoryInfo(a.FullName, _mavftp)).ToArray();

                }
                catch (Exception e)
                {
                    log.Error(e);
                }

                return new DirectoryInfo[] { };
            }

            public async Task<IEnumerable<MAVFtp.FtpFileInfo>> GetFiles()
            {
                try
                {
                    if (cache == null)
                        await GetDirectories().ConfigureAwait(true);

                    // rerequest every time
                    return cache.Where(a => !a.isDirectory);
                }
                catch (Exception e)
                {
                    log.Error(e);
                }

                return new List<MAVFtp.FtpFileInfo>();
            }
        }

        private async void ListView1_DragDrop(object sender, DragEventArgs e)
        {
            var files = e.Data.GetData(DataFormats.FileDrop) as string[];

            foreach (var file in files)
            {
                try
                {
                    await UploadFile(file).ConfigureAwait(true);
                }
                catch (Exception exception)
                {
                    log.Error(exception);
                    CustomMessageBox.Show(exception.Message);
                }
            }

            TreeView1_NodeMouseClick(null,
                new TreeNodeMouseClickEventArgs(treeView1.SelectedNode, MouseButtons.Left, 1, 1, 1));
        }

        private void ListView1_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            if (listView1.Sorting == SortOrder.Descending)
                listView1.Sorting = SortOrder.Ascending;
            else
                listView1.Sorting = SortOrder.Descending;

            listView1.ListViewItemSorter = Comparer<ListViewItem>.Create(((o, o1) =>
            {
                var v1 = o.SubItems[e.Column].Text;
                var v2 = o1.SubItems[e.Column].Text;
                if (v1.All(a => a >= '0' && a <= '9') && v2.All(a => a >= '0' && a <= '9'))
                {
                    if (listView1.Sorting == SortOrder.Descending)
                        return double.Parse("0" + v1).CompareTo(double.Parse("0" + v2)) * -1;
                    return double.Parse("0" + v1).CompareTo(double.Parse("0" + v2));
                }

                if (listView1.Sorting == SortOrder.Descending)
                    return v1.CompareTo(v2) * -1;
                return v1.CompareTo(v2);
            }));
        }

        private async void DownloadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            toolStripStatusLabel1.Text = "Download ";
            var sfd = new FolderBrowserDialog();
            sfd.SelectedPath = Settings.GetUserDataDirectory();
            var dr = sfd.ShowDialog();
            foreach (ListViewItem listView1SelectedItem in listView1.SelectedItems)
            {
                toolStripStatusLabel1.Text = "Download " + listView1SelectedItem.Text;
                if (dr == DialogResult.OK)
                {
                    var path = treeView1.SelectedNode.FullPath + "/" + listView1SelectedItem.Text;
                    ProgressReporterDialogue prd = new ProgressReporterDialogue();
                    CancellationTokenSource cancel = new CancellationTokenSource();
                    prd.doWorkArgs.CancelRequestChanged += (o, args) =>
                    {
                        prd.doWorkArgs.ErrorMessage = "User Cancel";
                        cancel.Cancel();
                        _mavftp.kCmdResetSessions();
                    };
                    prd.doWorkArgs.ForceExit = false;
                    Action<string, int> progress = delegate (string message, int i)
                    {
                        prd.UpdateProgressAndStatus(i, toolStripStatusLabel1.Text);
                    };
                    _mavftp.Progress += progress;

                    prd.DoWork += (iprd) =>
                    {
                        var ms = _mavftp.GetFile(path, cancel, false);
                        if (cancel.IsCancellationRequested)
                        {
                            iprd.doWorkArgs.CancelAcknowledged = true;
                            iprd.doWorkArgs.CancelRequested = true;
                            return;
                        }

                        var file = Path.Combine(sfd.SelectedPath, listView1SelectedItem.Text);
                        int a = 0;
                        while (File.Exists(file))
                        {
                            file = Path.Combine(sfd.SelectedPath, listView1SelectedItem.Text) + a++;
                        }
                        File.WriteAllBytes(file, ms.ToArray());
                    };
                    prd.RunBackgroundOperationAsync();
                    _mavftp.Progress -= progress;
                }
                else if (dr == DialogResult.Cancel)
                {
                    return;
                }
            }

            toolStripStatusLabel1.Text = "Ready";
        }

        private async void UploadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Multiselect = true;
            var dr = ofd.ShowDialog();
            if (dr == DialogResult.OK)
            {
                foreach (var ofdFileName in ofd.FileNames)
                {
                    try
                    {
                        await UploadFile(ofdFileName).ConfigureAwait(true);
                    }
                    catch (Exception exception)
                    {
                        log.Error(exception);
                        CustomMessageBox.Show(exception.Message);
                    }
                }
            }

            TreeView1_NodeMouseClick(null,
                new TreeNodeMouseClickEventArgs(treeView1.SelectedNode, MouseButtons.Left, 1, 1, 1));
        }

        private async Task UploadFile(string ofdFileName)
        {
            toolStripStatusLabel1.Text = "Upload " + Path.GetFileName(ofdFileName);
            var fn = treeView1.SelectedNode.FullPath + "/" + Path.GetFileName(ofdFileName);
            ProgressReporterDialogue prd = new ProgressReporterDialogue();
            CancellationTokenSource cancel = new CancellationTokenSource();
            prd.doWorkArgs.CancelRequestChanged += (o, args) =>
            {
                prd.doWorkArgs.ErrorMessage = "User Cancel";
                cancel.Cancel();
                _mavftp.kCmdResetSessions();
            };
            prd.doWorkArgs.ForceExit = false;

            Action<string, int> progress = delegate (string message, int i)
            {
                prd.UpdateProgressAndStatus(i, toolStripStatusLabel1.Text);
            };
            _mavftp.Progress += progress;

            prd.DoWork += (iprd) =>
            {
                _mavftp.UploadFile(fn, ofdFileName, cancel);
                if (cancel.IsCancellationRequested)
                {
                    iprd.doWorkArgs.CancelAcknowledged = true;
                    iprd.doWorkArgs.CancelRequested = true;
                    return;
                }

                prd.UpdateProgressAndStatus(-1, "Calc CRC");
                uint crc = 0;
                _mavftp.kCmdCalcFileCRC32(fn, ref crc, cancel);
                var crc32a = MAVFtp.crc_crc32(0, File.ReadAllBytes(ofdFileName));
                if (crc32a != crc)
                {
                    throw new BadCrcException();
                }
            };
            prd.RunBackgroundOperationAsync();
            _mavftp.Progress -= progress;
            toolStripStatusLabel1.Text = "Ready";
        }

        private void DeleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem listView1SelectedItem in listView1.SelectedItems)
            {
                toolStripStatusLabel1.Text = "Delete " + listView1SelectedItem.Text;
                ProgressReporterDialogue prd = new ProgressReporterDialogue();
                CancellationTokenSource cancel = new CancellationTokenSource();
                prd.doWorkArgs.CancelRequestChanged += (o, args) =>
                {
                    prd.doWorkArgs.ErrorMessage = "User Cancel";
                    cancel.Cancel();
                    _mavftp.kCmdResetSessions();
                };
                prd.doWorkArgs.ForceExit = false; 
                string fullName = ((DirectoryInfo)listView1SelectedItem.Tag).FullName;
                string text = listView1SelectedItem.Text;
                prd.DoWork += (iprd) =>
                {                   
                    var success = _mavftp.kCmdRemoveFile(fullName + "/" +
                                                         text, cancel);
                    if (!success)
                        CustomMessageBox.Show("Failed to delete file", text);
                };

                prd.RunBackgroundOperationAsync();
            }

            TreeView1_NodeMouseClick(null,
                new TreeNodeMouseClickEventArgs(treeView1.SelectedNode, MouseButtons.Left, 1, 1, 1));
            toolStripStatusLabel1.Text = "Ready";
        }

        private void RenameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            listView1.SelectedItems[0].BeginEdit();
        }

        private void ListView1_AfterLabelEdit(object sender, LabelEditEventArgs e)
        {
            if (e.Label == null)
                return;
            ProgressReporterDialogue prd = new ProgressReporterDialogue();
            CancellationTokenSource cancel = new CancellationTokenSource();
            prd.doWorkArgs.CancelRequestChanged += (o, args) =>
            {
                prd.doWorkArgs.ErrorMessage = "User Cancel";
                cancel.Cancel();
                _mavftp.kCmdResetSessions();
            };
            prd.doWorkArgs.ForceExit = false;
            var selectedNodeFullPath = treeView1.SelectedNode.FullPath; 
            var text = listView1.SelectedItems[0].Text;
            string label = e.Label;
            prd.DoWork += (iprd) =>
            {               
                _mavftp.kCmdRename(selectedNodeFullPath + "/" + text,
                    selectedNodeFullPath + "/" + label, cancel);
            };

            prd.RunBackgroundOperationAsync();
            TreeView1_NodeMouseClick(null,
                new TreeNodeMouseClickEventArgs(treeView1.SelectedNode, MouseButtons.Left, 1, 1, 1));
            toolStripStatusLabel1.Text = "Ready";
        }

        private void NewFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string folder = "";
            var dr = InputBox.Show("Folder Name", "Enter folder name", ref folder);
            if (dr == DialogResult.OK)
            {
                ProgressReporterDialogue prd = new ProgressReporterDialogue();
                CancellationTokenSource cancel = new CancellationTokenSource();
                prd.doWorkArgs.CancelRequestChanged += (o, args) =>
                {
                    prd.doWorkArgs.ErrorMessage = "User Cancel";
                    cancel.Cancel();
                    _mavftp.kCmdResetSessions();
                };
                prd.doWorkArgs.ForceExit = false;
                string fullPath = treeView1.SelectedNode.FullPath;
                prd.DoWork += (iprd) =>
                {
                    if (!_mavftp.kCmdCreateDirectory(fullPath + "/" + folder,
                        cancel))
                    {
                        CustomMessageBox.Show("Failed to create directory", Strings.ERROR);
                    }
                };

                prd.RunBackgroundOperationAsync();
            }

            TreeView1_NodeMouseClick(null,
                new TreeNodeMouseClickEventArgs(treeView1.SelectedNode, MouseButtons.Left, 1, 1, 1));
            toolStripStatusLabel1.Text = "Ready";
        }

        private void GetCRC32ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ProgressReporterDialogue prd = new ProgressReporterDialogue();
            CancellationTokenSource cancel = new CancellationTokenSource();
            prd.doWorkArgs.CancelRequestChanged += (o, args) =>
            {
                prd.doWorkArgs.ErrorMessage = "User Cancel";
                cancel.Cancel();
                _mavftp.kCmdResetSessions();
            };
            prd.doWorkArgs.ForceExit = false;
            var crc = 0u;
            string fullPath = treeView1.SelectedNode.FullPath;
            string text = listView1.SelectedItems[0].Text;
            prd.DoWork += (iprd) =>
            {

                _mavftp.kCmdCalcFileCRC32(fullPath + "/" + text,
                    ref crc, cancel);
            };

            prd.RunBackgroundOperationAsync();

            CustomMessageBox.Show(listView1.SelectedItems[0].Text + ": 0x" + crc.ToString("X"));
        }

        private void ListView1_MouseDown(object sender, MouseEventArgs e)
        {

        }

        private void ListView1_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Copy;
            else
                e.Effect = DragDropEffects.None;
        }

        private void ListView1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {
                treeView1.SelectedNode?.Expand();
                // find child node with name
                foreach (TreeNode node in treeView1.SelectedNode?.Nodes)
                {
                    if (node.Text == listView1.SelectedItems[0].Text)
                    {
                        treeView1.SelectedNode = node;

                        TreeView1_NodeMouseClick(null,
                            new TreeNodeMouseClickEventArgs(treeView1.SelectedNode, MouseButtons.Left, 1, 1, 1));
                        break;
                    }
                }
            }
        }

        private void DownloadBurstToolStripMenuItem_Click(object sender, EventArgs e)
        {
            toolStripStatusLabel1.Text = "Download ";
            var sfd = new FolderBrowserDialog();
            sfd.SelectedPath = Settings.GetUserDataDirectory();
            var dr = sfd.ShowDialog();
            foreach (ListViewItem listView1SelectedItem in listView1.SelectedItems)
            {
                if (dr == DialogResult.OK)
                {
                    toolStripStatusLabel1.Text = "Download " + listView1SelectedItem.Text;
                    var path = treeView1.SelectedNode.FullPath + "/" + listView1SelectedItem.Text;
                    ProgressReporterDialogue prd = new ProgressReporterDialogue();
                    CancellationTokenSource cancel = new CancellationTokenSource();
                    prd.doWorkArgs.CancelRequestChanged += (o, args) =>
                    {
                        prd.doWorkArgs.ErrorMessage = "User Cancel";
                        cancel.Cancel();
                        _mavftp.kCmdResetSessions();
                    };
                    prd.doWorkArgs.ForceExit = false;

                    Action<string, int> progress = delegate (string message, int i)
                    {
                        prd.UpdateProgressAndStatus(i, toolStripStatusLabel1.Text);
                    };
                    _mavftp.Progress += progress;

                    prd.DoWork += (iprd) =>
                    {
                        var ms = _mavftp.GetFile(path, cancel);
                        if (cancel.IsCancellationRequested)
                        {
                            iprd.doWorkArgs.CancelAcknowledged = true;
                            iprd.doWorkArgs.CancelRequested = true;
                            return;
                        }

                        var file = Path.Combine(sfd.SelectedPath, listView1SelectedItem.Text);
                        int a = 0;
                        while (File.Exists(file))
                        {
                            file = Path.Combine(sfd.SelectedPath, listView1SelectedItem.Text) + a++;
                        }
                        File.WriteAllBytes(file, ms.ToArray());
                    };
                    prd.RunBackgroundOperationAsync();
                    _mavftp.Progress -= progress;
                }
                else if (dr == DialogResult.Cancel)
                {
                    return;
                }
            }

            toolStripStatusLabel1.Text = "Ready";
        }

        private void MavFTPUI_Load(object sender, EventArgs e)
        {
            PopulateTreeView();
        }
    }
}