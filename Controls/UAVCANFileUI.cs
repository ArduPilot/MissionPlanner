using log4net;
using MissionPlanner.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using UAVCAN;

namespace MissionPlanner.Controls
{
    public partial class UAVCANFileUI : UserControl
    {
        private static readonly ILog log =
            LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private readonly uavcan _can;
        private readonly byte _nodeid;

        public UAVCANFileUI(uavcan can, byte nodeid)
        {
            _can = can;
            _nodeid = nodeid;
            /*_mavftp = new MAVFtp(_mav, (byte) _mav.sysidcurrent, (byte) mav.compidcurrent);
            _mavftp.Progress += (percent) =>
            {
                if (toolStripProgressBar1.Value == percent)
                    return;

                if (this.IsDisposed)
                {
                    _mavftp = null;
                    return;
                }

                this.BeginInvokeIfRequired(() => {
                    toolStripProgressBar1.Value = percent;
                    statusStrip1.Refresh();
                });
            };*/
            InitializeComponent();

            ThemeManager.ApplyThemeTo(this);

            treeView1.PathSeparator = "/";
        }

        private async void PopulateTreeView()
        {
            toolStripStatusLabel1.Text = "Updating Folders";

            treeView1.BeginUpdate();

            treeView1.Enabled = false;

            treeView1.Nodes.Clear();

            TreeNode rootNode = null;

            DirectoryInfo info = new DirectoryInfo(@"/", _can, _nodeid);
            if (info.Exists)
            {
                rootNode = new TreeNode(info.Name, 0, 0);
                rootNode.Tag = info;
                await GetDirectories(await info.GetDirectories(), rootNode);
                treeView1.Nodes.Add(rootNode);
            }

            toolStripStatusLabel1.Text = "Ready";

            treeView1.Enabled = true;
            
            treeView1.EndUpdate();

            treeView1.SelectedNode = rootNode;

            TreeView1_NodeMouseClick(this, new TreeNodeMouseClickEventArgs(rootNode, MouseButtons.Left, 1, 1, 1));
        }

