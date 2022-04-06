using System.Text;
using MarkdownSharp;

namespace AltitudeAngelWings.ApiClient.Client
{
    public static class DisplayInfoExtensions
    {
        public static string FormatAsHtml(this FeatureProperties featureProperties)
        {
            var builder = new StringBuilder();
            var markdown = new Markdown(new MarkdownOptions
            {
                AutoNewlines = false,
                AutoHyperlink = false,
                LinkEmails = false,
                EmptyElementSuffix = "/>"
            });
            builder.Append($"<div class=\"displayInfo\" style=\"background-color: {featureProperties.FillColor}\">");
            builder.Append($"<div class=\"title\">{featureProperties.DisplayInfo.Title}</div>");
            builder.Append($"<div class=\"category\">{featureProperties.DisplayInfo.Category}</div>");
            builder.Append($"<div class=\"detailedCategory\">{featureProperties.DisplayInfo.DetailedCategory}</div>");
            foreach (var section in featureProperties.DisplayInfo.Sections)
            {
                builder.Append("<div class=\"section\">");
                builder.Append($"<div class=\"title\">{section.Title}</div>");
                builder.Append($"<div class=\"displayTitle\">{section.DisplayTitle}</div>");
                builder.Append($"<div class\"text\">{markdown.Transform(section.Text)}</div>");
                builder.Append($"<div class\"disclaimer\">{markdown.Transform(section.Disclaimer)}</div>");
                builder.Append("</div>");
            }
            builder.Append("</div>");
            return builder.ToString();
        }
    }
}