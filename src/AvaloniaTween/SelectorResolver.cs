using Avalonia;
using Avalonia.Controls;
using Avalonia.VisualTree;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AvaloniaTweener
{
    public static class SelectorResolver
    {
        public static IEnumerable<Visual> Resolve(string selector, Visual root)
        {
            if (string.IsNullOrWhiteSpace(selector))
                return Enumerable.Empty<Visual>();

            // Handle "Self" selector - return the root itself
            if (selector.Equals("Self", StringComparison.OrdinalIgnoreCase))
            {
                return new[] { root };
            }

            if (selector.StartsWith("#"))
            {
                var name = selector.Substring(1);
                return root.GetVisualDescendants().Where(v => (v as Control)?.Name == name);
            }

            if (selector.StartsWith("."))
            {
                var className = selector.Substring(1);
                return root.GetVisualDescendants().Where(v => v.Classes.Contains(className));
            }

            return root.GetVisualDescendants().Where(v => v.GetType().Name == selector);
        }
    }
}