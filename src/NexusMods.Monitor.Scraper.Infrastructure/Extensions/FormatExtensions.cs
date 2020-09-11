using AngleSharp;

using NexusMods.Monitor.Scraper.Infrastructure.Formatters;

namespace NexusMods.Monitor.Scraper.Infrastructure.Extensions
{
    public static class FormatExtensions
    {
        public static string ToText(this IMarkupFormattable markup) => markup.ToHtml(new TextFormatter()).Trim(' ').Trim('\t').Trim('\n');
    }
}