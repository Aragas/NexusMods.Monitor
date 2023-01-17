using AngleSharp.Dom;

using System;

namespace NexusMods.Monitor.Metadata.Application.Extensions
{
    public static class ElementExtensions
    {
        private static bool IsParentHidden(IElement element)
        {
            while (true)
            {
                if (element.ParentElement is null) return IsHiddenInternal(element);

                element = element.ParentElement;
            }
        }

        private static bool IsHiddenInternal(IElement element)
        {
            return element.GetAttribute(AttributeNames.Hidden) is { } hidden && hidden.Equals("true", StringComparison.OrdinalIgnoreCase) ||
                   element.GetAttribute(AttributeNames.Style) is { } style && style.Contains("display:none", StringComparison.OrdinalIgnoreCase);
        }


        public static bool IsHidden(this IElement element)
        {
            return IsHiddenInternal(element) || IsParentHidden(element);
        }
    }
}