using DVPF.Core;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace OsuStatePresenter.Nodes
{
    // TODO: Build the "paused" status into the custom Status class
    [StateProperty(enabled: true, name: "SongIsPaused")]
    class PausedNode : OsuNode
    {
        // TODO: Fix bug: Jumping around in the map editor flickers Paused to true then back to false.
        // Possible solution: Use another timer to check the "expected" time, and do a fuzzy check around that time. If it's a huge difference, still assume the map is paused.

        /// <summary>
        /// Use a timer to delay the checking of the current/previous MapTime, to fix the
        /// "difference between 2 MapTime states" causing "flicker" between Paused=true/false.
        /// </summary>
        private Stopwatch _timer = new Stopwatch();

        public TimeSpan MinUpdateTime { get; set; } = StatePresenter.ScannerInterval.Add(TimeSpan.FromMilliseconds(250));

        public override async Task<object> DetermineValueAsync()
        {
            if (!_timer.IsRunning)
            {
                _timer.Start();
            }

            var currentValue = this.GetValue();

            if (_timer.Elapsed.CompareTo(MinUpdateTime) < 0 && currentValue != null) // timer time less than min time
            {
                // return whatever the current value is
                // _logger.Info($"false: {_timer.Elapsed.TotalMilliseconds}, {MinUpdateTime.TotalMilliseconds}");
                return await Task.FromResult(currentValue);
            }

            _timer.Restart();

            Preceders.TryGetValue(typeof(MapTimeNode), out var mapTimeNode);
            //Preceders.TryGetValue(typeof(StatusNode), out var statusNode);

            var currentMapTime = (int?)mapTimeNode?.GetValue() ?? 0;
            var previousMapTime = (int?)mapTimeNode?.GetPreviousValue() ?? 0;
            //var status = (string)statusNode?.GetValue();

            //_logger.Info($"{previousMapTime} -> {currentMapTime}");

            //if (status.Contains("Playing") && currentMapTime.Equals(previousMapTime))
            if (currentMapTime.Equals(previousMapTime))
            {
                return await Task.FromResult(true);
            }

            return await Task.FromResult(false);
        }

    }
}
