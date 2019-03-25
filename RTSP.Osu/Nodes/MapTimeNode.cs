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
            // TODO: System.IndexOutOfRangeException not being caught from OsuMemory DLL (occurs when game not running)
            int mapTime = _memoryReader.ReadPlayTime();

            return await Task.FromResult(mapTime);
        }
    }
}
