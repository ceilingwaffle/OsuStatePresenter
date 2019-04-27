namespace OsuStatePresenter.Nodes
{
    using System;
    using System.Diagnostics;
    using System.Threading.Tasks;

    using DVPF.Core;

    /// <inheritdoc />
    /// <summary>
    /// The node representing the current time of the current beatmap.
    /// </summary>
    [StateProperty(enabled: true, name: "MapTime", strictValue: true)]
    public class MapTimeNode : OsuNode
    {
        /// <summary>
        /// Used to simulate the current map time, until the _minTime time is reached.
        /// </summary>
        private readonly Stopwatch timer = new Stopwatch();

        /// <summary>
        /// The minimum time for the timer to wait before using _memoryReader.ReadPlayTime()
        /// </summary>
        private readonly TimeSpan minTime = TimeSpan.FromMilliseconds(value: 1000);
        
        /// <summary>
        /// The previous map time delivered as a return value of DetermineValueAsync().
        /// </summary>
        private int lastTimeReturned = -1;
        
        /// <summary>
        /// The previous map time read from memory.
        /// </summary>
        private int lastTimeRead = -1;

        /// <inheritdoc />
        /// <summary>
        /// Returns the map time wrapped in an object.
        /// </summary>
        /// <returns>
        /// The <see cref="T:System.Threading.Tasks.Task" />.
        /// </returns>
        public override async Task<object> DetermineValueAsync()
        {
            // return await Task.FromResult(_memoryReader.ReadPlayTime());

            // The timer will "simulate" the current time until the _minTime time is reached. 
            // (e.g. estimate until 1 second is reached, then use the time read from the memory reader).
            // 
            // This is to prevent situations where the memory reader is slow (where the SP scanner-interval-rate is faster
            // than the memory-read-rate), resulting in an inaccurate back-and-forth alternation of IsPaused=true/false values.
            // 
            // This comes at the cost of the "current time" sometimes not being accurate, if the user has toggled pause/play,
            // within a maximum time of the _minTime value (e.g. 1 second). It will always be accurate after _minTime ms.

            // TODO: Idea to prevent inaccurate time being returned for up to t=_minTime - listen for status changed to "playing", if old status != playing and new status == playing, read map time from memory
            if (!this.timer.IsRunning)
            {
                this.timer.Start();

                if (this.lastTimeReturned == -1)
                {
                    return await Task.FromResult(-1);
                }
            }

            // int mapTime = _lastTimeRead;

            // TODO: BUG - System.IndexOutOfRangeException not being caught from OsuMemory DLL (occurs when game not running)
            int mapTime = this.MemoryReader.ReadPlayTime();

            if (this.timer.Elapsed.CompareTo(this.minTime) > 0) 
            {
                // timer time greater than min time
                this.timer.Restart();
                this.lastTimeRead = mapTime;
            }
            else if (this.lastTimeRead > mapTime)
            {
                mapTime = this.timer.Elapsed.Milliseconds + this.lastTimeRead;
            }

            // Need to delay by some small time to prevent the current timer-read value sometimes being equal to the last (resulting in fast alternation between IsPaused=true/false)
            await Task.Delay(millisecondsDelay: 2);

            this.lastTimeReturned = mapTime;

            return await Task.FromResult(mapTime);
        }
    }
}
