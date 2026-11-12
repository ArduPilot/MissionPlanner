using MissionPlanner;
using MissionPlanner.Plugin;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

/// <summary>
/// AnonymizeBinlogPlugin - A Mission Planner plugin to anonymize ArduPilot .bin log files by applying random offsets to GPS coordinates.
/// derived from https://github.com/EosBandi/anomizer
/// </summary>

namespace AnonymizeBinlog
{
    public class AnonymizeBinlogPlugin : MissionPlanner.Plugin.Plugin
    {
        public override string Name => "Anonymize Binlog";
        public override string Version => "1.0";
        public override string Author => "MissionPlanner";

        public override bool Init()
        {
            return true;
        }

        public override bool Loaded()
        {
            Host.FDMenuMap.Items.Add(new ToolStripMenuItem("Anonymize Bin Log...", null, OnAnonymizeBinlog));
            return true;
        }

        public override bool Loop()
        {
            return true;
        }

        public override bool Exit()
        {
            return true;
        }

        private void OnAnonymizeBinlog(object sender, EventArgs e)
        {
            string inputPath;
            using (var ofd = new OpenFileDialog())
            {
                ofd.Title = "Select ArduPilot .bin log to anonymize";
                ofd.Filter = "BIN logs (*.bin)|*.bin|All files (*.*)|*.*";
                ofd.RestoreDirectory = true;
                if (ofd.ShowDialog() != DialogResult.OK)
                    return;
                inputPath = ofd.FileName;
            }

            string defaultOutput = Path.Combine(
                Path.GetDirectoryName(inputPath),
                Path.GetFileNameWithoutExtension(inputPath) + "_anon" + Path.GetExtension(inputPath));

            string outputPath;
            using (var sfd = new SaveFileDialog())
            {
                sfd.Title = "Save anonymized log";
                sfd.Filter = "BIN logs (*.bin)|*.bin|All files (*.*)|*.*";
                sfd.FileName = Path.GetFileName(defaultOutput);
                sfd.InitialDirectory = Path.GetDirectoryName(inputPath);
                sfd.RestoreDirectory = true;
                if (sfd.ShowDialog() != DialogResult.OK)
                    return;
                outputPath = sfd.FileName;
            }

            string offsetLatStr = "0";
            string offsetLonStr = "0";

            if (InputBox("Latitude Offset (degrees, blank = random)", ref offsetLatStr) != DialogResult.OK)
                return;
            if (InputBox("Longitude Offset (degrees, blank = random)", ref offsetLonStr) != DialogResult.OK)
                return;

            var rng = new Random();
            double offsetLat = string.IsNullOrWhiteSpace(offsetLatStr) ? RandomOffset(rng) : double.Parse(offsetLatStr);
            double offsetLon = string.IsNullOrWhiteSpace(offsetLonStr) ? RandomOffset(rng) : double.Parse(offsetLonStr);

            try
            {
                Anonymizer.Anonymize(inputPath, outputPath, offsetLat, offsetLon);
                MessageBox.Show(
                    $"Anonymization complete.\n\nOutput: {outputPath}\n\nLat offset: {offsetLat:+0.000000;-0.000000}°\nLon offset: {offsetLon:+0.000000;-0.000000}°",
                    "Anonymize Bin Log",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Anonymize Bin Log", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private static double RandomOffset(Random rng)
        {
            double magnitude = rng.NextDouble() * 1.5 + 0.5;
            double sign = rng.Next(2) == 0 ? -1.0 : 1.0;
            return magnitude * sign;
        }

        private static DialogResult InputBox(string prompt, ref string value)
        {
            var form = new Form { Text = "Anonymize Bin Log", Width = 360, Height = 140, FormBorderStyle = FormBorderStyle.FixedDialog, StartPosition = FormStartPosition.CenterParent, MaximizeBox = false, MinimizeBox = false };
            var label = new Label { Left = 10, Top = 10, Width = 330, Text = prompt };
            var textBox = new TextBox { Left = 10, Top = 35, Width = 330, Text = value };
            var ok = new Button { Text = "OK", Left = 170, Top = 65, Width = 80, DialogResult = DialogResult.OK };
            var cancel = new Button { Text = "Cancel", Left = 260, Top = 65, Width = 80, DialogResult = DialogResult.Cancel };
            form.Controls.AddRange(new Control[] { label, textBox, ok, cancel });
            form.AcceptButton = ok;
            form.CancelButton = cancel;
            var result = form.ShowDialog();
            value = textBox.Text;
            return result;
        }
    }

    internal static class Anonymizer
    {
        const byte HEAD1 = 0xA3;
        const byte HEAD2 = 0x95;
        const byte FMT_MSG_ID = 128;
        const int FMT_MSG_LEN = 89;

        const char UNIT_LATITUDE = 'D';
        const char UNIT_LONGITUDE = 'U';

        static readonly HashSet<string> FallbackLatNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "lat", "hlat", "dlat", "oalat", "dlt", "olt", "elat",
            "olat", "clat", "trlat", "wplat", "rlat", "tp_lat"
        };

        static readonly HashSet<string> FallbackLngNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "lng", "lon", "hlon", "hlng", "dlng", "oalng", "dlg", "olg",
            "elng", "olng", "clng", "trlng", "wplng", "rlng", "tp_lng"
        };

        static readonly Dictionary<char, int> DfFormatSize = new Dictionary<char, int>
        {
            {'a', 64}, {'b', 1}, {'B', 1}, {'c', 2}, {'C', 2},
            {'d', 8},  {'e', 4}, {'E', 4}, {'f', 4}, {'h', 2},
            {'H', 2},  {'i', 4}, {'I', 4}, {'L', 4}, {'M', 1},
            {'n', 4},  {'N', 16},{'Z', 64},{'q', 8}, {'Q', 8},
            {'A', 128}
        };

        static readonly HashSet<char> StringFormats = new HashSet<char> { 'a', 'n', 'N', 'Z', 'A' };

        class FmtDef
        {
            public string Name;
            public int Length;
            public string FmtStr;
            public string[] Columns;
            public FmtDef(string name, int length, string fmtStr, string[] columns)
            { Name = name; Length = length; FmtStr = fmtStr; Columns = columns; }
        }

        class FieldOffset
        {
            public int ByteOffset;
            public char FmtChar;
            public int Size;
            public FieldOffset(int byteOffset, char fmtChar, int size)
            { ByteOffset = byteOffset; FmtChar = fmtChar; Size = size; }
        }

        class CoordPatch
        {
            public string FieldName;
            public int ByteOffset;
            public char FmtChar;
            public int Size;
            public string CoordType;
            public CoordPatch(string fieldName, int byteOffset, char fmtChar, int size, string coordType)
            { FieldName = fieldName; ByteOffset = byteOffset; FmtChar = fmtChar; Size = size; CoordType = coordType; }
        }

        static Dictionary<int, FmtDef> ParseFormats(byte[] data)
        {
            var fmtDefs = new Dictionary<int, FmtDef>();
            int pos = 0;
            int end = data.Length - 2;

            while (pos < end)
            {
                if (data[pos] == HEAD1 && data[pos + 1] == HEAD2)
                {
                    byte msgId = data[pos + 2];
                    if (msgId == FMT_MSG_ID)
                    {
                        if (pos + FMT_MSG_LEN <= data.Length)
                        {
                            byte typeId = data[pos + 3];
                            byte msgLen = data[pos + 4];
                            string name = ReadAscii(data, pos + 5, 4);
                            string fmtStr = ReadAscii(data, pos + 9, 16);
                            string columns = ReadAscii(data, pos + 25, 64);
                            fmtDefs[typeId] = new FmtDef(name, msgLen, fmtStr, columns.Split(','));
                        }
                        pos += FMT_MSG_LEN;
                    }
                    else if (fmtDefs.TryGetValue(msgId, out var def))
                    {
                        pos += def.Length;
                    }
                    else
                    {
                        pos++;
                    }
                }
                else
                {
                    pos++;
                }
            }

            return fmtDefs;
        }

        static Dictionary<int, string> ParseFmtu(byte[] data, Dictionary<int, FmtDef> fmtDefs)
        {
            int? fmtuTid = null;
            foreach (var kvp in fmtDefs)
            {
                if (kvp.Value.Name == "FMTU")
                {
                    fmtuTid = kvp.Key;
                    break;
                }
            }

            if (fmtuTid == null)
                return new Dictionary<int, string>();

            int fmtuLen = fmtDefs[fmtuTid.Value].Length;
            var fmtuMap = new Dictionary<int, string>();
            int pos = 0;
            int end = data.Length - 2;

            while (pos < end)
            {
                if (data[pos] == HEAD1 && data[pos + 1] == HEAD2)
                {
                    byte msgId = data[pos + 2];
                    if (msgId == fmtuTid.Value && pos + fmtuLen <= data.Length)
                    {
                        byte fmtType = data[pos + 11];
                        string unitIds = ReadAscii(data, pos + 12, 16);
                        fmtuMap[fmtType] = unitIds;
                        pos += fmtuLen;
                    }
                    else if (fmtDefs.TryGetValue(msgId, out var def))
                    {
                        pos += def.Length;
                    }
                    else
                    {
                        pos++;
                    }
                }
                else
                {
                    pos++;
                }
            }

            return fmtuMap;
        }

        static List<FieldOffset> ComputeFieldOffsets(string fmtStr)
        {
            int offset = 3;
            var offsets = new List<FieldOffset>();
            foreach (char fc in fmtStr)
            {
                if (!DfFormatSize.TryGetValue(fc, out int sz))
                    break;
                offsets.Add(new FieldOffset(offset, fc, sz));
                offset += sz;
            }
            return offsets;
        }

        static Dictionary<int, List<CoordPatch>> IdentifyCoordFields(
            Dictionary<int, FmtDef> fmtDefs,
            Dictionary<int, string> fmtuMap)
        {
            bool useFmtu = fmtuMap.Count > 0;
            var result = new Dictionary<int, List<CoordPatch>>();

            foreach (var kvp in fmtDefs)
            {
                int typeId = kvp.Key;
                var def = kvp.Value;

                if (def.Name == "FMT" || def.Name == "FMTU" || def.Name == "MULT" || def.Name == "UNIT")
                    continue;

                var fieldOffsets = ComputeFieldOffsets(def.FmtStr);
                int count = Math.Min(fieldOffsets.Count, def.Columns.Length);
                var patches = new List<CoordPatch>();

                if (useFmtu && fmtuMap.TryGetValue(typeId, out string unitIds))
                {
                    for (int i = 0; i < unitIds.Length && i < count; i++)
                    {
                        string coordType = null;
                        if (unitIds[i] == UNIT_LATITUDE) coordType = "lat";
                        else if (unitIds[i] == UNIT_LONGITUDE) coordType = "lng";

                        if (coordType != null)
                        {
                            var fo = fieldOffsets[i];
                            patches.Add(new CoordPatch(def.Columns[i], fo.ByteOffset, fo.FmtChar, fo.Size, coordType));
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < count; i++)
                    {
                        string col = def.Columns[i];
                        string cl = col.ToLowerInvariant();
                        string coordType = null;

                        if (FallbackLatNames.Contains(cl))
                            coordType = "lat";
                        else if (FallbackLngNames.Contains(cl))
                            coordType = "lng";
                        else if (fieldOffsets[i].FmtChar == 'L')
                        {
                            if (cl.Contains("lat") || cl.Contains("lt"))
                                coordType = "lat";
                            else if (cl.Contains("lng") || cl.Contains("lon") || cl.Contains("lg"))
                                coordType = "lng";
                        }

                        if (coordType != null)
                        {
                            var fo = fieldOffsets[i];
                            patches.Add(new CoordPatch(col, fo.ByteOffset, fo.FmtChar, fo.Size, coordType));
                        }
                    }
                }

                if (patches.Count > 0)
                    result[typeId] = patches;
            }

            return result;
        }

        static double ReadValue(byte[] data, int offset, char fc)
        {
            switch (fc)
            {
                case 'b': return (sbyte)data[offset];
                case 'B': case 'M': return data[offset];
                case 'c': case 'h': return BitConverter.ToInt16(data, offset);
                case 'C': case 'H': return BitConverter.ToUInt16(data, offset);
                case 'e': case 'i': case 'L': return BitConverter.ToInt32(data, offset);
                case 'E': case 'I': return BitConverter.ToUInt32(data, offset);
                case 'f': return BitConverter.ToSingle(data, offset);
                case 'd': return BitConverter.ToDouble(data, offset);
                case 'q': return BitConverter.ToInt64(data, offset);
                case 'Q': return BitConverter.ToUInt64(data, offset);
                default: return 0;
            }
        }

        static void WriteValue(byte[] data, int offset, char fc, double newVal)
        {
            byte[] bytes;
            switch (fc)
            {
                case 'b': bytes = new[] { (byte)(sbyte)newVal }; break;
                case 'B': case 'M': bytes = new[] { (byte)newVal }; break;
                case 'c': case 'h': bytes = BitConverter.GetBytes((short)newVal); break;
                case 'C': case 'H': bytes = BitConverter.GetBytes((ushort)newVal); break;
                case 'e': case 'i': case 'L': bytes = BitConverter.GetBytes((int)newVal); break;
                case 'E': case 'I': bytes = BitConverter.GetBytes((uint)newVal); break;
                case 'f': bytes = BitConverter.GetBytes((float)newVal); break;
                case 'd': bytes = BitConverter.GetBytes(newVal); break;
                case 'q': bytes = BitConverter.GetBytes((long)newVal); break;
                case 'Q': bytes = BitConverter.GetBytes((ulong)newVal); break;
                default: return;
            }
            Array.Copy(bytes, 0, data, offset, bytes.Length);
        }

        static double OffsetValue(double oldVal, char fc, double offsetDeg)
        {
            switch (fc)
            {
                case 'L': return (int)(oldVal + offsetDeg * 1e7);
                case 'I': return (uint)((int)(oldVal + offsetDeg * 1e7) & 0xFFFFFFFF);
                case 'i': return (int)(oldVal + offsetDeg * 1e7);
                case 'f':
                    if (Math.Abs(oldVal) > 1000)
                        return oldVal + offsetDeg * 1e7;
                    else
                        return oldVal + offsetDeg;
                case 'd': return oldVal + offsetDeg;
                default: return oldVal;
            }
        }

        public static void Anonymize(string inputPath, string outputPath, double offsetLat, double offsetLng)
        {
            byte[] data = File.ReadAllBytes(inputPath);

            var fmtDefs = ParseFormats(data);
            var fmtuMap = ParseFmtu(data, fmtDefs);
            var coordFields = IdentifyCoordFields(fmtDefs, fmtuMap);

            if (coordFields.Count == 0)
                throw new InvalidOperationException("No coordinate fields found in log file.");

            byte[] outData = (byte[])data.Clone();
            int pos = 0;
            int end = data.Length - 2;

            while (pos < end)
            {
                if (data[pos] == HEAD1 && data[pos + 1] == HEAD2)
                {
                    byte msgId = data[pos + 2];

                    if (msgId == FMT_MSG_ID)
                    {
                        pos += FMT_MSG_LEN;
                        continue;
                    }

                    if (!fmtDefs.TryGetValue(msgId, out var def))
                    {
                        pos++;
                        continue;
                    }

                    int msgLen = def.Length;

                    if (coordFields.TryGetValue(msgId, out var patches) && pos + msgLen <= data.Length)
                    {
                        foreach (var patch in patches)
                        {
                            int absOff = pos + patch.ByteOffset;
                            if (absOff + patch.Size > data.Length)
                                continue;
                            if (StringFormats.Contains(patch.FmtChar))
                                continue;

                            double oldVal;
                            try { oldVal = ReadValue(data, absOff, patch.FmtChar); }
                            catch { continue; }

                            if (oldVal == 0)
                                continue;

                            double degOffset = patch.CoordType == "lat" ? offsetLat : offsetLng;
                            double newVal = OffsetValue(oldVal, patch.FmtChar, degOffset);

                            try { WriteValue(outData, absOff, patch.FmtChar, newVal); }
                            catch { }
                        }
                    }

                    pos += msgLen;
                }
                else
                {
                    pos++;
                }
            }

            File.WriteAllBytes(outputPath, outData);
        }

        static string ReadAscii(byte[] data, int offset, int length)
        {
            int end = offset + length;
            if (end > data.Length) end = data.Length;
            int nullPos = Array.IndexOf(data, (byte)0, offset, end - offset);
            int actualLen = nullPos >= 0 ? nullPos - offset : end - offset;
            return Encoding.ASCII.GetString(data, offset, actualLen);
        }
    }
}
