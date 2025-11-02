# AvaloniaTween

This is a library which has the goal to make it easier to define animations in code-behind and in markup for Avalonia UI applications.

It is inspired by the GSAP javascript animation library.

## 🎯 Goal
Design a fluent, composable animation API for Avalonia that mimics the ergonomics of GSAP (GreenSock Animation Platform)..

---

## ✅ Key Concepts Discussed

### TweenBuilder Prototype
- Fluent API for chaining animations:
  ```csharp
Animator.Select(".fadeable")
    .Animate(Visual.OpacityProperty)
        .From(0.0).To(1.0, TimeSpan.FromSeconds(1)).Ease(Easing.OutQuad)
    .Animate(Canvas.LeftProperty)
        .From(100.0).To(200.0, TimeSpan.FromSeconds(0.5))
    .StartAsync();
  ```
- Supports properties like Opacity, TranslateTransform.X, etc.
- Wraps Avalonia’s Animation and KeyFrame system

#### Selector-Based Targeting
* Resolve targets from the visual tree using selectors (#foo, .bar, Button, etc.)
* Enable batch animations

### Markup DSL Concept
- Proposed syntax:
```
<Button Anim:Tween="Opacity:0→1@1s; X:0→100@500ms"/>
```
- Use attached properties or markup extensions

