# AvaloniaTweener.Tests

Unit test project for the AvaloniaTweener library.

## Test Coverage

This test project includes comprehensive unit tests for:

### 1. Tween Parser Tests (`TweenParserTests.cs`)

Tests for the compact tween syntax parser that creates animations from strings.

**Coverage:**
- ? Simple opacity animations
- ? Animations with easing functions (OutCubic, OutBack, InBack, OutBounce, etc.)
- ? Margin animations with Thickness values
- ? TranslateTransform (x, y) animations
- ? ScaleTransform (scalex, scaley) animations
- ? RotateTransform (angle) animations
- ? Multiple animations in sequence (semicolon-separated)
- ? Complex multi-transform animations
- ? Canvas positioning properties (left, top, right, bottom)
- ? Duration parsing (seconds and milliseconds)
- ? Width and height animations
- ? Thickness parsing (uniform, two-value, four-value)

**Example Test Cases:**
```csharp
"opacity:0->1@500ms"
"y:0->-50@400ms ease:OutQuad"
"scalex:1->1.5@300ms ease:OutBack"
"angle:0->360@1s ease:InOutCubic"
"margin:0->200,0,0,0@800ms"
```

### 2. Fluent Interface Tests (`FluentInterfaceTests.cs`)

Tests for the programmatic fluent API for creating animations.

**Coverage:**
- ? Single property animations
- ? From/To keyframe creation
- ? Easing function application
- ? Delay configuration
- ? Speed multiplier configuration
- ? Repeat count configuration
- ? Yoyo (ping-pong) effect
- ? Multiple property animations
- ? Complex animation chains
- ? Reset functionality (restore original values)
- ? Hold functionality (forward fill mode)
- ? FromTo shorthand method
- ? Animation callbacks (OnStart, OnComplete, OnUpdate, OnRepeat)
- ? Batch operations on all tracks (WithDelay, WithEasing, Repeat, Yoyo)

**Example Test Cases:**
```csharp
Animator.Select(button)
    .Animate(Visual.OpacityProperty)
    .From(0.0)
    .To(1.0, TimeSpan.FromMilliseconds(500))
    .WithEasing(new CubicEaseOut())
    .Repeat(3)
    .Yoyo(true)
    .Build();
```

## Running the Tests

### Run all tests:
```bash
dotnet test
```

### Run with detailed output:
```bash
dotnet test --verbosity normal
```

### Run specific test class:
```bash
dotnet test --filter "FullyQualifiedName~TweenParserTests"
dotnet test --filter "FullyQualifiedName~FluentInterfaceTests"
```

## Test Statistics

- **Total Tests:** 32
- **Tween Parser Tests:** 16
- **Fluent Interface Tests:** 16

## Dependencies

- **xUnit** - Testing framework
- **Avalonia.Headless** - Headless rendering for Avalonia UI testing
- **Microsoft.NET.Test.Sdk** - .NET Test SDK

## Architecture Notes

### Avalonia Initialization
The tests use `IClassFixture<AvaloniaFixture>` to ensure Avalonia is initialized only once across all tests, avoiding the "Setup was already called" error.

### Test Helper: AvaloniaFixture
```csharp
public class AvaloniaFixture
{
    public AvaloniaFixture()
    {
        AppBuilder.Configure<TestApplication>()
            .UseHeadless(new AvaloniaHeadlessPlatformOptions())
            .SetupWithoutStarting();
    }
}
```

### Reflection-Based Testing
Some tests use reflection to access internal state of `SelectorAnimationBuilder` to verify that animations are configured correctly without needing to run them.

## Future Enhancements

Potential areas for additional test coverage:
- Integration tests with actual animation playback
- Timeline orchestration tests
- Selector resolution tests
- Animation registry tests
- Performance benchmarks
- Edge cases and error handling
