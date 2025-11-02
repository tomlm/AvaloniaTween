using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Animation.Easings;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using AvaloniaTweener.Controls;
using Xunit;

namespace AvaloniaTweener.Tests
{
    public class TweenParserTests
    {
        [Fact]
        public void Parse_SimpleOpacityAnimation_CreatesCorrectAnimation()
        {
            // Arrange
            var tween = "opacity:0->1@500ms";

            // Act
            var animation = TweenParser.Parse(tween);

            // Assert
            Assert.NotNull(animation);
            Assert.Single(animation.Tracks);
            
            var track = animation.Tracks[0];
            Assert.Equal(Visual.OpacityProperty, track.Property);
            Assert.Equal(0.0, track.From);
            Assert.Equal(1.0, track.To);
            Assert.Equal(TimeSpan.FromMilliseconds(500), track.Duration);
        }

        [Fact]
        public void Parse_AnimationWithEasing_CreatesCorrectEasing()
        {
            // Arrange
            var tween = "opacity:0->1@500ms ease:OutCubic";

            // Act
            var animation = TweenParser.Parse(tween);

            // Assert
            Assert.NotNull(animation);
            Assert.Single(animation.Tracks);
            
            var track = animation.Tracks[0];
            Assert.IsType<CubicEaseOut>(track.Easing);
        }

        [Fact]
        public void Parse_MultipleEasingNames_CreatesCorrectEasing()
        {
            // Test various easing names
            var easings = new Dictionary<string, Type>
            {
                { "ease:OutBack", typeof(BackEaseOut) },
                { "ease:InBack", typeof(BackEaseIn) },
                { "ease:InOutBack", typeof(BackEaseInOut) },
                { "ease:OutBounce", typeof(BounceEaseOut) },
                { "ease:InBounce", typeof(BounceEaseIn) },
                { "ease:OutElastic", typeof(ElasticEaseOut) },
                { "ease:InElastic", typeof(ElasticEaseIn) },
                { "ease:OutQuad", typeof(QuadraticEaseOut) },
                { "ease:InQuad", typeof(QuadraticEaseIn) },
                { "ease:Linear", typeof(LinearEasing) }
            };

            foreach (var kvp in easings)
            {
                // Arrange
                var tween = $"opacity:0->1@500ms {kvp.Key}";

                // Act
                var animation = TweenParser.Parse(tween);

                // Assert
                Assert.NotNull(animation);
                var track = animation.Tracks[0];
                Assert.IsType(kvp.Value, track.Easing);
            }
        }

        [Fact]
        public void Parse_MarginAnimation_CreatesCorrectThicknessValues()
        {
            // Arrange
            var tween = "margin:0->200,0,0,0@800ms";

            // Act
            var animation = TweenParser.Parse(tween);

            // Assert
            Assert.NotNull(animation);
            Assert.Single(animation.Tracks);
            
            var track = animation.Tracks[0];
            Assert.Equal(Layoutable.MarginProperty, track.Property);
            Assert.Equal(new Thickness(0), track.From);
            Assert.Equal(new Thickness(200, 0, 0, 0), track.To);
            Assert.Equal(TimeSpan.FromMilliseconds(800), track.Duration);
        }

        [Fact]
        public void Parse_TranslateTransformY_CreatesCorrectAnimation()
        {
            // Arrange
            var tween = "y:0->-50@400ms ease:OutQuad";

            // Act
            var animation = TweenParser.Parse(tween);

            // Assert
            Assert.NotNull(animation);
            Assert.Single(animation.Tracks);
            
            var track = animation.Tracks[0];
            Assert.Equal(TranslateTransform.YProperty, track.Property);
            Assert.Equal(0.0, track.From);
            Assert.Equal(-50.0, track.To);
            Assert.Equal(TimeSpan.FromMilliseconds(400), track.Duration);
        }

        [Fact]
        public void Parse_ScaleTransform_CreatesCorrectAnimation()
        {
            // Arrange
            var tween = "scalex:1->1.5@300ms ease:OutBack";

            // Act
            var animation = TweenParser.Parse(tween);

            // Assert
            Assert.NotNull(animation);
            Assert.Single(animation.Tracks);
            
            var track = animation.Tracks[0];
            Assert.Equal(ScaleTransform.ScaleXProperty, track.Property);
            Assert.Equal(1.0, track.From);
            Assert.Equal(1.5, track.To);
            Assert.Equal(TimeSpan.FromMilliseconds(300), track.Duration);
        }

        [Fact]
        public void Parse_RotateTransform_CreatesCorrectAnimation()
        {
            // Arrange
            var tween = "angle:0->360@1s ease:InOutCubic";

            // Act
            var animation = TweenParser.Parse(tween);

            // Assert
            Assert.NotNull(animation);
            Assert.Single(animation.Tracks);
            
            var track = animation.Tracks[0];
            Assert.Equal(RotateTransform.AngleProperty, track.Property);
            Assert.Equal(0.0, track.From);
            Assert.Equal(360.0, track.To);
            Assert.Equal(TimeSpan.FromSeconds(1), track.Duration);
        }

