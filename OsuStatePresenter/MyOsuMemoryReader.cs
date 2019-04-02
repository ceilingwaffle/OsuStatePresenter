using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OsuMemoryDataProvider;

namespace OsuStatePresenter
{
    class MyOsuMemoryReader : OsuMemoryReader
    {
        protected static readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        public MyOsuMemoryReader()
        {
            _logger.Info("Creating new MyOsuMemoryReader.Instance...");
            Instance = new OsuMemoryReader();
        }

        public new IOsuMemoryReader Instance { get; }
    }
}
