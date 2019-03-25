using System;
using System.CodeDom;
using System.Threading.Tasks;
using RTSP.Core;

namespace RTSP.Osu.Nodes
{
    [StateProperty(enabled: true, name: "Mods")]
    class ModsNode : OsuNode
    {
        public override async Task<object> DetermineValueAsync()
        {
            // TODO: Win32Exception not being caught from OsuMemory DLL when using osu Cutting Edge version
            int modsBitwise = _memoryReader.GetMods();
            var mods = ((Mods)modsBitwise).ToString();

            return await Task.FromResult(mods);
        }
    }
}
