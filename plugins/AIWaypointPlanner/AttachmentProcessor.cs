using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using UglyToad.PdfPig;

namespace MissionPlanner.AIWaypointPlanner
{
    public enum AttachmentContentKind
    {
        ExtractedText,
        Image,
        NativePdf
    }

    public sealed class MissionAttachment
    {
        public string DisplayName { get; set; }
        public string MediaType { get; set; }
        public long SizeBytes { get; set; }
        public AttachmentContentKind Kind { get; set; }
        public string ExtractedText { get; set; }
        public string DataUrl { get; set; }
        public string Status { get; set; }

        public MissionAttachment()
        {
            DisplayName = string.Empty;
            MediaType = "application/octet-stream";
            ExtractedText = string.Empty;
            DataUrl = string.Empty;
            Status = string.Empty;
        }
    }

    public sealed class AttachmentProcessor
    {
        public const int MaximumAttachmentCount = 6;
        public const long MaximumFileBytes = 12L * 1024L * 1024L;
        public const long MaximumImageBytes = 8L * 1024L * 1024L;
        public const long MaximumTotalBytes = 24L * 1024L * 1024L;
        public const int MaximumExtractedCharacters = 120000;

        private static readonly HashSet<string> TextExtensions = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            ".txt", ".md", ".csv", ".tsv", ".json", ".xml", ".kml", ".gpx",
            ".yaml", ".yml", ".html", ".htm", ".log", ".ini", ".cfg"
        };

