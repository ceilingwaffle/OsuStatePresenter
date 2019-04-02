using System.Threading.Tasks;
using DVPF.Core;
using System;
using System.ServiceModel;

namespace OsuStatePresenter.Nodes
{
    [StateProperty(enabled: true, name: "CurrentBPM")]
    public class BpmNode : OsuNode
    {
        public override async Task<object> DetermineValueAsync()
        {
            Preceders.TryGetValue(typeof(MapTimeNode), out var mapTimeNode);
            Preceders.TryGetValue(typeof(ModsNode), out var modsNode);
            Preceders.TryGetValue(typeof(BeatmapNode), out var beatmapNode);

            if (beatmapNode is null)
                return null;

            var mapTime = (int?)mapTimeNode?.GetValue() ?? -1;
            var mods = (string)modsNode?.GetValue() ?? "";
            var beatmap = (BMAPI.v1.Beatmap)beatmapNode.GetValue();

            float bpm = CalculateBpm(mapTime, beatmap);

            // unknown bpm
            if (bpm <= 0)
                return null;

            return await Task.FromResult(bpm);
        }

        private float CalculateBpm(int mapTime, BMAPI.v1.Beatmap beatmap)
        {
            // TODO: Optimize? Probably a faster way of doing this instead of Reverse()? e.g. using a temp variable for the previous tp.
            // https://osu.ppy.sh/help/wiki/osu!_File_Formats/Osu_(file_format)#timing-points

            float bpm = 0.0f;

            if (beatmap is null)
                return bpm;

            var timingPoints = beatmap.TimingPoints;
            timingPoints.Reverse();

            foreach (var tp in timingPoints)
            {
                if (tp is null)
                    continue;

                if (tp.InheritsBPM == false && mapTime >= tp.Time)
                {
                    bpm = ConvertDelayToBpm(tp.BpmDelay);
                    break;
                }
            }

            return bpm;
        }

        private float ConvertDelayToBpm(float bpmDelay)
        {
            // 60,000ms = 1 minute
            return 60000f / bpmDelay;
        }
    }
}
