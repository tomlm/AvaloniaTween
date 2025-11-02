# Markup Controls for AvaloniaTween

This document describes how to define tweener animations as static resources using the new markup controls.

## Overview

Instead of using string-based syntax, you can now define animations declaratively in XAML using a structured markup syntax with the following elements:

- **`<markup:Animation>`** - The root container for an animation definition
- **`<markup:Transform>`** - Represents a property track with one or more animation steps
- **`<markup:Animate>`** - Represents individual animation keyframes

## Getting Started

Add the XML namespace to your XAML file:

```xml
xmlns:markup="using:AvaloniaTweener.Markup"
xmlns:tweener="using:AvaloniaTweener"
```

## Basic Structure

```xml
<Window.Resources>
    <markup:Animation x:Key="MyAnimation">
        <markup:Transform PropertyName="TranslateTransform.Y" Reset="True">
            <markup:Animate To="-100" Duration="0:0:0.7">
                <markup:Animate.Easing>
                    <QuadraticEaseOut/>
                </markup:Animate.Easing>
            </markup:Animate>
            <markup:Animate To="0" Duration="0:0:0.7">
                <markup:Animate.Easing>
                    <BounceEaseOut/>
                </markup:Animate.Easing>
            </markup:Animate>
        </markup:Transform>
    </markup:Animation>
</Window.Resources>
```

## Using Animations

### On Click

```xml
<Button Content="Click Me!" 
        tweener:Tweener.OnClickAnimation="{StaticResource MyAnimation}">
    <Button.RenderTransform>
        <TranslateTransform/>
    </Button.RenderTransform>
</Button>
```

### On Load

```xml
<Border tweener:Tweener.OnLoadAnimation="{StaticResource MyAnimation}">
    <Border.RenderTransform>
        <TranslateTransform/>
    </Border.RenderTransform>
    <TextBlock Text="Animates on load!"/>
</Border>
```

## Examples

### 1. Bounce Animation

```xml
<markup:Animation x:Key="BounceAnimation">
    <markup:Transform PropertyName="TranslateTransform.Y" Reset="True">
        <markup:Animate To="-100" Duration="0:0:0.7">
            <markup:Animate.Easing>
                <QuadraticEaseOut/>
            </markup:Animate.Easing>
        </markup:Animate>
        <markup:Animate To="0" Duration="0:0:0.7">
            <markup:Animate.Easing>
                <BounceEaseOut/>
            </markup:Animate.Easing>
        </markup:Animate>
    </markup:Transform>
</markup:Animation>
```

### 2. Color Transition

```xml
<markup:Animation x:Key="ColorAnimation">
    <markup:Transform PropertyName="Button.Background" Delay="0:0:0.1">
        <markup:Animate To="Purple" Duration="0:0:0.5" />
        <markup:Animate To="Orange" Duration="0:0:0.5" />
        <markup:Animate To="Teal" Duration="0:0:0.5" />
    </markup:Transform>
</markup:Animation>
```

### 3. Multi-Property Animation (Scale + Rotate)

```xml
<markup:Animation x:Key="ScaleRotateAnimation">
    <markup:Transform PropertyName="ScaleTransform.ScaleX" Reset="True">
        <markup:Animate To="2.0" Duration="0:0:1">
            <markup:Animate.Easing>
                <BackEaseOut/>
            </markup:Animate.Easing>
        </markup:Animate>
        <markup:Animate To="1.0" Duration="0:0:1">
            <markup:Animate.Easing>
                <ElasticEaseOut/>
            </markup:Animate.Easing>
        </markup:Animate>
    </markup:Transform>
    <markup:Transform PropertyName="RotateTransform.Angle" Reset="True">
        <markup:Animate To="360" Duration="0:0:1">
            <markup:Animate.Easing>
                <CubicEaseInOut/>
            </markup:Animate.Easing>
        </markup:Animate>
    </markup:Transform>
</markup:Animation>
```

### 4. Slide Animation

```xml
<markup:Animation x:Key="SlideAnimation">
    <markup:Transform PropertyName="Button.Margin" Reset="True">
        <markup:Animate To="100,0,0,0" Duration="0:0:1">
            <markup:Animate.Easing>
                <BackEaseOut/>
            </markup:Animate.Easing>
        </markup:Animate>
    </markup:Transform>
</markup:Animation>
```

## `<markup:Transform>` Properties

