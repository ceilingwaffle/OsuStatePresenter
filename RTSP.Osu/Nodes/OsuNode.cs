using RTSP.Core;
using OsuMemoryDataProvider;


namespace RTSP.Osu.Nodes
{
    abstract class OsuNode : Node
    {
        protected IOsuMemoryReader _memoryReader;

        public OsuNode()
        {
            OsuMemoryDataProvider.DataProvider.Initalize();
            _memoryReader = OsuMemoryDataProvider.DataProvider.Instance;
        }
    }
}
