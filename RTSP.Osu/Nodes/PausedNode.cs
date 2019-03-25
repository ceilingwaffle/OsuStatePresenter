using System.Threading.Tasks;
using RTSP.Core;
using System;

namespace RTSP.Osu.Nodes
{
    // TODO: Build the "paused" status into the custom Status class
    [StateProperty(enabled: true, name: "IsPaused")]
    class PausedNode : OsuNode
    {
        public override async Task<object> DetermineValueAsync()
        {
            // TODO: Fix minimum "2 state" delay time for paused status to update

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
