using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading;
using System.Xml.Linq;
using UglyToad.PdfPig;

namespace MissionPlanner.AIWaypointPlanner
{
    public enum AttachmentContentKind
    {
        ExtractedText,
        Image,
        NativePdf,
        NativeDocument
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

        private static readonly Dictionary<string, string> NativeDocumentMediaTypes =
            new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { ".doc", "application/msword" },
                { ".docx", "application/vnd.openxmlformats-officedocument.wordprocessingml.document" },
                { ".rtf", "application/rtf" },
                { ".odt", "application/vnd.oasis.opendocument.text" },
                { ".ppt", "application/vnd.ms-powerpoint" },
                { ".pptx", "application/vnd.openxmlformats-officedocument.presentationml.presentation" },
                { ".xls", "application/vnd.ms-excel" },
                { ".xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" }
            };

        public MissionAttachment Load(string path, IEnumerable<MissionAttachment> existingAttachments)
        {
            return Load(path, existingAttachments, UiStrings.DefaultLanguageCode, CancellationToken.None);
        }

        public MissionAttachment Load(
            string path,
            IEnumerable<MissionAttachment> existingAttachments,
            string languageCode)
        {
            return Load(path, existingAttachments, languageCode, CancellationToken.None);
        }

        public MissionAttachment Load(
            string path,
            IEnumerable<MissionAttachment> existingAttachments,
            string languageCode,
            CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentException(UiStrings.Get(languageCode, "Attachment.ErrorPathRequired"), "path");

            var file = new FileInfo(path);
            if (!file.Exists)
                throw new FileNotFoundException(UiStrings.Get(languageCode, "Attachment.ErrorNotFound"), path);
            if (file.Length <= 0)
                throw new InvalidOperationException(UiStrings.Format(
                    languageCode, "Attachment.ErrorEmptyFileFormat", file.Name));
            if (file.Length > MaximumFileBytes)
                throw new InvalidOperationException(UiStrings.Format(
                    languageCode, "Attachment.ErrorFileTooLargeFormat", file.Name));

            List<MissionAttachment> existing = (existingAttachments ?? Enumerable.Empty<MissionAttachment>()).ToList();
            if (existing.Count >= MaximumAttachmentCount)
                throw new InvalidOperationException(UiStrings.Format(
                    languageCode, "Attachment.ErrorTooManyFilesFormat", MaximumAttachmentCount));
            if (existing.Any(item => item != null &&
                string.Equals(item.DisplayName, file.Name, StringComparison.OrdinalIgnoreCase)))
            {
                throw new InvalidOperationException(UiStrings.Get(
                    languageCode, "Attachment.DuplicateName"));
            }
            if (existing.Sum(item => item.SizeBytes) + file.Length > MaximumTotalBytes)
                throw new InvalidOperationException(UiStrings.Get(languageCode, "Attachment.ErrorTotalSize"));

            string extension = file.Extension.ToLowerInvariant();
            MissionAttachment attachment;
            if (ImageMediaTypes.ContainsKey(extension))
                attachment = LoadImage(file, ImageMediaTypes[extension], languageCode, cancellationToken);
            else if (extension == ".pdf")
                attachment = LoadPdf(file, languageCode, cancellationToken);
            else if (extension == ".docx")
                attachment = LoadDocx(file, languageCode, cancellationToken);
            else if (NativeDocumentMediaTypes.ContainsKey(extension))
                attachment = LoadNativeDocument(
                    file, NativeDocumentMediaTypes[extension], languageCode, cancellationToken);
            else if (TextExtensions.Contains(extension))
                attachment = LoadText(file, GetTextMediaType(extension), languageCode, cancellationToken);
            else
                throw new InvalidOperationException(UiStrings.Format(
                    languageCode, "Attachment.ErrorUnsupportedTypeFormat", extension));

            cancellationToken.ThrowIfCancellationRequested();
            int totalPromptCharacters = existing
                .Where(item => item != null && item.Kind == AttachmentContentKind.ExtractedText)
                .Sum(item => item.ExtractedText == null ? 0 : item.ExtractedText.Length) +
                (attachment.Kind == AttachmentContentKind.ExtractedText
                    ? attachment.ExtractedText.Length
                    : 0);
            if (totalPromptCharacters > MaximumExtractedCharacters)
                throw new InvalidOperationException(UiStrings.Get(
                    languageCode, "Attachment.ErrorExtractedTextTotal"));
            return attachment;
        }

