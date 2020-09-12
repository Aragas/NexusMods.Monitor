using AngleSharp;

using NexusMods.Monitor.Scraper.Application.Formatters;

namespace NexusMods.Monitor.Scraper.Application.Extensions
{
    public static class FormatExtensions
    {
        public static string ToText(this IMarkupFormattable markup) => markup.ToHtml(new TextFormatter()).Trim(' ').Trim('\t').Trim('\n');
    }
}