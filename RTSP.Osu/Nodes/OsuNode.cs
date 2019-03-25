using RTSP.Core;
using OsuMemoryDataProvider;
using System.Threading;

namespace RTSP.Osu.Nodes
{
    abstract class OsuNode : Node
    {
        protected static readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();
        protected IOsuMemoryReader _memoryReader;

        public OsuNode()
        {
            OsuMemoryDataProvider.DataProvider.Initalize();
            _memoryReader = OsuMemoryDataProvider.DataProvider.Instance;
        }
    }
}
