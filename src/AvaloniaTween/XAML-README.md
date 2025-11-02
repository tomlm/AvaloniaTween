# XAML Markup Support for AvaloniaTween

This document describes how to use AvaloniaTween animations in XAML markup.

## Installation

Add the XML namespace to your XAML file:

```xml
xmlns:anim="using:AvaloniaAnimate.Xaml"
```

## Features

### 1. **Reanimator.Animate** - Compact Inline Syntax

The most concise way to define animations directly in XAML:

```xml
<Border anim:Reanimator.Animate="opacity:0->1@500ms ease:OutCubic">
    <TextBlock Text="Fades in on load!" />
</Border>
```

**Syntax Format:**
```
property:from->to@duration ease:easing; property2:from->to@duration
```

**Examples:**

```xml
<!-- Single property -->
<Border anim:Reanimator.Animate="opacity:0->1@500ms" />

<!-- Multiple properties (semicolon separated) -->
<Border anim:Reanimator.Animate="opacity:0->1@500ms; left:0->200@1s" />

<!-- With easing -->
<Border anim:Reanimator.Animate="scalex:1->1.1@200ms ease:OutBack" />

<!-- With delay -->
<Border anim:Reanimator.Animate="opacity:0->1@500ms delay:1s" />

<!-- Only "to" value (uses current value as "from") -->
<Border anim:Reanimator.Animate="opacity:1@500ms" />
```

**Supported Properties:**
- `opacity` - Visual.OpacityProperty
- `left` - Canvas.LeftProperty
- `top` - Canvas.TopProperty
- `width` - Layoutable.WidthProperty
- `height` - Layoutable.HeightProperty
- `angle` - RotateTransform.AngleProperty
- `scalex` - ScaleTransform.ScaleXProperty
- `scaley` - ScaleTransform.ScaleYProperty
- `x` - TranslateTransform.XProperty
- `y` - TranslateTransform.YProperty

**Supported Easings:**
- `Linear`, `OutCubic`, `InCubic`, `InOutCubic`
- `OutBack`, `InBack`, `InOutBack`
- `OutBounce`, `InBounce`, `InOutBounce`
- `OutElastic`, `InElastic`, `InOutElastic`
- `OutQuad`, `InQuad`, `InOutQuad`
- `OutExpo`, `InExpo`, `InOutExpo`
- `OutCirc`, `InCirc`, `InOutCirc`
- `OutSine`, `InSine`, `InOutSine`

**Time Units:**
- `ms` - milliseconds (e.g., `500ms`)
- `s` - seconds (e.g., `1.5s`)

---

### 2. **Reanimator.OnLoad** - Named Animations

Play a named animation that was registered in code-behind:

```xml
<Border anim:Reanimator.OnLoad="fadeIn">
    <TextBlock Text="Plays registered animation!" />
</Border>
```

**Register animations in your code-behind or App.xaml.cs:**

```csharp
Animator.Register("fadeIn", builder =>
{
    builder.Animate(Visual.OpacityProperty)
        .FromTo(0.0, 1.0, TimeSpan.FromSeconds(0.5))
        .WithEasing(new CubicEaseOut());
});
```

---

### 3. **AnimationResource** - Reusable Animation Definitions

Define complex animations as XAML resources:

```xml
<Window.Resources>
    <anim:AnimationResource x:Key="FadeIn" Name="fadeIn" Selector="Self">
        <anim:AnimationTrack Property="{x:Static Visual.OpacityProperty}" 
                             From="0" To="1" Duration="0:0:0.5">
            <anim:AnimationTrack.Easing>
                <CubicEaseOut/>
            </anim:AnimationTrack.Easing>
        </anim:AnimationTrack>
    </anim:AnimationResource>

    <anim:AnimationResource x:Key="SlideAndFade" Selector="Self">
        <anim:AnimationTrack Property="{x:Static Canvas.LeftProperty}" 
                             From="0" To="200" Duration="0:0:1"/>
        <anim:AnimationTrack Property="{x:Static Visual.OpacityProperty}" 
                             From="0" To="1" Duration="0:0:1"/>
    </anim:AnimationResource>
</Window.Resources>

<!-- Use the resource -->
<Border anim:Reanimator.OnLoadResource="{StaticResource FadeIn}">
    <TextBlock Text="Using resource animation!" />
</Border>
```

