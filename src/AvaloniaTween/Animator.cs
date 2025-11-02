using Avalonia;

namespace AvaloniaAnimate
{
    public static class Animator
    {
        public static SelectorAnimationBuilder Select(string selector, Visual root)
        {
            var targets = SelectorResolver.Resolve(selector, root);
            return new SelectorAnimationBuilder(targets);
        }
    }
}