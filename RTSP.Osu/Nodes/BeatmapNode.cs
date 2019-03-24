using System.Threading.Tasks;
using RTSP.Core;
using System;

namespace RTSP.Osu.Nodes
{
    [StateProperty(enabled: false, name: "Beatmap")]
    class BeatmapNode : OsuNode
    {
        public override async Task<object> DetermineValueAsync()
        {
            await Task.Delay(TimeSpan.FromMilliseconds(100));

            var value = Helpers.RandomStringFrom("Beatmap A", "Beatmap B");

            return await Task.FromResult(value);
        }
    }
}
