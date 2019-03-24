using RTSP.Core;
using System.Threading.Tasks;

namespace RTSP.Osu.Nodes
{
    [StateProperty(enabled: true, name: "MapID")]
    class MapIdNode : OsuNode
    {
        public override async Task<object> DetermineValueAsync()
        {
            int mapId = _memoryReader.GetMapId();

            return await Task.FromResult(mapId);
        }
    }
}
