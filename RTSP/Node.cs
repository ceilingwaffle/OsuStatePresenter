using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTSP
{
    public abstract class Node
    {
        private readonly Dictionary<Type, Node> _children;
        private readonly Dictionary<Type, Node> _parents;

        public Node()
        {
            _children = new Dictionary<Type, Node>();
            _parents = new Dictionary<Type, Node>();
        }

        public void AddChildren(params Node[] nodes)
        {
            foreach (var node in nodes)
            {
                if (node == null)
                    throw new ArgumentNullException(node.GetType().ToString());

                _children[node.GetType()] = node;
                node._parents[this.GetType()] = this;
            }
        }

        internal bool HasChildren()
        {
            return _children.Count > 0;
        }

        internal bool HasParents()
        {
            return _parents.Count > 0;
        }
    }
}
