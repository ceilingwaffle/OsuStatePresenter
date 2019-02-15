using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTSP
{
    public class NodeSupervisor
    {
        private readonly Dictionary<Type, Node> _masterNodes;

        public NodeSupervisor()
        {
            _masterNodes = new Dictionary<Type, Node>();
        }

        public void AddMasterNodes(params Node[] nodes)
        {
            foreach (var node in nodes)
            {
                if (node.HasParents())
                    throw new Exception($"Node {node.GetType().ToString()} has at least one parent and is therefore not a master Node.");

                _masterNodes[node.GetType()] = node;
            }

        }


    }
}
