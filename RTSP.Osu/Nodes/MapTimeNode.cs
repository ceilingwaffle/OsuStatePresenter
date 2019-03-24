using RTSP.Core;
using System;
using System.Threading.Tasks;

namespace RTSP.Osu.Nodes
{
    [StateProperty(enabled: true, name: "MapTime")]
    class MapTimeNode : Node
    {
        public override async Task<object> DetermineValueAsync()
        {
            await Task.Delay(TimeSpan.FromMilliseconds(1000));

            return await Task.FromResult(180);
        }
    }
}
