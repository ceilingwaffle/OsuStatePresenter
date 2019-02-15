using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTSP
{
    public class NodeSupervisor
    {
        /// <summary>
        /// Nodes not depending on values from any other Nodes.
        /// </summary>
        internal Dictionary<Type, Node> MasterNodes { get; private set; }
        /// <summary>
        /// Nodes having no Nodes depedent on their values.
        /// </summary>
        internal Dictionary<Type, Node> LeafNodes { get; private set; }

        public NodeSupervisor()
        {
            MasterNodes = new Dictionary<Type, Node>();
            LeafNodes = new Dictionary<Type, Node>();
        }

        public void AddMasterNodes(params Node[] nodes)
        {
            foreach (var node in nodes)
            {
                if (node.HasParents())
                    throw new Exception($"Node {node.GetType().ToString()} has at least one parent and is therefore not a master Node.");

                MasterNodes[node.GetType()] = node;
            }

        }

        internal void BuildLeafNodes()
        {
            // TODO: replace all these uses of Select with a custom Class for Dictionary<Type, Node> with a GetNodes() method
            var masterNodesList = MasterNodes.ToList().Select((n) => { return n.Value; });

            LeafNodes = RecursiveChildLeafGet(masterNodesList);
        }

        private Dictionary<Type, Node> RecursiveChildLeafGet(IEnumerable<Node> nodes)
        {
            var leafs = new Dictionary<Type, Node>();

            // TODO: Replace recursion with while loop or task? Possible stack overflow exception...
            foreach (var node in nodes)
            {
                if (! node.HasChildren())
                {
                    leafs[node.GetType()] = node;
                }
                else
                {
                    var children = node.Children.ToList().Select((n) => { return n.Value; });
                    var childrenLeafs = RecursiveChildLeafGet(children);

                    // merge
                    childrenLeafs.ToList().ForEach(n => leafs[n.Key] = n.Value);
                }

            }

            return leafs;
        }


    }
}
