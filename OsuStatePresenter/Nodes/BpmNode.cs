namespace OsuStatePresenter.Nodes
{
    using System.Threading.Tasks;

    using DVPF.Core;

    /// <inheritdoc />
    /// <summary>
    /// The node representing the current BPM of the beatmap, relative to the current beatmap time.
    /// </summary>
    [StateProperty(enabled: true, name: "CurrentBPM")]
    public class BpmNode : OsuNode
    {
        /// <inheritdoc />
        /// <summary>
        /// Returns the current BPM wrapped in an object.
        /// </summary>
        /// <returns>
        /// The <see cref="T:System.Threading.Tasks.Task" />.
        /// </returns>
        public override async Task<object> DetermineValueAsync()
        {
            this.Preceders.TryGetValue(typeof(MapTimeNode), out Node mapTimeNode);
            this.Preceders.TryGetValue(typeof(ModsNode), out Node modsNode);
            this.Preceders.TryGetValue(typeof(BeatmapNode), out Node beatmapNode);

            if (beatmapNode is null)
            {
                return null;
            }

            int mapTime = (int?)mapTimeNode?.GetValue() ?? -1;
            string mods = (string)modsNode?.GetValue() ?? string.Empty;
            var beatmap = (BMAPI.v1.Beatmap)beatmapNode.GetValue();

            float bpm = CalculateBpm(mapTime, beatmap, mods);

            // unknown bpm
            if (bpm <= 0)
            {
                return null;
            }

            return await Task.FromResult(bpm);
        }

        private static float ConvertDelayToBpm(float bpmDelay)
        {
            // 60,000ms = 1 minute
            return 60000f / bpmDelay;
        }

        // ReSharper disable once SuggestBaseTypeForParameter
        private static float CalculateBpm(int mapTime, BMAPI.v1.Beatmap beatmap, string mods)
        {
            // TODO: OPTIMIZE - Probably a faster way of doing this instead of Reverse()? e.g. using a temp variable for the previous tp.
            // https://osu.ppy.sh/help/wiki/osu!_File_Formats/Osu_(file_format)#timing-points
            var bpm = 0.0f;

            if (beatmap is null)
            {
                return bpm;
            }

            var timingPoints = beatmap.TimingPoints;
            timingPoints.Reverse();

            foreach (BMAPI.v1.TimingPoint tp in timingPoints)
            {
                if (tp?.InheritsBPM != false || !(mapTime >= tp.Time))
                {
                    continue;
                }

                bpm = ConvertDelayToBpm(tp.BpmDelay);
                break;
            }

            // TODO: UNFINISHED - More accurate check for DT/HT - something other than substring search
            if (mods.Contains("DoubleTime"))
            {
                bpm *= 1.5f;
            }
            else if (mods.Contains("HalfTime"))
            {
                bpm *= 0.75f;
            }

            return bpm;
        }
    }
}