| Property | Type | Description |
|----------|------|-------------|
| `PropertyName` | `string` | The property to animate (e.g., "TranslateTransform.Y", "Button.Background") |
| `Property` | `AvaloniaProperty` | Alternative to PropertyName - directly specify the AvaloniaProperty |
| `Reset` | `bool` | Whether to reset the property to its original value after animation completes (default: false) |
| `Delay` | `TimeSpan?` | Global delay before this transform starts |

### Supported Property Names

Common property name formats:
- Transform properties: `"TranslateTransform.X"`, `"TranslateTransform.Y"`, `"ScaleTransform.ScaleX"`, `"ScaleTransform.ScaleY"`, `"RotateTransform.Angle"`
- Control properties: `"Button.Background"`, `"Button.Foreground"`, `"Button.Margin"`, `"Visual.Opacity"`
- Layout properties: `"Canvas.Left"`, `"Canvas.Top"`, `"Layoutable.Width"`, `"Layoutable.Height"`

You can also use the full format: `"TypeName.PropertyNameProperty"` (e.g., `"TranslateTransform.YProperty"`)

## `<markup:Animate>` Properties

| Property | Type | Description |
|----------|------|-------------|
| `From` | `object` | Starting value (optional, will use current value if not specified) |
| `To` | `object` | Target value |
| `Duration` | `TimeSpan?` | Duration of this animation step (format: `0:0:1` for 1 second) |
| `Delay` | `TimeSpan?` | Delay before this step starts |
| `Easing` | `Easing` | Easing function for this step |

### TimeSpan Format

Use standard .NET TimeSpan format: `hours:minutes:seconds.milliseconds`

Examples:
- `0:0:1` - 1 second
- `0:0:0.5` - 0.5 seconds (500ms)
- `0:0:0.7` - 0.7 seconds (700ms)
- `0:0:2.5` - 2.5 seconds

### Available Easing Functions

All standard Avalonia easing functions are supported:
- `<LinearEasing/>`
- `<QuadraticEaseIn/>`, `<QuadraticEaseOut/>`, `<QuadraticEaseInOut/>`
- `<CubicEaseIn/>`, `<CubicEaseOut/>`, `<CubicEaseInOut/>`
- `<BackEaseIn/>`, `<BackEaseOut/>`, `<BackEaseInOut/>`
- `<BounceEaseIn/>`, `<BounceEaseOut/>`, `<BounceEaseInOut/>`
- `<ElasticEaseIn/>`, `<ElasticEaseOut/>`, `<ElasticEaseInOut/>`
- `<CircularEaseIn/>`, `<CircularEaseOut/>`, `<CircularEaseInOut/>`
- `<ExponentialEaseIn/>`, `<ExponentialEaseOut/>`, `<ExponentialEaseInOut/>`
- `<SineEaseIn/>`, `<SineEaseOut/>`, `<SineEaseInOut/>`

## Complete Example

See `MarkupExamples.axaml` in the Demo project for a complete working example with multiple animation types.

## Benefits

? **Type-safe** - Properties and values are validated at compile-time  
? **IntelliSense** - Full IDE support with auto-completion  
? **Reusable** - Define once as a resource, use anywhere  
? **Readable** - Clear, structured syntax vs. string parsing  
? **Powerful** - Support for multiple properties, complex animations, and all easing functions  

## Comparison with Code-Behind

### Markup Syntax (Static Resource)

```xml
<markup:Animation x:Key="foo">
    <markup:Transform PropertyName="TranslateTransform.Y" Reset="True">
        <markup:Animate To="-100" Duration="0:0:0.7">
            <markup:Animate.Easing>
                <QuadraticEaseOut/>
            </markup:Animate.Easing>
        </markup:Animate>
        <markup:Animate To="0" Duration="0:0:0.7">
            <markup:Animate.Easing>
                <BounceEaseOut/>
            </markup:Animate.Easing>
        </markup:Animate>
    </markup:Transform>
</markup:Animation>
```

### Equivalent Code-Behind

```csharp
await Tweener.Select(button)
    .Animate(TranslateTransform.YProperty)
        .To(-100.0, TimeSpan.FromMilliseconds(700))
            .WithEasing(new QuadraticEaseOut())
        .To(0.0, TimeSpan.FromMilliseconds(700))
            .WithEasing(new BounceEaseOut())
        .Reset()
    .StartAsync();
```

Both approaches give you the same powerful animation capabilities!
