using System.Threading.Tasks;
using DVPF.Core;

namespace OsuStatePresenter.Nodes
{
    /// <summary>
    /// The "map start" is defined as any period of time in the beatmap before the first hit object.
    /// </summary>
    [StateProperty(enabled: true, name: "IsMapStart", strictValue: true)]
    public class MapStartNode : OsuNode
    {
        public override async Task<object> DetermineValueAsync()
        {
            Preceders.TryGetValue(typeof(BeatmapNode), out Node beatmapNode);
            //Preceders.TryGetValue(typeof(MapTimeNode), out Node mapTimeNode);

            if (beatmapNode == null) // || mapTimeNode == null
                return null;

            var beatmap = (BMAPI.v1.Beatmap)beatmapNode.GetValue();

            if (beatmap == null)
                return null;

            // TODO: Fix bug
            // Temp fix 
            // Get the map time from the memory reader instead of MapTimeNode so that we prevent the
            // flicker between "playing" and "map start" status during the first second of playing a map.
            // (better but still slight flicker at start of map sometimes)
            var currentMapTime = _memoryReader.ReadPlayTime(); //(int?)mapTimeNode?.GetValue();

            //if ((int?)mapTimeNode?.GetValue() == null)
            //    return null;

            float mapFirstObjectTime = ((BeatmapNode)beatmapNode).GetTimeOfFirstHitObject(beatmap);
            System.Diagnostics.Debug.WriteLine($"First object time: {mapFirstObjectTime}");
            bool isMapStart = currentMapTime < mapFirstObjectTime;
            return await Task.FromResult(isMapStart);
        }
    }
}
