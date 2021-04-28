namespace OsuStatePresenter.Nodes
{
    using System;
    using System.Threading.Tasks;

    using DVPF.Core;

    using OsuParsers.Beatmaps;
    using OsuParsers.Decoders;

    // TODO: REFACTOR - Build the "paused" status into the custom Status class

    /// <inheritdoc />
    /// <summary>
    /// The node representing whether or not the current beatmap is in a paused state (e.g. User hit the esc key, or pressed stop on the main menu.)
    /// </summary>
    [StateProperty(enabled: true, name: "SongIsPaused")]
    public class PausedNode : OsuNode
    {
        // TODO: BUG - Jumping around in the map editor flickers Paused to true then back to false.

        // TODO: PERFORMANCE -  Idea: Read two samples of MapTime really fast back-to-back and determine paused status from 
        //                      the difference, instead of using previous values stored in the MapTime Node (this will decrease
        //                      the time it takes to read the paused status when the scanner interval time is set to some high value).
        private readonly TimeSpan defaultMinUpdateTime = TimeSpan.FromMilliseconds(value: 1000);

        private DateTime timeOfLastValueChange = DateTime.Now;

        /// <inheritdoc />
        /// <summary>
        /// Initializes a new instance of the <see cref="T:OsuStatePresenter.Nodes.PausedNode" /> class.
        /// </summary>
        public PausedNode()
        {
            this.MinUpdateTime = this.DeterminePausedStatusUpdateWaitTime();
        }

        /// <summary>
        /// <para>
        /// Gets or sets the minimum time delay between value changing from true->false or false->true
        /// to fix the situational "fast flicker" between Paused=true/false.
        /// </para>
        /// <para>
        /// The paused status may still update immediately (e.g. if the song was paused >= the min time, 
        /// and then the player clicks "play", the paused status changes to false immediately).
        /// </para>
        /// </summary>
        public TimeSpan MinUpdateTime { get; set; }

        /// <inheritdoc />
        /// <summary>
        /// Returns a boolean wrapped in an object for whether or not the current map has been paused.
        /// </summary>
        /// <returns>
        /// The <see cref="T:System.Threading.Tasks.Task" />.
        /// </returns>
        public override async Task<object> DetermineValueAsync()
        {
            object currentValue = this.GetValue();
            bool isPaused = this.GetPausedStatusFromMemory();

            if (currentValue is null)
            {
                return await Task.FromResult(isPaused);
            }

            if (this.ValueChanged())
            {
                this.timeOfLastValueChange = DateTime.Now;
            }

            if (this.ReadValueIsDifferentToLastSubmittedValue(currentValue, isPaused)
                && !this.TimeSinceLastValueChangeIsGreaterThanMinUpdateTime())
            {
                // return whatever the current value is

                // _logger.Info($"false: {TimeSinceLastValueChange().TotalMilliseconds}, {MinUpdateTime.TotalMilliseconds}");
                return await Task.FromResult(currentValue);
            }

            return await Task.FromResult(isPaused);
        }

        private TimeSpan DeterminePausedStatusUpdateWaitTime()
        {
            double scanTime = StatePresenter.NodeScannerInterval.Add(TimeSpan.FromMilliseconds(value: 250)).TotalMilliseconds;
            double minTime = this.defaultMinUpdateTime.TotalMilliseconds;

            double chosenTime = Math.Max(scanTime, minTime);

            return TimeSpan.FromMilliseconds(chosenTime);
        }

        private bool GetPausedStatusFromMemory()
        {
            this.Preceders.TryGetValue(typeof(MapTimeNode), out var mapTimeNode);
            this.Preceders.TryGetValue(typeof(BeatmapNode), out var beatmapNode);

            int currentMapTime = (int?)mapTimeNode?.GetValue() ?? 0;
            int previousMapTime = (int?)mapTimeNode?.GetPreviousValue() ?? 0;

            // _logger.Info($"{previousMapTime} -> {currentMapTime}");

            // To avoid flickering between "paused" and "not paused" at the beginning of every map, 
            // we check the current map time against the time of the first beatmap hit object
            Beatmap beatmap = null;

            try
            {
                beatmap = (Beatmap)beatmapNode.GetValue();
            }
            catch (InvalidCastException e)
            {
                Logger.Warn($"Error casting beatmap object in PausedNode. The exception message is: {e.Message}");

                // just fall back to the older buggy "flicker paused/not paused" status when the map time is near the start of the map.
                if (currentMapTime.Equals(previousMapTime))
                {
                    return true;
                }
            }

            float timeOfFirstHitObject = float.MaxValue;

            if (beatmap != null)
            {
                timeOfFirstHitObject = ((BeatmapNode)beatmapNode).GetTimeOfFirstHitObject(beatmap);
            }

            return currentMapTime.Equals(previousMapTime) && currentMapTime > timeOfFirstHitObject;
        }

        private bool ReadValueIsDifferentToLastSubmittedValue(object currentValue, bool isPausedReadFromMemory)
        {
            if (currentValue is null)
            {
                throw new Exception($"currentValue should never be null here (in {this.GetType()}).");
            }

            return !((bool)currentValue).Equals(isPausedReadFromMemory);
        }

        private bool TimeSinceLastValueChangeIsGreaterThanMinUpdateTime()
        {
            return this.TimeSinceLastValueChange().CompareTo(this.MinUpdateTime) > 0;
        }

        private TimeSpan TimeSinceLastValueChange()
        {
            return DateTime.Now - this.timeOfLastValueChange;
        }
    }
}
