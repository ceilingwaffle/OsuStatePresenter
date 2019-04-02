using DVPF.Core;
using OsuMemoryDataProvider;
using System.Threading;
using System.Reflection;
using System;

namespace OsuStatePresenter.Nodes
{
    abstract class OsuNode : Node
    {
        protected static readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();
        protected IOsuMemoryReader _memoryReader;
        //protected IOsuMemoryReader _memoryReader2;

        public OsuNode()
        {
            // using dll
            //OsuMemoryDataProvider.DataProvider.Initalize();
            //_memoryReader = OsuMemoryDataProvider.DataProvider.Instance;

            // using project reference
            //_memoryReader = new OsuMemoryReader(); // multi-instance
            _memoryReader = OsuMemoryReader.Instance; // shared instance
        }

    }

}