---

### 4. **Style-Based Animations**

Apply animations in Avalonia styles:

```xml
<Window.Styles>
    <Style Selector="Button:pointerover">
        <Setter Property="anim:Reanimator.Animate" 
                Value="scalex:1->1.1@200ms ease:OutBack; scaley:1->1.1@200ms ease:OutBack"/>
    </Style>
</Window.Styles>

<Button Content="Hover me!" />
```

---

## Complete Example

```xml
<Window xmlns="https://github.com/avaloniaui"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        xmlns:anim="using:AvaloniaAnimate.Xaml">

    <Window.Resources>
        <!-- Define reusable animation -->
        <anim:AnimationResource x:Key="BounceIn" Selector="Self">
            <anim:AnimationTrack Property="{x:Static ScaleTransform.ScaleXProperty}" 
                                 From="0" To="1" Duration="0:0:0.6">
                <anim:AnimationTrack.Easing>
                    <BounceEaseOut/>
                </anim:AnimationTrack.Easing>
            </anim:AnimationTrack>
        </anim:AnimationResource>
    </Window.Resources>

    <Window.Styles>
        <!-- Style with animation on hover -->
        <Style Selector="Button.animated:pointerover">
            <Setter Property="anim:Reanimator.Animate" 
                    Value="scalex:1->1.1@200ms ease:OutBack"/>
        </Style>
    </Window.Styles>

    <StackPanel Spacing="20" Margin="20">
        
        <!-- Inline compact syntax -->
        <Border Background="Blue" 
                Padding="20" 
                anim:Reanimator.Animate="opacity:0->1@500ms ease:OutCubic">
            <TextBlock Text="Fade in!" Foreground="White"/>
        </Border>

        <!-- Named animation -->
        <Border Background="Green" 
                Padding="20"
                anim:Reanimator.OnLoad="fadeIn">
            <TextBlock Text="Named animation!" Foreground="White"/>
        </Border>

        <!-- Resource-based -->
        <Border Background="Red" 
                Padding="20"
                RenderTransformOrigin="0.5,0.5"
                anim:Reanimator.OnLoadResource="{StaticResource BounceIn}">
            <Border.RenderTransform>
                <ScaleTransform/>
            </Border.RenderTransform>
            <TextBlock Text="Bounce in!" Foreground="White"/>
        </Border>

        <!-- Styled animation -->
        <Button Content="Hover Me!" Classes="animated" />

        <!-- Multiple properties -->
        <Canvas Height="100">
            <Border Background="Orange" 
                    Padding="20"
                    Canvas.Left="0"
                    anim:Reanimator.Animate="left:0->300@1s ease:OutElastic; opacity:0->1@1s">
                <TextBlock Text="Slide and fade!" Foreground="White"/>
            </Border>
        </Canvas>

    </StackPanel>
</Window>
```

---

## Benefits Over Standard Avalonia Animations

? **95% less XAML** - Compare `anim:Reanimator.Animate="opacity:0->1@500ms"` to standard KeyFrame syntax  
? **Multi-property support** - Animate multiple properties in one line  
? **Style-friendly** - Works seamlessly in Avalonia styles  
? **Named registry** - Define once, use anywhere  
? **Resource support** - Complex animations as reusable resources  
? **Fluent syntax** - Same philosophy as the code-behind API

---

## Tips

1. **For transforms**, remember to add `RenderTransform` and set `RenderTransformOrigin`:
   ```xml
   <Border RenderTransformOrigin="0.5,0.5"
           anim:Reanimator.Animate="scalex:0->1@500ms">
       <Border.RenderTransform>
           <ScaleTransform/>
       </Border.RenderTransform>
   </Border>
   ```

2. **Combine code and XAML**: Register complex animations in code, reference them by name in XAML

3. **Use styles for reusable animations**: Define once in a style, apply to all matching controls

4. **Semicolons separate properties**: `"opacity:0->1@500ms; left:0->200@1s"`

5. **Animations play on load**: All Reanimator attached properties trigger when the control loads
