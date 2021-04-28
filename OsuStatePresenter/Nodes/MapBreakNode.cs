namespace OsuStatePresenter.Nodes
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using BMAPI.v1.Events;

    using DVPF.Core;
    using OsuParsers.Beatmaps;
    using OsuParsers.Beatmaps.Sections;

    /// <inheritdoc />
    /// <summary>
    /// The node representing whether the current time of the beatmap is in a map break (the grey area in the map editor).
    /// </summary>
    [StateProperty(enabled: true, name: "IsMapBreak")]
    public class MapBreakNode : OsuNode
    {
        private int? cachedMapId;
        private BeatmapEventsSection cachedBeatmapEvents = new BeatmapEventsSection();

        /// <inheritdoc />
        /// <summary>
        /// Returns a boolean wrapped in an object for whether or not the map is currently in a break.
        /// </summary>
        /// <returns>
        /// The <see cref="T:System.Threading.Tasks.Task" />.
        /// </returns>
        public override async Task<object> DetermineValueAsync()
        {
            this.Preceders.TryGetValue(typeof(MapTimeNode), out Node mapTimeNode);
            this.Preceders.TryGetValue(typeof(BeatmapNode), out Node beatmapNode);

            if (beatmapNode is null)
            {
                return null;
            }

            int mapTime = (int?)mapTimeNode?.GetValue() ?? -1; 
            var beatmap = (Beatmap)beatmapNode.GetValue();

            if (beatmap is null)
            {
                return null;
            }


            // only update the cache (and reverse the events) if the beatmap changed
            if (!beatmap.MetadataSection.BeatmapID.Equals(this.cachedMapId))
            {
                this.UpdateMapCache(beatmap.MetadataSection.BeatmapID, beatmap.EventsSection);
            }

            bool isMapBreak = IsMapBreak(mapTime, this.cachedBeatmapEvents);

            return await Task.FromResult(isMapBreak);
        }

        private static bool IsMapBreak(int mapTime, BeatmapEventsSection beatmapEvents)
        {
            foreach (var bmBreak in beatmapEvents.Breaks)
            {
                if (mapTime >= bmBreak.StartTime && mapTime <= bmBreak.EndTime)
                {
                    return true;
                }
            }

            return false;
        }

        private void UpdateMapCache(int? mapId, BeatmapEventsSection beatmapEvents)
        {
            this.cachedMapId = mapId;
            this.cachedBeatmapEvents = beatmapEvents;
            //this.cachedBeatmapEvents.Reverse();
        }
    }
}