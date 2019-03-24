using RTSP.Core;
using System.Threading.Tasks;

namespace RTSP.Osu.Nodes
{
    [StateProperty(enabled: true, name: "MapID")]
    class MapIdNode : Node
    {
        public override async Task<object> DetermineValueAsync()
        {
            var value = Helpers.Rand(1, 1000);

            return await Task.FromResult(value);
        }
    }
}
