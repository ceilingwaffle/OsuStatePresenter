using System.Threading.Tasks;
using RTSP.Core;
using System;

namespace RTSP.Osu.Nodes
{
    [StateProperty(enabled: true, name: "Mods")]
    class ModsNode : OsuNode
    {
        public override async Task<object> DetermineValueAsync()
        {
            int mods = _memoryReader.GetMods();

            return await Task.FromResult(mods);
        }
    }
}