        private static readonly Dictionary<string, string> ImageMediaTypes =
            new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { ".png", "image/png" },
                { ".jpg", "image/jpeg" },
                { ".jpeg", "image/jpeg" },
                { ".webp", "image/webp" },
                { ".gif", "image/gif" }
            };

        public MissionAttachment Load(string path, IEnumerable<MissionAttachment> existingAttachments)
        {
            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentException("文件路径不能为空。", "path");

            var file = new FileInfo(path);
            if (!file.Exists)
                throw new FileNotFoundException("找不到所选文件。", path);
            if (file.Length <= 0)
                throw new InvalidOperationException("不能添加空文件：" + file.Name);
            if (file.Length > MaximumFileBytes)
                throw new InvalidOperationException("单个文件不能超过 12 MB：" + file.Name);

            List<MissionAttachment> existing = (existingAttachments ?? Enumerable.Empty<MissionAttachment>()).ToList();
            if (existing.Count >= MaximumAttachmentCount)
                throw new InvalidOperationException("最多只能添加 " + MaximumAttachmentCount + " 个文件。");
            if (existing.Sum(item => item.SizeBytes) + file.Length > MaximumTotalBytes)
                throw new InvalidOperationException("附件总大小不能超过 24 MB。");

            string extension = file.Extension.ToLowerInvariant();
            MissionAttachment attachment;
            if (ImageMediaTypes.ContainsKey(extension))
                attachment = LoadImage(file, ImageMediaTypes[extension]);
            else if (extension == ".pdf")
                attachment = LoadPdf(file);
            else if (extension == ".docx")
                attachment = LoadDocx(file);
            else if (TextExtensions.Contains(extension))
                attachment = LoadText(file, GetTextMediaType(extension));
            else
                throw new InvalidOperationException(
                    "暂不支持该文件类型：" + extension + "。可使用 PDF、DOCX、PNG/JPEG/WebP/GIF 或常见文本/数据文件。");

            int totalCharacters = existing.Sum(item => item.ExtractedText == null ? 0 : item.ExtractedText.Length) +
                                  attachment.ExtractedText.Length;
            if (totalCharacters > MaximumExtractedCharacters)
                throw new InvalidOperationException("附件提取文本总量不能超过 120,000 个字符，请精简或拆分资料。");
            return attachment;
        }

        public static void ValidateForProtocol(IEnumerable<MissionAttachment> attachments, ApiProtocol protocol)
        {
            List<MissionAttachment> files = (attachments ?? Enumerable.Empty<MissionAttachment>()).ToList();
            if (files.Count > MaximumAttachmentCount)
                throw new InvalidOperationException("附件数量超过安全上限。");
            if (files.Sum(item => item.SizeBytes) > MaximumTotalBytes)
                throw new InvalidOperationException("附件总大小超过安全上限。");
            if (files.Sum(item => item.ExtractedText == null ? 0 : item.ExtractedText.Length) > MaximumExtractedCharacters)
                throw new InvalidOperationException("附件提取文本超过安全上限。");
            if (protocol == ApiProtocol.ChatCompletions && files.Any(item => item.Kind == AttachmentContentKind.NativePdf))
            {
                throw new InvalidOperationException(
                    "该 PDF 没有可提取文本，需使用 Responses API 的原生 PDF 输入，或先对 PDF 进行 OCR 后再添加。");
            }
        }

        private static MissionAttachment LoadImage(FileInfo file, string mediaType)
        {
            if (file.Length > MaximumImageBytes)
                throw new InvalidOperationException("单张图像不能超过 8 MB：" + file.Name);
            byte[] bytes = File.ReadAllBytes(file.FullName);
            return new MissionAttachment
            {
                DisplayName = file.Name,
                MediaType = mediaType,
                SizeBytes = file.Length,
                Kind = AttachmentContentKind.Image,
                DataUrl = "data:" + mediaType + ";base64," + Convert.ToBase64String(bytes),
                Status = "图像将发送给模型"
            };
        }

        private static MissionAttachment LoadPdf(FileInfo file)
        {
            var text = new StringBuilder();
            try
            {
                using (PdfDocument document = PdfDocument.Open(file.FullName))
                {
                    foreach (var page in document.GetPages())
                    {
                        if (!string.IsNullOrWhiteSpace(page.Text))
                            text.AppendLine(page.Text.Trim());
                    }
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("无法读取 PDF（可能已加密或损坏）：" + file.Name + "。" + ex.Message, ex);
            }

            string extracted = NormalizeExtractedText(text.ToString());
            if (!string.IsNullOrWhiteSpace(extracted))
            {
                return new MissionAttachment
                {
                    DisplayName = file.Name,
                    MediaType = "application/pdf",
                    SizeBytes = file.Length,
                    Kind = AttachmentContentKind.ExtractedText,
                    ExtractedText = extracted,
                    Status = "已提取 PDF 文本"
                };
            }

            byte[] bytes = File.ReadAllBytes(file.FullName);
            return new MissionAttachment
            {
                DisplayName = file.Name,
                MediaType = "application/pdf",
                SizeBytes = file.Length,
                Kind = AttachmentContentKind.NativePdf,
                DataUrl = "data:application/pdf;base64," + Convert.ToBase64String(bytes),
                Status = "无文本 PDF，将以原生文件发送（Responses）"
            };
        }

        private static MissionAttachment LoadDocx(FileInfo file)
        {
            string extracted;
            try
            {
                using (ZipArchive archive = ZipFile.OpenRead(file.FullName))
                {
                    ZipArchiveEntry documentEntry = archive.GetEntry("word/document.xml");
                    if (documentEntry == null)
                        throw new InvalidDataException("DOCX 中缺少 word/document.xml。");
                    using (Stream stream = documentEntry.Open())
                    {
                        XDocument document = XDocument.Load(stream);
                        XNamespace word = "http://schemas.openxmlformats.org/wordprocessingml/2006/main";
                        var paragraphs = document.Descendants(word + "p")
                            .Select(paragraph => string.Concat(paragraph.Descendants(word + "t").Select(node => node.Value)))
                            .Where(value => !string.IsNullOrWhiteSpace(value));
                        extracted = NormalizeExtractedText(string.Join(Environment.NewLine, paragraphs));
                    }
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("无法读取 DOCX（可能已加密或损坏）：" + file.Name + "。" + ex.Message, ex);
            }

            if (string.IsNullOrWhiteSpace(extracted))
                throw new InvalidOperationException("DOCX 中没有可读取的正文文字：" + file.Name);
            return new MissionAttachment
            {
                DisplayName = file.Name,
                MediaType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                SizeBytes = file.Length,
                Kind = AttachmentContentKind.ExtractedText,
                ExtractedText = extracted,
                Status = "已提取 DOCX 正文与表格文本"
            };
        }

        private static MissionAttachment LoadText(FileInfo file, string mediaType)
        {
            byte[] bytes = File.ReadAllBytes(file.FullName);
            if (bytes.Any(value => value == 0) && !HasUtf16Bom(bytes))
                throw new InvalidOperationException("文件看起来是二进制内容，无法作为文本读取：" + file.Name);

            string extracted;
            using (var reader = new StreamReader(file.FullName, Encoding.UTF8, true))
                extracted = NormalizeExtractedText(reader.ReadToEnd());
            if (string.IsNullOrWhiteSpace(extracted))
                throw new InvalidOperationException("文本文件没有可读取内容：" + file.Name);

            return new MissionAttachment
            {
                DisplayName = file.Name,
                MediaType = mediaType,
                SizeBytes = file.Length,
                Kind = AttachmentContentKind.ExtractedText,
                ExtractedText = extracted,
                Status = "已读取文本"
            };
        }

        private static string NormalizeExtractedText(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return string.Empty;
            return text.Replace("\0", string.Empty).Trim();
        }

        private static bool HasUtf16Bom(byte[] bytes)
        {
            return bytes.Length >= 2 && ((bytes[0] == 0xff && bytes[1] == 0xfe) || (bytes[0] == 0xfe && bytes[1] == 0xff));
        }

        private static string GetTextMediaType(string extension)
        {
            if (extension == ".json") return "application/json";
            if (extension == ".xml" || extension == ".kml" || extension == ".gpx") return "application/xml";
            if (extension == ".csv") return "text/csv";
            if (extension == ".html" || extension == ".htm") return "text/html";
            return "text/plain";
        }
    }
}
