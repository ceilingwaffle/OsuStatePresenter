using System.Threading.Tasks;
using DependentValuePresentationFramework;
using System;

namespace OsuStatePresenter.Nodes
{
    [StateProperty(enabled: false, name: "Beatmap")]
    class BeatmapNode : OsuNode
    {
        private readonly string _osuSongsFolderPath = @"G:\osu!\Songs\";

        public override async Task<object> DetermineValueAsync()
        {
            Preceders.TryGetValue(typeof(MapIdNode), out Node mapIdNode);

            if (mapIdNode is null)
                return null;

            string mapFolderName = _memoryReader.GetMapFolderName();
            string mapFileName = _memoryReader.GetOsuFileName();
            string fullMapFilePath = BuildMapPathString(mapFolderName, mapFileName);
            BMAPI.v1.Beatmap beatmap = BuildBeatmapFromFile(fullMapFilePath);

            return await Task.FromResult(beatmap);
        }

        private string BuildMapPathString(string mapFolderName, string mapFileName)
        {
            return string.Concat(_osuSongsFolderPath, mapFolderName, @"\", mapFileName);
        }

        private BMAPI.v1.Beatmap BuildBeatmapFromFile(string fullMapFilePath)
        {
            // TODO: Build decorated custom-beatmap class object (to reduce external class coupling to BMAPI).
            return new BMAPI.v1.Beatmap(fullMapFilePath);
        }
    }
}
