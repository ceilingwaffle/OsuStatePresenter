using System.Threading.Tasks;
using RTSP.Core;
using System;

namespace RTSP.Osu.Nodes
{
    [StateProperty(enabled: false, name: "Mods")]
    class ModsNode : Node
    {
        public override async Task<object> DetermineValueAsync()
        {
            await Task.Delay(TimeSpan.FromMilliseconds(1000));

            var value = Helpers.RandomStringFrom("HDHR", "nomod");

            return await Task.FromResult("HDHR");
        }
    }
}
