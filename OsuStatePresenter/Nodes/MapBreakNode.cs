using BMAPI.v1;
using DVPF.Core;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OsuStatePresenter.Nodes
{
    [StateProperty(enabled: true, name: "IsMapBreak")]
    public class MapBreakNode : OsuNode
    {
        private int? _cachedMapId = null;
        private List<BMAPI.v1.Events.EventBase> _cachedBeatmapEvents = new List<BMAPI.v1.Events.EventBase>();

        public override async Task<object> DetermineValueAsync()
        {
            Preceders.TryGetValue(typeof(MapTimeNode), out var mapTimeNode);
            Preceders.TryGetValue(typeof(BeatmapNode), out var beatmapNode);

            if (beatmapNode is null)
                return null;

            var mapTime = (int?)mapTimeNode?.GetValue() ?? -1; 
            var beatmap = (BMAPI.v1.Beatmap)beatmapNode.GetValue();

            if (beatmap is null)
                return null;

            // only update the cache (and reverse the events) if the beatmap changed
            if (!beatmap.BeatmapID.Equals(_cachedMapId))
                UpdateMapCache(beatmap.BeatmapID, beatmap.Events);

            bool isMapBreak = IsMapBreak(mapTime, _cachedBeatmapEvents);

            return await Task.FromResult(isMapBreak);
        }

        private void UpdateMapCache(int? mapId, List<BMAPI.v1.Events.EventBase> beatmapEvents)
        {
            _cachedMapId = mapId;
            _cachedBeatmapEvents = beatmapEvents;
            _cachedBeatmapEvents.Reverse();
        }

        private bool IsMapBreak(int mapTime, List<BMAPI.v1.Events.EventBase> beatmapEvents)
        {
            foreach (var ev in beatmapEvents)
            {
                if (ev is BMAPI.v1.Events.BreakEvent breakEvent
                    && mapTime >= breakEvent.StartTime
                    && mapTime <= breakEvent.EndTime)
                {
                    return true;
                }
            }

            return false;
        }
    }
}