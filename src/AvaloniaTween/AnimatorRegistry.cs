using System;
using System.Collections.Generic;

namespace AvaloniaAnimate
{
    public static class AnimatorRegistry
    {
        private static readonly Dictionary<string, Action<SelectorAnimationBuilder>> _animations = new();

        public static void Register(string name, Action<SelectorAnimationBuilder> configure)
        {
            _animations[name] = configure;
        }

        public static void Unregister(string name)
        {
            _animations.Remove(name);
        }

        public static bool TryGet(string name, out Action<SelectorAnimationBuilder>? configure)
        {
            return _animations.TryGetValue(name, out configure);
        }

        public static void Clear()
        {
            _animations.Clear();
        }
    }
}
