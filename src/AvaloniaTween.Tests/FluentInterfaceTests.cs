using System;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Animation.Easings;
using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Layout;
using Avalonia.Media;
using Xunit;

namespace AvaloniaTweener.Tests
{
    public class FluentInterfaceTests : IClassFixture<AvaloniaFixture>
    {
        [Fact]
        public void Animate_SingleProperty_CreatesTrack()
        {
            // Arrange
            var button = new Button();
            var builder = Animator.Select(button);

            // Act
            builder.Animate(Visual.OpacityProperty)
                .From(0.0)
                .To(1.0, TimeSpan.FromMilliseconds(500))
                .Build();

            // Assert
            Assert.True(builder != null);
        }

        [Fact]
        public void Animate_FromTo_CreatesCorrectKeyframes()
        {
            // Arrange
            var button = new Button();
            var builder = Animator.Select(button);
            PropertyAnimationTrack? track = null;

            // Act
            var propertyBuilder = builder.Animate(Visual.OpacityProperty);
            propertyBuilder.From(0.0).To(1.0, TimeSpan.FromMilliseconds(500));
            
            // Access internal state through reflection for testing
            var field = typeof(SelectorAnimationBuilder).GetField("_tracks",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var tracks = field?.GetValue(builder) as System.Collections.Generic.List<PropertyAnimationTrack>;
            
            if (tracks != null && tracks.Count > 0)
            {
                track = tracks[0];
            }

            // Assert
            Assert.NotNull(track);
            Assert.Equal(Visual.OpacityProperty, track.Property);
            Assert.NotNull(track.KeyFrames);
            Assert.Equal(2, track.KeyFrames.Count);
            Assert.Equal(0.0, track.KeyFrames[0].Cue);
            Assert.Equal(0.0, track.KeyFrames[0].Value);
            Assert.Equal(1.0, track.KeyFrames[1].Cue);
            Assert.Equal(1.0, track.KeyFrames[1].Value);
            Assert.Equal(TimeSpan.FromMilliseconds(500), track.KeyFrames[1].Duration);
        }

        [Fact]
        public void Animate_WithEasing_SetsEasingOnKeyframe()
        {
            // Arrange
            var button = new Button();
            var builder = Animator.Select(button);

            // Act
            builder.Animate(Visual.OpacityProperty)
                .From(0.0)
                .To(1.0, TimeSpan.FromMilliseconds(500))
                .WithEasing(new CubicEaseOut())
                .Build();

            // Access internal state
            var field = typeof(SelectorAnimationBuilder).GetField("_tracks",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var tracks = field?.GetValue(builder) as System.Collections.Generic.List<PropertyAnimationTrack>;
            var track = tracks?[0];

            // Assert
            Assert.NotNull(track);
            Assert.NotNull(track.KeyFrames);
            Assert.Equal(2, track.KeyFrames.Count);
            Assert.IsType<CubicEaseOut>(track.KeyFrames[1].Easing);
        }

        [Fact]
        public void Animate_WithDelay_SetsDelayOnTrack()
        {
            // Arrange
            var button = new Button();
            var builder = Animator.Select(button);

            // Act
            builder.Animate(Visual.OpacityProperty)
                .From(0.0)
                .To(1.0, TimeSpan.FromMilliseconds(500))
                .WithDelay(TimeSpan.FromMilliseconds(200))
                .Build();

            // Access internal state
            var field = typeof(SelectorAnimationBuilder).GetField("_tracks",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var tracks = field?.GetValue(builder) as System.Collections.Generic.List<PropertyAnimationTrack>;
            var track = tracks?[0];

            // Assert
            Assert.NotNull(track);
            Assert.Equal(TimeSpan.FromMilliseconds(200), track.Delay);
        }

        [Fact]
        public void Animate_WithSpeed_SetsSpeedRatioOnTrack()
        {
            // Arrange
            var button = new Button();
            var builder = Animator.Select(button);

            // Act
            builder.Animate(Visual.OpacityProperty)
                .From(0.0)
                .To(1.0, TimeSpan.FromMilliseconds(500))
                .WithSpeed(2.0)
                .Build();

            // Access internal state
            var field = typeof(SelectorAnimationBuilder).GetField("_tracks",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var tracks = field?.GetValue(builder) as System.Collections.Generic.List<PropertyAnimationTrack>;
            var track = tracks?[0];

            // Assert
            Assert.NotNull(track);
            Assert.Equal(2.0, track.SpeedRatio);
        }

        [Fact]
        public void Animate_Repeat_SetsRepeatCountOnTrack()
        {
            // Arrange
            var button = new Button();
            var builder = Animator.Select(button);

            // Act
            builder.Animate(Visual.OpacityProperty)
                .From(0.0)
                .To(1.0, TimeSpan.FromMilliseconds(500))
                .Repeat(3)
                .Build();

            // Access internal state
            var field = typeof(SelectorAnimationBuilder).GetField("_tracks",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var tracks = field?.GetValue(builder) as System.Collections.Generic.List<PropertyAnimationTrack>;
            var track = tracks?[0];

            // Assert
            Assert.NotNull(track);
            Assert.Equal(3, track.RepeatCount);
        }

        [Fact]
        public void Animate_Yoyo_SetsYoyoOnTrack()
        {
            // Arrange
            var button = new Button();
            var builder = Animator.Select(button);

            // Act
            builder.Animate(Visual.OpacityProperty)
                .From(0.0)
                .To(1.0, TimeSpan.FromMilliseconds(500))
                .Yoyo(true)
                .Build();

            // Access internal state
            var field = typeof(SelectorAnimationBuilder).GetField("_tracks",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var tracks = field?.GetValue(builder) as System.Collections.Generic.List<PropertyAnimationTrack>;
            var track = tracks?[0];

            // Assert
            Assert.NotNull(track);
            Assert.True(track.Yoyo);
        }

        [Fact]
        public void Animate_MultipleProperties_CreatesMultipleTracks()
        {
            // Arrange
            var button = new Button();
            var builder = Animator.Select(button);

            // Act
            builder.Animate(Visual.OpacityProperty)
                .From(0.0)
                .To(1.0, TimeSpan.FromMilliseconds(500))
                .Animate(Layoutable.WidthProperty)
                .From(100.0)
                .To(200.0, TimeSpan.FromMilliseconds(500))
                .Build();

            // Access internal state
            var field = typeof(SelectorAnimationBuilder).GetField("_tracks",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var tracks = field?.GetValue(builder) as System.Collections.Generic.List<PropertyAnimationTrack>;

            // Assert
            Assert.NotNull(tracks);
            Assert.Equal(2, tracks.Count);
            Assert.Equal(Visual.OpacityProperty, tracks[0].Property);
            Assert.Equal(Layoutable.WidthProperty, tracks[1].Property);
        }

        [Fact]
        public void Animate_ComplexChain_CreatesCorrectConfiguration()
        {
            // Arrange
            var button = new Button();
            var builder = Animator.Select(button);

            // Act
            builder.Animate(Visual.OpacityProperty)
                .From(1.0)
                .To(0.0, TimeSpan.FromMilliseconds(300))
                .WithEasing(new CubicEaseOut())
                .WithDelay(TimeSpan.FromMilliseconds(100))
                .Animate(ScaleTransform.ScaleXProperty)
                .From(1.0)
                .To(1.5, TimeSpan.FromMilliseconds(400))
                .WithEasing(new BackEaseOut())
                .Repeat(2)
                .Build();

            // Access internal state
            var field = typeof(SelectorAnimationBuilder).GetField("_tracks",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var tracks = field?.GetValue(builder) as System.Collections.Generic.List<PropertyAnimationTrack>;

            // Assert
            Assert.NotNull(tracks);
            Assert.Equal(2, tracks.Count);

            // First track (opacity)
            var opacityTrack = tracks[0];
            Assert.Equal(Visual.OpacityProperty, opacityTrack.Property);
            Assert.Equal(TimeSpan.FromMilliseconds(100), opacityTrack.Delay);
            Assert.NotNull(opacityTrack.KeyFrames);
            Assert.Equal(2, opacityTrack.KeyFrames.Count);

            // Second track (scale)
            var scaleTrack = tracks[1];
            Assert.Equal(ScaleTransform.ScaleXProperty, scaleTrack.Property);
            Assert.Equal(2, scaleTrack.RepeatCount);
            Assert.NotNull(scaleTrack.KeyFrames);
            Assert.Equal(2, scaleTrack.KeyFrames.Count);
        }

        [Fact]
        public void Animate_Reset_SetsRestoreOriginalValue()
        {
            // Arrange
            var button = new Button();
            var builder = Animator.Select(button);

            // Act
            builder.Animate(Visual.OpacityProperty)
                .From(0.0)
                .To(1.0, TimeSpan.FromMilliseconds(500))
                .Reset()
                .Build();

            // Access internal state
            var field = typeof(SelectorAnimationBuilder).GetField("_tracks",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var tracks = field?.GetValue(builder) as System.Collections.Generic.List<PropertyAnimationTrack>;
            var track = tracks?[0];

            // Assert
            Assert.NotNull(track);
            Assert.True(track.RestoreOriginalValue);
            Assert.Equal(FillMode.None, track.FillMode);
        }

        [Fact]
        public void Animate_Hold_SetsFillModeForward()
        {
            // Arrange
            var button = new Button();
            var builder = Animator.Select(button);

            // Act
            builder.Animate(Visual.OpacityProperty)
                .From(0.0)
                .To(1.0, TimeSpan.FromMilliseconds(500))
                .Hold()
                .Build();

            // Access internal state
            var field = typeof(SelectorAnimationBuilder).GetField("_tracks",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var tracks = field?.GetValue(builder) as System.Collections.Generic.List<PropertyAnimationTrack>;
            var track = tracks?[0];

            // Assert
            Assert.NotNull(track);
            Assert.Equal(FillMode.Forward, track.FillMode);
        }

        [Fact]
        public void Animate_FromToShorthand_CreatesCorrectKeyframes()
        {
            // Arrange
            var button = new Button();
            var builder = Animator.Select(button);

            // Act
            builder.Animate(Visual.OpacityProperty)
                .FromTo(0.0, 1.0, TimeSpan.FromMilliseconds(500))
                .Build();

            // Access internal state
            var field = typeof(SelectorAnimationBuilder).GetField("_tracks",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var tracks = field?.GetValue(builder) as System.Collections.Generic.List<PropertyAnimationTrack>;
            var track = tracks?[0];

            // Assert
            Assert.NotNull(track);
            Assert.NotNull(track.KeyFrames);
            Assert.Equal(2, track.KeyFrames.Count);
            Assert.Equal(0.0, track.KeyFrames[0].Value);
            Assert.Equal(1.0, track.KeyFrames[1].Value);
        }

        [Fact]
        public void Animate_Callbacks_AreSet()
        {
            // Arrange
            var button = new Button();
            var builder = Animator.Select(button);
            var onStartCalled = false;
            var onCompleteCalled = false;
            var onUpdateCalled = false;
            var onRepeatCalled = false;

            // Act
            builder.Animate(Visual.OpacityProperty)
                .From(0.0)
                .To(1.0, TimeSpan.FromMilliseconds(500))
                .OnStart(() => onStartCalled = true)
                .OnComplete(() => onCompleteCalled = true)
                .OnUpdate(progress => onUpdateCalled = true)
                .OnRepeat(() => onRepeatCalled = true)
                .Build();

            // Access internal state
            var field = typeof(SelectorAnimationBuilder).GetField("_tracks",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var tracks = field?.GetValue(builder) as System.Collections.Generic.List<PropertyAnimationTrack>;
            var track = tracks?[0];

            // Assert
            Assert.NotNull(track);
            Assert.NotNull(track.OnStart);
            Assert.NotNull(track.OnComplete);
            Assert.NotNull(track.OnUpdate);
            Assert.NotNull(track.OnRepeat);

            // Verify callbacks can be invoked
            track.OnStart?.Invoke();
            Assert.True(onStartCalled);
            
            track.OnComplete?.Invoke();
            Assert.True(onCompleteCalled);
            
            track.OnUpdate?.Invoke(0.5);
            Assert.True(onUpdateCalled);
            
            track.OnRepeat?.Invoke();
            Assert.True(onRepeatCalled);
        }

        [Fact]
        public void SelectorAnimationBuilder_WithDelay_AppliesDelayToAllTracks()
        {
            // Arrange
            var button = new Button();
            var builder = Animator.Select(button);

            // Act
            builder.Animate(Visual.OpacityProperty)
                .From(0.0)
                .To(1.0, TimeSpan.FromMilliseconds(500))
                .Animate(Layoutable.WidthProperty)
                .From(100.0)
                .To(200.0, TimeSpan.FromMilliseconds(500))
                .Build()
                .WithDelay(TimeSpan.FromMilliseconds(200));

            // Access internal state
            var field = typeof(SelectorAnimationBuilder).GetField("_tracks",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var tracks = field?.GetValue(builder) as System.Collections.Generic.List<PropertyAnimationTrack>;

            // Assert
            Assert.NotNull(tracks);
            Assert.Equal(2, tracks.Count);
            Assert.All(tracks, track => Assert.Equal(TimeSpan.FromMilliseconds(200), track.Delay));
        }

        [Fact]
        public void SelectorAnimationBuilder_WithEasing_AppliesEasingToAllTracks()
        {
            // Arrange
            var button = new Button();
            var builder = Animator.Select(button);
            var easing = new BounceEaseOut();

            // Act
            builder.Animate(Visual.OpacityProperty)
                .From(0.0)
                .To(1.0, TimeSpan.FromMilliseconds(500))
                .Animate(Layoutable.WidthProperty)
                .From(100.0)
                .To(200.0, TimeSpan.FromMilliseconds(500))
                .Build()
                .WithEasing(easing);

            // Access internal state
            var field = typeof(SelectorAnimationBuilder).GetField("_tracks",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var tracks = field?.GetValue(builder) as System.Collections.Generic.List<PropertyAnimationTrack>;

            // Assert
            Assert.NotNull(tracks);
            Assert.Equal(2, tracks.Count);
            Assert.All(tracks, track => Assert.Equal(easing, track.Easing));
        }

        [Fact]
        public void SelectorAnimationBuilder_Repeat_AppliesRepeatToAllTracks()
        {
            // Arrange
            var button = new Button();
            var builder = Animator.Select(button);

            // Act
            builder.Animate(Visual.OpacityProperty)
                .From(0.0)
                .To(1.0, TimeSpan.FromMilliseconds(500))
                .Animate(Layoutable.WidthProperty)
                .From(100.0)
                .To(200.0, TimeSpan.FromMilliseconds(500))
                .Build()
                .Repeat(3);

            // Access internal state
            var field = typeof(SelectorAnimationBuilder).GetField("_tracks",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var tracks = field?.GetValue(builder) as System.Collections.Generic.List<PropertyAnimationTrack>;

            // Assert
            Assert.NotNull(tracks);
            Assert.Equal(2, tracks.Count);
            Assert.All(tracks, track => Assert.Equal(3, track.RepeatCount));
        }

        [Fact]
        public void SelectorAnimationBuilder_Yoyo_AppliesYoyoToAllTracks()
        {
            // Arrange
            var button = new Button();
            var builder = Animator.Select(button);

            // Act
            builder.Animate(Visual.OpacityProperty)
                .From(0.0)
                .To(1.0, TimeSpan.FromMilliseconds(500))
                .Animate(Layoutable.WidthProperty)
                .From(100.0)
                .To(200.0, TimeSpan.FromMilliseconds(500))
                .Build()
                .Yoyo(true);

            // Access internal state
            var field = typeof(SelectorAnimationBuilder).GetField("_tracks",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var tracks = field?.GetValue(builder) as System.Collections.Generic.List<PropertyAnimationTrack>;

            // Assert
            Assert.NotNull(tracks);
            Assert.Equal(2, tracks.Count);
            Assert.All(tracks, track => Assert.True(track.Yoyo));
        }
    }

    // Test application for Avalonia Headless
    public class TestApplication : Application
    {
    }

    // Fixture to initialize Avalonia once for all tests
    public class AvaloniaFixture
    {
        public AvaloniaFixture()
        {
            AppBuilder.Configure<TestApplication>()
                .UseHeadless(new AvaloniaHeadlessPlatformOptions())
                .SetupWithoutStarting();
        }
    }
}
