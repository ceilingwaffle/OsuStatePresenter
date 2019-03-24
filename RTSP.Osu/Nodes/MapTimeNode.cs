using RTSP.Core;
using System;
using System.Threading.Tasks;

namespace RTSP.Osu.Nodes
{
    [StateProperty(enabled: true, name: "MapTime")]
    class MapTimeNode : OsuNode
    {
        public override async Task<object> DetermineValueAsync()
        {
            int mapTime = _memoryReader.ReadPlayTime();

            return await Task.FromResult(mapTime);
        }
    }
}
