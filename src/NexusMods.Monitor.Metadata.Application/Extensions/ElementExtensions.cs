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

        private static bool IsHiddenInternal(IElement element) =>
            element.GetAttribute(AttributeNames.Hidden) is { } hidden && hidden.Equals("true", StringComparison.OrdinalIgnoreCase) ||
            element.GetAttribute(AttributeNames.Style) is { } style && style.Equals("display:none", StringComparison.OrdinalIgnoreCase);

        public static bool IsHidden(this IElement element) => IsHiddenInternal(element) || IsParentHidden(element);
    }
}