        private async Task GetDirectories(DirectoryInfo[] subDirs,
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

            DirectoryInfo[] subSubDirs;
            foreach (var treeNode in info)
            {
                //subSubDirs = await ((DirectoryInfo)treeNode.Tag).GetDirectories();
                //if (subSubDirs.Length != 0)
                {
                    //  await GetDirectories(subSubDirs, treeNode);
                }

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

            var dirs = await nodeDirInfo.GetDirectories();

            newSelected.Nodes.Clear();

            await GetDirectories(dirs, newSelected).ConfigureAwait(true);

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
                    new ListViewItem.ListViewSubItem(item, file.Size.ToString())
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
            private readonly uavcan _can;
            private readonly byte _nodeid;
            private List<uavcan.UAVCANFileInfo> cache;

            public DirectoryInfo(string dir, uavcan can, byte nodeid)
            {
                _can = can;
                _nodeid = nodeid;
                FullPath = dir;
            }

            public override string Name
            {
                get { return Path.GetFileName(FullPath); }
            }

            public override bool Exists => true;

            public override void Delete()
            {

            }

            public async Task<DirectoryInfo[]> GetDirectories()
            {
                // rerequest every time
                await Task.Run(() =>
                {
                    lock (_can)
                    {
                        cache = _can.FileGetDirectoryEntrys(_nodeid, FullPath);
                    }
                }).ConfigureAwait(true);
                return cache.Where(a => a.isDirectory && a.Name != "." && a.Name != "..")
                    .Select(a => new DirectoryInfo(a.FullName, _can, _nodeid)).ToArray();
            }

            public async Task<IEnumerable<uavcan.UAVCANFileInfo>> GetFiles()
            {
                if (cache == null)
                    await GetDirectories();

                // rerequest every time
                return cache.Where(a => !a.isDirectory);
            }
        }

        private async void ListView1_DragDrop(object sender, DragEventArgs e)
        {
            var files = e.Data.GetData(DataFormats.FileDrop) as string[];

            foreach (var file in files)
            {
                await UploadFile(file).ConfigureAwait(true);
            }

            TreeView1_NodeMouseClick(null,
                new TreeNodeMouseClickEventArgs(treeView1.SelectedNode, MouseButtons.Left, 1, 1, 1));
        }

        private void ListView1_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            if (listView1.Sorting == null || listView1.Sorting == SortOrder.Descending)
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
            foreach (ListViewItem listView1SelectedItem in listView1.SelectedItems)
            {
                toolStripStatusLabel1.Text = "Download " + listView1SelectedItem.Text;
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.FileName = listView1SelectedItem.Text;
                sfd.RestoreDirectory = true;
                sfd.OverwritePrompt = true;
                var dr = sfd.ShowDialog();
                if (dr == DialogResult.OK)
                {
                    var path = treeView1.SelectedNode.FullPath + "/" + listView1SelectedItem.Text;
                    ProgressReporterDialogue prd = new ProgressReporterDialogue();
                    CancellationTokenSource cancel = new CancellationTokenSource();
                    prd.doWorkArgs.CancelRequestChanged += (o, args) =>
                    {
                        prd.doWorkArgs.ErrorMessage = "User Cancel";
                        cancel.Cancel();
                    };
                    prd.doWorkArgs.ForceExit = false;
                    Action<int> progress = delegate (int i) { prd.UpdateProgressAndStatus(i, toolStripStatusLabel1.Text); };
                    //_can.Progress += progress;

                    prd.DoWork += (iprd) =>
                    {
                        var ms = new MemoryStream();
                        _can.FileRead(_nodeid, path, File.OpenWrite(sfd.FileName), cancel.Token);
                        if (cancel.IsCancellationRequested)
                        {
                            iprd.doWorkArgs.CancelAcknowledged = true;
                            iprd.doWorkArgs.CancelRequested = true;
                            return;
                        }
                    };
                    prd.RunBackgroundOperationAsync();
                    //_mavftp.Progress -= progress;
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
                    await UploadFile(ofdFileName).ConfigureAwait(true);
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
            };
            prd.doWorkArgs.ForceExit = false;

            Action<int> progress = delegate (int i) { prd.UpdateProgressAndStatus(i, toolStripStatusLabel1.Text); };
            //_mavftp.Progress += progress;

            prd.DoWork += (iprd) =>
            {
                _can.FileWrite(_nodeid, fn, File.OpenRead(ofdFileName), cancel.Token);
                if (cancel.IsCancellationRequested)
                {
                    iprd.doWorkArgs.CancelAcknowledged = true;
                    iprd.doWorkArgs.CancelRequested = true;
                    return;
                }
            };
            prd.RunBackgroundOperationAsync();
            //_mavftp.Progress -= progress;
            toolStripStatusLabel1.Text = "Ready";
        }

        private void DeleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem listView1SelectedItem in listView1.SelectedItems)
            {
                toolStripStatusLabel1.Text = "Delete " + listView1SelectedItem.Text;
                //var success = _mavftp.kCmdRemoveFile(((DirectoryInfo)listView1SelectedItem.Tag).FullName + "/" + listView1SelectedItem.Text);
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

            //_mavftp.kCmdRename(treeView1.SelectedNode.FullPath + "/" + listView1.SelectedItems[0].Text,
            //    treeView1.SelectedNode.FullPath + "/" + e.Label);

            TreeView1_NodeMouseClick(null,
                new TreeNodeMouseClickEventArgs(treeView1.SelectedNode, MouseButtons.Left, 1, 1, 1));
            toolStripStatusLabel1.Text = "Ready";
        }

        private void NewFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string folder = "";
            var dr = InputBox.Show("Folder Name", "Enter folder name", ref folder);
            //if (dr == DialogResult.OK)
            // _mavftp.kCmdCreateDirectory(treeView1.SelectedNode.FullPath + "/" + folder);

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
                // _mavftp.kCmdResetSessions();
            };
            prd.doWorkArgs.ForceExit = false;
            var crc = 0u;
            prd.DoWork += (iprd) =>
            {
                //_mavftp.kCmdCalcFileCRC32(treeView1.SelectedNode.FullPath + "/" + listView1.SelectedItems[0].Text,
                //   ref crc, cancel);
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

        private void MavFTPUI_Load(object sender, EventArgs e)
        {
            PopulateTreeView();
        }
    }
}
