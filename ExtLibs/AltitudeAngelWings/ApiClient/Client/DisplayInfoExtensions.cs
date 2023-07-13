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
            if (featureProperties.UtmStatus != null && featureProperties.UtmStatus.UtmDetails != null && featureProperties.UtmStatus.Enabled == true)
            {
                builder.Append("<div class=\"utmStatus\">");
                builder.Append($"<div class=\"title\">{markdown.Transform(featureProperties.UtmStatus.Title)}</div>");
                builder.Append($"<div class=\"text\">{markdown.Transform(featureProperties.UtmStatus.Description)}</div>");
                // TODO: Rate cards
                builder.Append($"<div class=\"button\"><a href=\"{featureProperties.UtmStatus.UtmDetails.ExternalUrl}\">Request To Fly Here</a></div>");
                builder.Append("</div>");
            }
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