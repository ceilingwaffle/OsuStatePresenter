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

        public StatePresenter()
        {
            NodeSupervisor = new NodeSupervisor();
        }

        public void Start()
        {
            // TODO
        }
    }
}
