namespace OsuStatePresenter.Nodes
{
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading.Tasks;

    using BMAPI.v1;

    using DVPF.Core;

    /// <inheritdoc />
    /// <summary>
    /// The node representing whether or not the current time of the beatmap is at the beginning of the map.
    /// The "map start" is defined as any period of time in the beatmap before the first hit object.	
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.SpacingRules", "SA1027:TabsMustNotBeUsed", Justification = "Reviewed. Suppression is OK here.")]
    [StateProperty(enabled: true, name: "IsMapStart", strictValue: false)]
    public class MapStartNode : OsuNode
    {
        /// <inheritdoc />
        /// <summary>
        /// Returns a boolean wrapped in an object for whether or not the map is currently at the start.
        /// </summary>
        /// <returns>
        /// The <see cref="T:System.Threading.Tasks.Task" />.
        /// </returns>
        public override async Task<object> DetermineValueAsync()
        {
            // this.Preceders.TryGetValue(typeof(MapTimeNode), out Node mapTimeNode);
            this.Preceders.TryGetValue(typeof(BeatmapNode), out Node beatmapNode);

            // TODO: OPTIMIZE - cache the beatmap instead of loading it every time. only load when map ID changes (SEE MapBreakNode - extract out the caching stuff into a BeatmapCacher class).
            var beatmap = (BMAPI.v1.Beatmap)beatmapNode?.GetValue();

            if (beatmap == null)
            {
                return null;
            }

            // TODO: BUG - Fix bug
            // Temp fix 
            // Get the map time from the memory reader instead of MapTimeNode so that we prevent the
            // flicker between "playing" and "map start" status during the first second of playing a map.
            // (better but still slight flicker at start of map sometimes)
            int currentMapTime = this.MemoryReader.ReadPlayTime(); // (int?)mapTimeNode?.GetValue();

            // if ((int?)mapTimeNode?.GetValue() == null) return null;
            float mapFirstObjectTime = ((BeatmapNode)beatmapNode).GetTimeOfFirstHitObject(beatmap);
            //System.Diagnostics.Debug.WriteLine($"First object time: {mapFirstObjectTime}");
            bool isMapStart = currentMapTime < mapFirstObjectTime;
            //Debug.WriteLine($"{currentMapTime} -- {mapFirstObjectTime}");

            return await Task.FromResult(isMapStart);
        }
    }
}
