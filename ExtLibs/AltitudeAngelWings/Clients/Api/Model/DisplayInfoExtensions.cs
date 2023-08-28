using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Markdig;

namespace AltitudeAngelWings.Clients.Api.Model
{
    public static class DisplayInfoExtensions
    {
        private static readonly IDictionary<string, CultureInfo> CurrencyLookup = CultureInfo.GetCultures(CultureTypes.AllCultures)
            .Where(c => !c.IsNeutralCulture)
            .Select(c =>
            {
                try
                {
                    return (c, new RegionInfo(c.Name) );
                }
                catch (Exception)
                {
                    return (c, null);
                }
            })
            .Where(r => r.Item2 != null)
            .GroupBy(r => r.Item2.ISOCurrencySymbol)
            .ToDictionary(g => g.Key, g => g.First().c);

        public static string FormatAsHtml(this FeatureProperties featureProperties,
            IDictionary<string, RateCardDetail> rateCardDetails)
        {
            var settings = ServiceLocator.GetService<ISettings>();
            var pipeline = ServiceLocator.GetService<MarkdownPipeline>();
            var builder = new StringBuilder();
            builder.Append("<div class=\"feature\">");
            builder.Append($"<div class=\"highlight\" style=\"background-color: {featureProperties.FillColor}\"></div>");
            builder.Append("<div class=\"header\">");
            builder.Append($"<div class=\"category\">{featureProperties.DisplayInfo.Category}</div>");
            builder.Append($"<div class=\"detailedCategory\"> : {featureProperties.DisplayInfo.DetailedCategory}</div>");
            builder.Append($"<div class=\"displayTitle\">{featureProperties.DisplayInfo.Title.ToUpper()}</div>");
            if (featureProperties.HasUtmStatus())
            {
                builder.Append($"<div class=\"utmBadge\"><img src=\"{settings.CdnUrl}/images/icons/");
                if (featureProperties.IsUtmReady())
                {
                    builder.Append("utm-ready.png");
                }
                else if (featureProperties.IsUtmBasic())
                {
                    builder.Append("utm-basic.png");
                }
                else
                {
                    builder.Append("utm-legacy.png");
                }
                builder.Append("\"/></div>");
            }
            builder.Append("</div>");

            if (featureProperties.HasUtmStatus())
            {
                builder.Append("<div class=\"section\">");
                builder.Append("<div class=\"displayTitle\">APPROVAL REQUIRED</div>");

                if (featureProperties.IsUtmReady())
                {
                    builder.Append("<div class=\"utmReadyTitle\">Online Approval</div>");

                }
                else if (featureProperties.IsUtmBasic())
                {
                    builder.Append("<div class=\"utmBasicTitle\">Notification Available</div>");
                }
                else
                {
                    builder.Append("<div class=\"utmLegacyTitle\">Not Connected</div>");
                }
                builder.Append("</div>");

                if (featureProperties.IsUtmEnabled())
                {
                    var isReady = featureProperties.IsUtmReady();
                    builder.Append($"<div class=\"{(isReady ? "utmReadyStatus" : "utmBasicStatus")}\">");
                    builder.Append("<div class=\"section\">");

                    if (!string.IsNullOrWhiteSpace(featureProperties.UtmStatus.Title))
                    {
                        builder.Append($"<div class=\"title\">{Markdown.ToHtml(featureProperties.UtmStatus.Title, pipeline)}</div>");
                    }

                    builder.Append($"<div class=\"displayTitle\">{(isReady ? "FACILITY IS UTM CONNECTED" : "NOTIFY FACILITY")}</div>");

                    if (!string.IsNullOrWhiteSpace(featureProperties.UtmStatus.Description))
                    {
                        builder.Append($"<div class=\"text\">{Markdown.ToHtml(featureProperties.UtmStatus.Description, pipeline)}</div>");
                    }
                    else if (!isReady)
                    {
                        builder.Append("<div class=\"text\"><p>This facility supports prior notification. Submitting your flight plan will begin the approval process with the facility.<ul><li>Prior Notification</li></ul></p></div>");
                    }

                    foreach (var rateType in featureProperties.UtmStatus.RateTypes.Keys)
                    {
                        var rateCard = featureProperties.UtmStatus.RateTypes[rateType]
                            .Where(c =>
                            {
                                if (c.AppliesFrom == null) return false;
                                var now = DateTimeOffset.UtcNow;
                                if (c.AppliesFrom > now) return false;
                                if (c.AppliesTo == null) return true;
                                return c.AppliesTo >= now;
                            })
                            .OrderBy(c => c.AppliesFrom)
                            .Select(c => rateCardDetails[c.Id])
                            .FirstOrDefault();
                        if (rateCard == null) continue;
                        builder.Append("<div class=\"rateType\">");
                        builder.Append("<div class=\"section\">");
                        builder.Append($"<div class=\"displayTitle\">{MapRateTypeToText(rateType)}</div>");
                        builder.Append("<div class=\"rateCard\">");
                        builder.Append($"<p>{featureProperties.DisplayInfo.Title.ToUpper()}</p>");
                        builder.Append($"<p>{Markdown.ToHtml(rateCard.ExplanatoryText, pipeline)}</p>");
                        builder.Append("<ul>");
                        foreach (var rate in rateCard.Rates.OrderBy(r => r.Ordinal))
                        {
                            builder.Append("<li>");
                            builder.Append(rate.Name);
                            builder.Append(" (");
                            var total = rate.Rate + rateCard.StandingCharge;
                            total += rateCard.TaxRate / 100 * total;
                            builder.Append(CurrencyLookup.TryGetValue(rateCard.Currency, out var value)
                                ? total.ToString("C", value)
                                : $"{total} {rateCard.Currency}");

                            builder.Append(")</li>");
                        }
                        builder.Append("</ul>");
                        builder.Append("<p>If you wish to operate here, depending on your flight plan, you may require their prior approval.</p>");
                        builder.Append($"<p><a href=\"{rateCard.RateCardTerms}\">Terms and conditions</a></p>");
                        builder.Append("</div>");
                        builder.Append("</div>");
                        builder.Append("</div>");
                    }

                    if (!string.IsNullOrWhiteSpace(featureProperties.UtmStatus.UtmDetails.ExternalUrl))
                    {
                        builder.Append($"<div class=\"button\"><a href=\"{featureProperties.UtmStatus.UtmDetails.ExternalUrl}\">{(isReady ? "Request To Fly Here" : "Notify Facility")}</a></div>");
                    }

                    builder.Append("</div>");
                    builder.Append("</div>");
                }
            }

            if (featureProperties.Contact?.PhoneNumbers != null && featureProperties.Contact.PhoneNumbers.Count > 0)
            {
                if (featureProperties.IsUtmEnabled())
                {
                    builder.Append("<div class=\"contact\">");
                    builder.Append("<div class=\"section\">");
                    builder.Append("<div class=\"displayTitle\">CONTACTS</div>");
                }
                else
                {
                    builder.Append("<div class=\"noUtm\">");
                    builder.Append("<div class=\"section\">");
                    builder.Append("<div class=\"displayTitle\">FACILITY IS NOT UTM READY</div>");
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
                builder.Append("</div>");
            }
            foreach (var section in featureProperties.DisplayInfo.Sections)
            {
                builder.Append("<div class=\"section\">");
                if (!string.IsNullOrWhiteSpace(section.Title))
                {
                    builder.Append($"<div class=\"title\">{section.Title.ToUpper()}</div>");
                }

                if (!string.IsNullOrWhiteSpace(section.DisplayTitle))
                {
                    builder.Append($"<div class=\"displayTitle\">{section.DisplayTitle.ToUpper()}</div>");
                }

                if (!string.IsNullOrWhiteSpace(section.Text))
                {
                    builder.Append($"<div class=\"text\">{Markdown.ToHtml(section.Text, pipeline)}</div>");
                }

                if (!string.IsNullOrWhiteSpace(section.Disclaimer))
                {
                    builder.Append($"<div class=\"disclaimer\">{Markdown.ToHtml(section.Disclaimer, pipeline)}</div>");
                }
                builder.Append("</div>");
            }
            builder.Append("</div>");
            return builder.ToString();
        }

        private static string MapRateTypeToText(string rateType)
        {
            switch (rateType)
            {
                case "flight-plan-approvals":
                    return "APPROVAL SERVICES";
                default:
                    return rateType;
            }
        }
    }
}