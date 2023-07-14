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
            builder.Append("<div class=\"feature\">");
            builder.Append($"<div class=\"header\" style=\"background-color: {featureProperties.FillColor}\">");
            builder.Append($"<div class=\"category\">{featureProperties.DisplayInfo.Category}</div> ");
            builder.Append($"<div class=\"detailedCategory\">({featureProperties.DisplayInfo.DetailedCategory})</div>");
            builder.Append($"<div class=\"title\">{featureProperties.DisplayInfo.Title}</div>");
            builder.Append("</div>");
            if (featureProperties.UtmStatus?.UtmDetails != null && featureProperties.UtmStatus.Enabled)
            {
                builder.Append("<div class=\"utmStatus\">");
                builder.Append("<div class=\"section\">");
                builder.Append("<div class=\"title\">FACILITY IS UTM READY</div>");
                builder.Append("</div>");

                if (!string.IsNullOrWhiteSpace(featureProperties.UtmStatus.Title))
                {
                    builder.Append($"<div class=\"title\">{markdown.Transform(featureProperties.UtmStatus.Title)}</div>");
                }

                if (!string.IsNullOrWhiteSpace(featureProperties.UtmStatus.Description))
                {
                    builder.Append($"<div class=\"text\">{markdown.Transform(featureProperties.UtmStatus.Description)}</div>");
                }

                // TODO: Rate cards

                if (!string.IsNullOrWhiteSpace(featureProperties.UtmStatus.UtmDetails.ExternalUrl))
                {
                    builder.Append($"<div class=\"button\"><a href=\"{featureProperties.UtmStatus.UtmDetails.ExternalUrl}\">Request To Fly Here</a></div>");
                }
                builder.Append("</div>");
            }
            if (featureProperties.Contact?.PhoneNumbers != null)
            {
                builder.Append("<div class=\"contact\">");
                if (featureProperties.UtmStatus?.UtmDetails != null && featureProperties.UtmStatus.Enabled)
                {
                    builder.Append("<div class=\"section\">");
                    builder.Append("<div class=\"title\">Contacts</div>");
                }
                else
                {
                    builder.Append("<div class=\"section\">");
                    builder.Append("<div class=\"title\">FACILITY IS NOT UTM READY</div>");
                    builder.Append("<div class=\"text\"><p>We can't submit a digital flight request to this facility as it isn't connected to Altitude Angel or has no compatible UTM service in operation. It may be possible to fly here, but you will have to contact the facility operator by phone to find out whether you can and what process to follow. According to our records, you can contact them on the number(s) below:</p></div>");
                }

                foreach (var phoneNumber in featureProperties.Contact.PhoneNumbers)
                {
                    builder.Append("<div class=\"phoneNumber\">");
                    builder.Append($"<span class=\"location\">{phoneNumber.Location}</span>");
                    builder.Append($"<span class=\"number\"><a href=\"tel:{phoneNumber.Number}\">{phoneNumber.Number}</a></span>");
                    builder.Append("</div>");
                }
                builder.Append("</div>");
            }
            foreach (var section in featureProperties.DisplayInfo.Sections)
            {
                builder.Append("<div class=\"section\">");
                if (!string.IsNullOrWhiteSpace(section.Title))
                {
                    builder.Append($"<div class=\"title\">{section.Title}</div>");
                }

                if (!string.IsNullOrWhiteSpace(section.DisplayTitle))
                {
                    builder.Append($"<div class=\"displayTitle\">{section.DisplayTitle}</div>");
                }

                if (!string.IsNullOrWhiteSpace(section.Text))
                {
                    builder.Append($"<div class=\"text\">{markdown.Transform(section.Text)}</div>");
                }

                if (!string.IsNullOrWhiteSpace(section.Disclaimer))
                {
                    builder.Append($"<div class=\"disclaimer\">{markdown.Transform(section.Disclaimer)}</div>");
                }
                builder.Append("</div>");
            }
            builder.Append("</div>");
            return builder.ToString();
        }
    }
}