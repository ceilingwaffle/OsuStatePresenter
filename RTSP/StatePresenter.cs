using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTSP
{
    public class StatePresenter
    {
        public NodeSupervisor NodeSupervisor { get; private set; }
        private TimeSpan _scannerInterval = TimeSpan.FromMilliseconds(5000);

        public StatePresenter()
        {
            NodeSupervisor = new NodeSupervisor();
        }

        public async Task Start()
        {
            while (true)
            {
                var masterNodes = NodeSupervisor.MasterNodes.ToList().Select((n) => { return n.Value; });

                foreach (var node in masterNodes)
                {
                    // TODO
                    Console.WriteLine(node.GetType().ToString());
                }

                await Task.Delay(_scannerInterval);
            }

        }
    }
}
