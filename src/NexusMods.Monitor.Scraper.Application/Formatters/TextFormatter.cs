using AngleSharp;
using AngleSharp.Dom;

namespace NexusMods.Monitor.Scraper.Application.Formatters
{
    public sealed class TextFormatter : IMarkupFormatter
    {
        public string Comment(IComment comment) => string.Empty;
        public string Doctype(IDocumentType doctype) => string.Empty;
        public string Processing(IProcessingInstruction processing) => string.Empty;
        public string Text(ICharacterData text) => text.Data;
        public string OpenTag(IElement element, bool selfClosing) => element.LocalName switch
        {
            "p" => "\n\n",
            "br" => "\n",
            "span" => " ",
            _ => string.Empty
        };
        public string CloseTag(IElement element, bool selfClosing) => string.Empty;
        public string LiteralText(ICharacterData text) => text.Data;
    }
}