        public static void ValidateForProtocol(IEnumerable<MissionAttachment> attachments, ApiProtocol protocol)
        {
            ValidateForProtocol(attachments, protocol, UiStrings.DefaultLanguageCode);
        }

        public static void ValidateForProtocol(
            IEnumerable<MissionAttachment> attachments,
            ApiProtocol protocol,
            string languageCode)
        {
            List<MissionAttachment> files = (attachments ?? Enumerable.Empty<MissionAttachment>()).ToList();
            if (files.Count > MaximumAttachmentCount)
                throw new InvalidOperationException(UiStrings.Get(languageCode, "Attachment.ErrorCountLimit"));
            if (files.Sum(item => item.SizeBytes) > MaximumTotalBytes)
                throw new InvalidOperationException(UiStrings.Get(languageCode, "Attachment.ErrorTotalLimit"));
            if (protocol == ApiProtocol.ChatCompletions &&
                files.Sum(item => item.ExtractedText == null ? 0 : item.ExtractedText.Length) >
                MaximumExtractedCharacters)
                throw new InvalidOperationException(UiStrings.Get(languageCode, "Attachment.ErrorTextLimit"));
            if (protocol == ApiProtocol.ChatCompletions && files.Any(item =>
                (item.Kind == AttachmentContentKind.NativePdf || item.Kind == AttachmentContentKind.NativeDocument) &&
                string.IsNullOrWhiteSpace(item.ExtractedText)))
            {
                throw new InvalidOperationException(UiStrings.Get(
                    languageCode, "Attachment.ErrorChatNativeUnsupported"));
            }
        }

        private static MissionAttachment LoadImage(
            FileInfo file,
            string mediaType,
            string languageCode,
            CancellationToken cancellationToken)
        {
            if (file.Length > MaximumImageBytes)
                throw new InvalidOperationException(UiStrings.Format(
                    languageCode, "Attachment.ErrorImageTooLargeFormat", file.Name));
            byte[] bytes = ReadAllBytes(file, languageCode, cancellationToken);
            return new MissionAttachment
            {
                DisplayName = file.Name,
                MediaType = mediaType,
                SizeBytes = file.Length,
                Kind = AttachmentContentKind.Image,
                DataUrl = "data:" + mediaType + ";base64," + Convert.ToBase64String(bytes),
                Status = UiStrings.Get(languageCode, "Attachment.ImageWillSend")
            };
        }

