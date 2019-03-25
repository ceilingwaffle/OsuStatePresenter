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
            // TODO: One Node can hang another Node because they share the same OsuMemoryDataProvider instance - and the Node must wait for the reader to become "available" before it can read.

            OsuMemoryDataProvider.DataProvider.Initalize();
            _memoryReader = OsuMemoryDataProvider.DataProvider.Instance;
        }
    }
}
