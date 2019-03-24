using System.Threading.Tasks;
using RTSP.Core;
using System;

namespace RTSP.Osu.Nodes
{
    [StateProperty(enabled: false, name: "RedLinesTimingPoints")]
    class RedLinesNode : OsuNode
    {
        public override async Task<object> DetermineValueAsync()
        {
            await Task.Delay(TimeSpan.FromMilliseconds(100));

            return await Task.FromResult("TODO: RedLinesTimingPoints object");
        }
    }
}
