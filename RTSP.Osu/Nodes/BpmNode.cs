using System.Threading.Tasks;
using RTSP.Core;
using System;

namespace RTSP.Osu.Nodes
{
    [StateProperty(enabled: false, name: "CurrentBPM")]
    class BpmNode : OsuNode
    {
        public override async Task<object> DetermineValueAsync()
        {
            await Task.Delay(TimeSpan.FromMilliseconds(100));

            Preceders.TryGetValue(typeof(MapTimeNode), out var mapTimeNode);
            Preceders.TryGetValue(typeof(ModsNode), out var modsNode);
            Preceders.TryGetValue(typeof(RedLinesNode), out var redLinesNode);

            var mapTime = mapTimeNode.GetValue();
            var mods = modsNode.GetValue();
            var redLineTimingPoints = redLinesNode.GetValue();

            return await Task.FromResult(200);
        }
    }
}