        private static MissionAttachment LoadPdf(
            FileInfo file,
            string languageCode,
            CancellationToken cancellationToken)
        {
            var text = new StringBuilder();
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                using (PdfDocument document = PdfDocument.Open(file.FullName))
                {
                    foreach (var page in document.GetPages())
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        if (!string.IsNullOrWhiteSpace(page.Text))
                            text.AppendLine(page.Text.Trim());
                    }
                }
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(UiStrings.Format(
                    languageCode, "Attachment.ErrorPdfReadFormat", file.Name, ex.Message), ex);
            }

            string extracted = NormalizeExtractedText(text.ToString());
            byte[] bytes = ReadAllBytes(file, languageCode, cancellationToken);
            return new MissionAttachment
            {
                DisplayName = file.Name,
                MediaType = "application/pdf",
                SizeBytes = file.Length,
                Kind = AttachmentContentKind.NativePdf,
                ExtractedText = extracted,
                DataUrl = "data:application/pdf;base64," + Convert.ToBase64String(bytes),
                Status = string.IsNullOrWhiteSpace(extracted)
                    ? UiStrings.Get(languageCode, "Attachment.PdfNative")
                    : UiStrings.Get(languageCode, "Attachment.PdfText")
            };
        }

        private static MissionAttachment LoadDocx(
            FileInfo file,
            string languageCode,
            CancellationToken cancellationToken)
        {
            string extracted;
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                using (ZipArchive archive = ZipFile.OpenRead(file.FullName))
                {
                    ZipArchiveEntry documentEntry = archive.GetEntry("word/document.xml");
                    if (documentEntry == null)
                        throw new InvalidDataException(UiStrings.Get(
                            languageCode, "Attachment.ErrorDocxXmlMissing"));
                    using (Stream stream = documentEntry.Open())
                    {
                        XDocument document = XDocument.Load(stream);
                        cancellationToken.ThrowIfCancellationRequested();
                        XNamespace word = "http://schemas.openxmlformats.org/wordprocessingml/2006/main";
                        var paragraphs = document.Descendants(word + "p")
                            .Select(paragraph => string.Concat(paragraph.Descendants(word + "t").Select(node => node.Value)))
                            .Where(value => !string.IsNullOrWhiteSpace(value));
                        extracted = NormalizeExtractedText(string.Join(Environment.NewLine, paragraphs));
                    }
                }
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(UiStrings.Format(
                    languageCode, "Attachment.ErrorDocxReadFormat", file.Name, ex.Message), ex);
            }

            if (string.IsNullOrWhiteSpace(extracted))
                throw new InvalidOperationException(UiStrings.Format(
                    languageCode, "Attachment.ErrorDocxEmptyFormat", file.Name));
            return new MissionAttachment
            {
                DisplayName = file.Name,
                MediaType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                SizeBytes = file.Length,
                Kind = AttachmentContentKind.NativeDocument,
                ExtractedText = extracted,
                DataUrl = "data:application/vnd.openxmlformats-officedocument.wordprocessingml.document;base64," +
                          Convert.ToBase64String(ReadAllBytes(file, languageCode, cancellationToken)),
                Status = UiStrings.Get(languageCode, "Attachment.DocxText")
            };
        }

        private static MissionAttachment LoadNativeDocument(
            FileInfo file,
            string mediaType,
            string languageCode,
            CancellationToken cancellationToken)
        {
            byte[] bytes = ReadAllBytes(file, languageCode, cancellationToken);
            return new MissionAttachment
            {
                DisplayName = file.Name,
                MediaType = mediaType,
                SizeBytes = file.Length,
                Kind = AttachmentContentKind.NativeDocument,
                DataUrl = "data:" + mediaType + ";base64," + Convert.ToBase64String(bytes),
                Status = UiStrings.Get(languageCode, "Attachment.DocumentNative")
            };
        }

        private static MissionAttachment LoadText(
            FileInfo file,
            string mediaType,
            string languageCode,
            CancellationToken cancellationToken)
        {
            byte[] bytes = ReadAllBytes(file, languageCode, cancellationToken);
            if (bytes.Any(value => value == 0) && !HasUtf16Bom(bytes))
                throw new InvalidOperationException(UiStrings.Format(
                    languageCode, "Attachment.ErrorBinaryTextFormat", file.Name));

            string extracted;
            using (var reader = new StreamReader(new MemoryStream(bytes), Encoding.UTF8, true))
                extracted = NormalizeExtractedText(reader.ReadToEnd());
            cancellationToken.ThrowIfCancellationRequested();
            if (string.IsNullOrWhiteSpace(extracted))
                throw new InvalidOperationException(UiStrings.Format(
                    languageCode, "Attachment.ErrorTextEmptyFormat", file.Name));

            return new MissionAttachment
            {
                DisplayName = file.Name,
                MediaType = mediaType,
                SizeBytes = file.Length,
                Kind = AttachmentContentKind.ExtractedText,
                ExtractedText = extracted,
                Status = UiStrings.Get(languageCode, "Attachment.TextRead")
            };
        }

        private static byte[] ReadAllBytes(
            FileInfo file,
            string languageCode,
            CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (file.Length > int.MaxValue)
                throw new IOException(UiStrings.Format(
                    languageCode, "Attachment.ErrorFileTooLargeFormat", file.Name));

            byte[] bytes = new byte[(int)file.Length];
            int offset = 0;
            using (var stream = new FileStream(
                file.FullName, FileMode.Open, FileAccess.Read, FileShare.Read, 81920,
                FileOptions.SequentialScan))
            {
                while (offset < bytes.Length)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    int read = stream.Read(bytes, offset, Math.Min(81920, bytes.Length - offset));
                    if (read == 0)
                        throw new EndOfStreamException(UiStrings.Get(
                            languageCode, "Attachment.ErrorChangedWhileReading"));
                    offset += read;
                }
            }
            cancellationToken.ThrowIfCancellationRequested();
            return bytes;
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
