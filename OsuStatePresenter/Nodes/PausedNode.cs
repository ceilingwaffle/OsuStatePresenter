using DVPF.Core;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace OsuStatePresenter.Nodes
{
    // TODO: REFACTOR - Build the "paused" status into the custom Status class
    [StateProperty(enabled: true, name: "SongIsPaused")]
    public class PausedNode : OsuNode
    {
        // TODO: BUG - Jumping around in the map editor flickers Paused to true then back to false.

        // TODO: PERFORMANCE -  Idea: Read two samples of MapTime really fast back-to-back and determine paused status from 
        //                      the difference, instead of using previous values stored in the MapTime Node (this will decrease
        //                      the time it takes to read the paused status when the scanner interval time is set to some high value).

        /// <summary>
        /// Minimum time delay between value changing from true->false or false->true
        /// to fix the situational "fast flicker" between Paused=true/false.
        ///
        /// The paused status may still update immediately (e.g. if the song was paused >= the min time, 
        /// and then the player clicks "play", the paused status changes to false immediately).
        /// </summary>
        public TimeSpan MinUpdateTime { get; set; }
        private TimeSpan _defaultMinUpdateTime = TimeSpan.FromMilliseconds(1000);
        private DateTime _timeOfLastValueChange = DateTime.Now;

        public PausedNode()
        {
            MinUpdateTime = _DeterminePausedStatusUpdateWaitTime();
        }

        public override async Task<object> DetermineValueAsync()
        {
            var currentValue = this.GetValue();
            bool isPaused = _GetPausedStatusFromMemory();

            if (currentValue is null)
            {
                return await Task.FromResult(isPaused);
            }

            if (this.ValueChanged())
            {
                _timeOfLastValueChange = DateTime.Now;
            }

            if (_ReadValueIsDifferentToLastSubmittedValue(currentValue, isPaused) && !_TimeSinceLastValueChangeIsGreaterThanMinUpdateTime())
            {
                // return whatever the current value is

                //_logger.Info($"false: {TimeSinceLastValueChange().TotalMilliseconds}, {MinUpdateTime.TotalMilliseconds}");
                return await Task.FromResult(currentValue);
            }

            return await Task.FromResult(isPaused);
        }

        private TimeSpan _DeterminePausedStatusUpdateWaitTime()
        {
            double scanTime = StatePresenter.NodeScannerInterval.Add(TimeSpan.FromMilliseconds(250)).TotalMilliseconds;
            double minTime = _defaultMinUpdateTime.TotalMilliseconds;

            double chosenTime = Math.Max(scanTime, minTime);

            return TimeSpan.FromMilliseconds(chosenTime);
        }

        private bool _GetPausedStatusFromMemory()
        {
            Preceders.TryGetValue(typeof(MapTimeNode), out var mapTimeNode);
            //Preceders.TryGetValue(typeof(StatusNode), out var statusNode);

            var currentMapTime = (int?)mapTimeNode?.GetValue() ?? 0;
            var previousMapTime = (int?)mapTimeNode?.GetPreviousValue() ?? 0;
            //var status = (string)statusNode?.GetValue();

            _logger.Info($"{previousMapTime} -> {currentMapTime}");

            //if (status.Contains("Playing") && currentMapTime.Equals(previousMapTime))
            if (currentMapTime.Equals(previousMapTime))
            {
                return true;
            }

            return false;
        }

        private bool _ReadValueIsDifferentToLastSubmittedValue(object currentValue, bool isPausedReadFromMemory)
        {
            if (currentValue is null)
            {
                throw new Exception($"currentValue should never be null here (in {this.GetType().ToString()}).");
            }

            return !((bool)currentValue).Equals(isPausedReadFromMemory);
        }

        private bool _TimeSinceLastValueChangeIsGreaterThanMinUpdateTime()
        {
            return _TimeSinceLastValueChange().CompareTo(MinUpdateTime) > 0;
        }

        private TimeSpan _TimeSinceLastValueChange()
        {
            return DateTime.Now - _timeOfLastValueChange;
        }
    }
}
