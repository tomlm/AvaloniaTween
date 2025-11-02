using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AvaloniaTweener.Fluent;

public class Timeline
{
    private readonly List<TimelineEntry> _entries = new();

    public Timeline Add(SelectorAnimationBuilder animation, string position = "+=0")
    {
        var offset = ParsePosition(position);
        _entries.Add(new TimelineEntry
        {
            Animation = animation,
            Offset = offset,
            Position = position
        });
        return this;
    }

    public async Task StartAsync(CancellationToken cancellationToken = default)
    {
        // For sequential animations with "+=0", we need to track actual duration
        TimeSpan currentTime = TimeSpan.Zero;
        
        foreach (var entry in _entries)
        {
            // Calculate when this animation should start
            TimeSpan startTime;
            
            if (entry.Position.StartsWith("+="))
            {
                // Relative to end of previous animation
                var delay = double.Parse(entry.Position[2..]);
                startTime = currentTime + TimeSpan.FromSeconds(delay);
            }
            else if (entry.Position.StartsWith("-="))
            {
                // Overlap with previous animation
                var overlap = double.Parse(entry.Position[2..]);
                startTime = currentTime - TimeSpan.FromSeconds(overlap);
            }
            else
            {
                // Absolute time
                var absoluteTime = double.Parse(entry.Position);
                startTime = TimeSpan.FromSeconds(absoluteTime);
            }
            
            // Wait until it's time to start this animation
            var waitTime = startTime - currentTime;
            if (waitTime > TimeSpan.Zero)
            {
                await Task.Delay(waitTime, cancellationToken);
            }
            
            // Start the animation
            await entry.Animation.StartAsync(cancellationToken);
            
            // Update current time (this is approximate since we don't know the actual duration)
            // For now, we'll track based on when we started the animation
            currentTime = startTime;
        }
    }

    private TimeSpan ParsePosition(string position)
    {
        // Simple parser for positions like "+=0.5", "-=0.2", "1.5"
        if (position.StartsWith("+="))
        {
            var seconds = double.Parse(position[2..]);
            var lastOffset = _entries.LastOrDefault()?.Offset ?? TimeSpan.Zero;
            return lastOffset + TimeSpan.FromSeconds(seconds);
        }
        else if (position.StartsWith("-="))
        {
            var seconds = double.Parse(position[2..]);
            var lastOffset = _entries.LastOrDefault()?.Offset ?? TimeSpan.Zero;
            return lastOffset - TimeSpan.FromSeconds(seconds);
        }
        else
        {
            var seconds = double.Parse(position);
            return TimeSpan.FromSeconds(seconds);
        }
    }

    private class TimelineEntry
    {
        public SelectorAnimationBuilder Animation { get; init; } = null!;
        public TimeSpan Offset { get; init; }
        public string Position { get; init; } = "+=0";
    }
}
