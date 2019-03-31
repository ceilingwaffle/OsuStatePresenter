using System.Threading.Tasks;
using DVPF.Core;
using System;

namespace OsuStatePresenter.Nodes
{
    // TODO: Build the "paused" status into the custom Status class
    [StateProperty(enabled: true, name: "SongIsPaused")]
    class PausedNode : OsuNode
    {
        public override async Task<object> DetermineValueAsync()
        {
            // TODO: Use a 1 second timer to delay the checking of the current/previous MapTime, to fix the "difference between 2 states MapTime" causing "flicker" between IsPaused=true/false states

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
