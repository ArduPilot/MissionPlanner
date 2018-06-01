using System;
using System.Collections.Generic;
using System.Text;
using System.IO;


namespace Core.FileHandling
{
    public class LoadSave
    {
        private string fileType;
        private string filter;
        private List<FileFilter> filterList = new List<FileFilter>();
        //---
        public string FilePath;
        public string Data;
        public string InitialDirectory;
        public string DefaultFilename;
        //---
        /// <summary>
        /// A LoadSave object provides easy access to Load and Save dialogues and keeps track of the filepath of the file currently in use
        /// </summary>
        /// <param name="file_type">The extension of file to be handled eg. "xml" or "txt"</param>
        public LoadSave()
            : this("*") {

        }
        public LoadSave(string filterName, params string[] exts) {
            Init(new FileFilter(filterName, exts));
        }
        public LoadSave(string ext) {
            Init(new FileFilter(ext));
        }

        private void Init(FileFilter dFilter) {
            filterList.Add(dFilter);
            fileType = dFilter.Extension;
            RegenFilter();
            FilePath = null;
            InitialDirectory = null;
            DefaultFilename = null;
        }

        public void AddFilter(string ext) {
            filterList.Add(new FileFilter(ext));
            RegenFilter();
        }

        public void AddFilter(string name, params string[] exts) {
            filterList.Add(new FileFilter(name, exts));
            RegenFilter();
        }

        private bool m_AllowAllFiles;
        public bool AllowAllFiles {
            get {
                return m_AllowAllFiles;
            }
            set {
                m_AllowAllFiles = value;
                RegenFilter();
            }
        }

        private void RegenFilter() {
            //Sql files (*.sql)|*.sql|All files (*.*)|*.*
            //Image Files(*.BMP;*.JPG;*.GIF)|*.BMP;*.JPG;*.GIF
            filter = "";
            foreach (FileFilter f in filterList) {
                if (filter != "") {
                    filter += "|";
                }
                filter += f.Name + " (" + f.ExtensionString + ")|" + f.ExtensionString + "";
            }
            if (m_AllowAllFiles) {
                filter += "|All files (*.*)|*.*";
            }
        }

        public bool HasSavePath {
            get {
                return FilePath != null;
            }
        }
        public bool FileExists() {
            return File.Exists(FilePath);
        }

 


        public void Save() {
            if (Data == null) {
                throw new Exception("No data to save");
            }
            if (HasSavePath) {
                SaveData();
            } 
        }

        public void SaveAs(string filePath) {
            FilePath = filePath;
            Save();
        }
        public void SaveAs(string data, string filePath) {
            FilePath = filePath;
            Data = data;
            Save();
        }

        public void Save(string data) {
            Data = data;
            Save();
        }

        private void SaveData() {
            try {
                StreamWriter save_writer = null;
                if (FilePath != null) {
                    save_writer = new StreamWriter(FilePath);
                }
                if (save_writer != null) {
                    save_writer.Write(Data);
                    save_writer.Close();
                }
            }
            catch (Exception e) {
                throw e;
            }
        }


        public bool Load(string path) {
            Data = GetFileContentsFromPath(path);
            if (Data != null) {
                FilePath = path;
                return true;
            } else {
                return false;
            }
        }

        /// <summary>
        /// Unsets the FilePath (will prompt a SaveAs on Save).
        /// </summary>
        /// <param name="data">The new data set</param>
        public void NewFile(string data) {
            Data = data;
            FilePath = null;
        }

        /// <summary>
        /// Unsets the FilePath (will prompt a SaveAs on Save). Sets Data to null.
        /// </summary>
        public void NewFile() {
            Data = null;
            FilePath = null;
        }

        public string GetFileContentsFromPath(string f_path) {
            if (f_path != null) {
                FilePath = f_path;
                StreamReader s_reader = new StreamReader(f_path);
                string f_data = s_reader.ReadToEnd();
                s_reader.Close();
                return f_data;
            } else {
                return null;
            }
        }
        
        internal class FileFilter
        {
            internal string Name;
            internal string Extension;
            internal List<string> Extensions;

            internal FileFilter(string name, params string[] exts) {
                Extensions = new List<string>(exts);
                for (int i = 0; i < Extensions.Count; i++) {
                    string ext = Extensions[i];
                    if (ext.StartsWith(".")) {
                        Extensions[i] = ext.Substring(1);
                    }
                }
                Name = name;
                Extension = exts[0];
            }

            internal FileFilter(string name, string ext) {
                if (ext.StartsWith(".")) {
                    ext = ext.Substring(1);
                }
                Name = name;
                Extension = ext;
            }

            internal FileFilter(string ext) {
                if (ext.StartsWith(".")) {
                    ext = ext.Substring(1);
                }
                Extension = ext;
                Name = ext.ToUpper();
            }

            public string ExtensionString {
                get {
                    if (Extensions == null || Extensions.Count < 2) {
                        return "*." + Extension;
                    }
                    //*.BMP;*.JPG;*.GIF
                    string ans = "";
                    foreach (string ext in Extensions) {
                        ans += "*." + ext + ";";
                    }
                    return ans.Substring(0, ans.Length - 1);
                }
            }
        }
    }


}
