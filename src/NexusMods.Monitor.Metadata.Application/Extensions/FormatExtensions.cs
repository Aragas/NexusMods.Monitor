using AngleSharp;

using NexusMods.Monitor.Metadata.Application.Formatters;

namespace NexusMods.Monitor.Metadata.Application.Extensions
{
    internal static class FormatExtensions
    {
        public static string ToText(this IMarkupFormattable markup) => markup.ToHtml(new TextFormatter()).Trim(' ').Trim('\t').Trim('\n');
    }
}