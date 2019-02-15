using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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

        public async Task StartAsync()
        {
            NodeSupervisor.BuildLeafNodes();

            var cts = new CancellationTokenSource();

            while (true)
            {
                if (cts.IsCancellationRequested)
                    return;

                //var masterNodes = NodeSupervisor.MasterNodes.ToList().Select((n) => { return n.Value; });

                //foreach (var node in masterNodes)
                //{
                //    // TODO

                //    if (node.GetUpdateTaskStatus() != TaskStatus.Running)
                //    {
                //        Console.WriteLine($"{node.GetType().ToString()}.Update() RESTART...");
                //        node.DisposeUpdateTask();
                //        await node.UpdateAsync();
                //    }
                //    else
                //    {
                //        Console.WriteLine($"{node.GetType().ToString()}.Update() is already running...");
                //    }

                //}

                var leafNodes = NodeSupervisor.LeafNodes.ToList().Select((n) => { return n.Value; });

                foreach (var node in leafNodes)
                {
                    // TODO

                    if (node.GetUpdateTaskStatus() != TaskStatus.Running)
                    {
                        Console.WriteLine($"{node.GetType().ToString()}.Update() RESTART...");
                        
                        await node.UpdateAsync();
                    }
                    else
                    {
                        Console.WriteLine($"{node.GetType().ToString()}.Update() is already running...");
                    }

                }

                Console.WriteLine("----------------------");
                await Task.Delay(_scannerInterval);
            }

        }
    }
}