        [Fact]
        public void Parse_MultipleAnimations_CreatesMultipleTracks()
        {
            // Arrange
            var tween = "opacity:1->0@500ms ease:OutCubic; opacity:0->1@500ms ease:InCubic";

            // Act
            var animation = TweenParser.Parse(tween);

            // Assert
            Assert.NotNull(animation);
            Assert.Equal(2, animation.Tracks.Count);
            
            var track1 = animation.Tracks[0];
            Assert.Equal(Visual.OpacityProperty, track1.Property);
            Assert.Equal(1.0, track1.From);
            Assert.Equal(0.0, track1.To);
            Assert.IsType<CubicEaseOut>(track1.Easing);

            var track2 = animation.Tracks[1];
            Assert.Equal(Visual.OpacityProperty, track2.Property);
            Assert.Equal(0.0, track2.From);
            Assert.Equal(1.0, track2.To);
            Assert.IsType<CubicEaseIn>(track2.Easing);
        }

        [Fact]
        public void Parse_ComplexMultiTransform_CreatesMultipleTracks()
        {
            // Arrange
            var tween = "angle:0->180@800ms ease:OutBack; scalex:1->1.3@400ms; scalex:1.3->1@400ms; scaley:1->1.3@400ms; scaley:1.3->1@400ms";

            // Act
            var animation = TweenParser.Parse(tween);

            // Assert
            Assert.NotNull(animation);
            Assert.Equal(5, animation.Tracks.Count);
            
            Assert.Equal(RotateTransform.AngleProperty, animation.Tracks[0].Property);
            Assert.Equal(ScaleTransform.ScaleXProperty, animation.Tracks[1].Property);
            Assert.Equal(ScaleTransform.ScaleXProperty, animation.Tracks[2].Property);
            Assert.Equal(ScaleTransform.ScaleYProperty, animation.Tracks[3].Property);
            Assert.Equal(ScaleTransform.ScaleYProperty, animation.Tracks[4].Property);
        }

        [Fact]
        public void Parse_CanvasLeftProperty_CreatesCorrectAnimation()
        {
            // Arrange
            var tween = "left:0->200@1s";

            // Act
            var animation = TweenParser.Parse(tween);

            // Assert
            Assert.NotNull(animation);
            Assert.Single(animation.Tracks);
            
            var track = animation.Tracks[0];
            Assert.Equal(Canvas.LeftProperty, track.Property);
            Assert.Equal(0.0, track.From);
            Assert.Equal(200.0, track.To);
        }

        [Fact]
        public void Parse_DurationInSeconds_ParsesCorrectly()
        {
            // Arrange
            var tween = "opacity:0->1@2s";

            // Act
            var animation = TweenParser.Parse(tween);

            // Assert
            Assert.NotNull(animation);
            var track = animation.Tracks[0];
            Assert.Equal(TimeSpan.FromSeconds(2), track.Duration);
        }

        [Fact]
        public void Parse_DurationInMilliseconds_ParsesCorrectly()
        {
            // Arrange
            var tween = "opacity:0->1@750ms";

            // Act
            var animation = TweenParser.Parse(tween);

            // Assert
            Assert.NotNull(animation);
            var track = animation.Tracks[0];
            Assert.Equal(TimeSpan.FromMilliseconds(750), track.Duration);
        }

        [Fact]
        public void Parse_WidthAndHeightProperties_CreatesCorrectAnimations()
        {
            // Arrange
            var tween = "width:100->200@500ms; height:50->150@500ms";

            // Act
            var animation = TweenParser.Parse(tween);

            // Assert
            Assert.NotNull(animation);
            Assert.Equal(2, animation.Tracks.Count);
            
            Assert.Equal(Layoutable.WidthProperty, animation.Tracks[0].Property);
            Assert.Equal(100.0, animation.Tracks[0].From);
            Assert.Equal(200.0, animation.Tracks[0].To);

            Assert.Equal(Layoutable.HeightProperty, animation.Tracks[1].Property);
            Assert.Equal(50.0, animation.Tracks[1].From);
            Assert.Equal(150.0, animation.Tracks[1].To);
        }

        [Fact]
        public void Parse_UniformThickness_ParsesCorrectly()
        {
            // Arrange
            var tween = "margin:0->20@500ms";

            // Act
            var animation = TweenParser.Parse(tween);

            // Assert
            Assert.NotNull(animation);
            var track = animation.Tracks[0];
            Assert.Equal(new Thickness(0), track.From);
            Assert.Equal(new Thickness(20), track.To);
        }

        [Fact]
        public void Parse_TwoValueThickness_ParsesCorrectly()
        {
            // Arrange
            var tween = "margin:0,0->20,10@500ms";

            // Act
            var animation = TweenParser.Parse(tween);

            // Assert
            Assert.NotNull(animation);
            var track = animation.Tracks[0];
            Assert.Equal(new Thickness(0, 0), track.From);
            Assert.Equal(new Thickness(20, 10), track.To);
        }
    }
}
