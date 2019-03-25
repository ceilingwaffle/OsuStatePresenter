using System.Threading.Tasks;
using RTSP.Core;

namespace RTSP.Osu.Nodes
{
    [StateProperty(enabled: true, name: "Mods")]
    class ModsNode : OsuNode
    {
        public override async Task<object> DetermineValueAsync()
        {
            //int modsBitwise = _memoryReader.GetMods();

            //var mods = ((Mods)modsBitwise).ToString();

            var mods = "HDHR";

            return await Task.FromResult(mods);
        }
    }
}
