using DVPF.Core;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace OsuStatePresenter.Nodes
{
    [StateProperty(enabled: true, name: "MapTime", strictValue: true)]
    class MapTimeNode : OsuNode
    {
        /// <summary>
        /// Used to simulate the current map time, until the _minTime time is reached.
        /// </summary>
        private readonly Stopwatch _timer = new Stopwatch();
        /// <summary>
        /// The minimum time for the timer to wait before using _memoryReader.ReadPlayTime()
        /// </summary>
        private TimeSpan _minTime = TimeSpan.FromMilliseconds(1000);
        /// <summary>
        /// The previous map time delivered as a return value of DetermineValueAsync().
        /// </summary>
        private int _lastTimeReturned = -1;
        /// <summary>
        /// The previous map time read from memory.
        /// </summary>
        private int _lastTimeRead = -1;

        public override async Task<object> DetermineValueAsync()
        {
            // The timer will "simulate" the current time until the _minTime time is reached. 
            // (e.g. estimate until 1 second is reached, then use the time read from the memory reader).
            //
            // This is to prevent situations where the memory reader is slow (where the SP scanner-interval-rate is faster
            // than the memory-read-rate), resulting in an inaccurate back-and-forth alternation of IsPausued=true/false values.
            //
            // This comes at the cost of the "current time" sometimes not being accurate, if the user has toggled pause/unpause,
            // within a maximum time of the _minTime value (e.g. 1 second). It will always be accurate after _minTime ms.

            if (!_timer.IsRunning)
            {
                _timer.Start();

                if (_lastTimeReturned == -1)
                {
                    return await Task.FromResult(-1);
                }
            }

            int mapTime = _lastTimeRead;

            // TODO: System.IndexOutOfRangeException not being caught from OsuMemory DLL (occurs when game not running)
            mapTime = _memoryReader.ReadPlayTime();

            if (_timer.Elapsed.CompareTo(_minTime) > 0) // timer time greater than min time
            {
                _timer.Restart();
                _lastTimeRead = mapTime;
            }
            else if (_lastTimeRead > mapTime)
            {
                mapTime = _timer.Elapsed.Milliseconds + _lastTimeRead;
            }

            // Need to delay by some small time to prevent the current timer-read value sometimes being equal to the last (resulting in fast alternation between IsPaused=true/false)
            await Task.Delay(2);

            _lastTimeReturned = mapTime;

            return await Task.FromResult(mapTime);
        }
    }
}
