using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTSP
{
    public class NodeSupervisor
    {
        public NodeSupervisor()
        {
            MasterNodes = new Dictionary<Type, Node>();
        }

        internal Dictionary<Type, Node> MasterNodes { get; private set; }

        public void AddMasterNodes(params Node[] nodes)
        {
            foreach (var node in nodes)
            {
                if (node.HasParents())
                    throw new Exception($"Node {node.GetType().ToString()} has at least one parent and is therefore not a master Node.");

                MasterNodes[node.GetType()] = node;
            }

        }


    }
}
