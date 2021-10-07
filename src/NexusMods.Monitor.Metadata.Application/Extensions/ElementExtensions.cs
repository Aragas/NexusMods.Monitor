using AngleSharp.Dom;

using System;

namespace NexusMods.Monitor.Metadata.Application.Extensions
{
    public static class ElementExtensions
    {
        private static bool IsParentHidden(IElement element)
        {
            if (element.ParentElement is null)
                return IsHiddenInternal(element);

            return IsParentHidden(element.ParentElement);
        }

        private static bool IsHiddenInternal(IElement element)
        {
            var t1 = element.GetAttribute(AttributeNames.Hidden) is { } hidden && hidden.Equals("true", StringComparison.OrdinalIgnoreCase);
            var t2 = element.GetAttribute(AttributeNames.Style) is { } style && style.Contains("display:none", StringComparison.OrdinalIgnoreCase);

            return t1 || t2;
        }

        public static bool IsHidden(this IElement element)
        {
            var t1 = IsHiddenInternal(element);
            var t2 = IsParentHidden(element);
            return t1 || t2;
        }
    }
}