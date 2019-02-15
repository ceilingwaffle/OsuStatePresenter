using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RTSP
{
    public abstract class Node
    {
        private List<object> _valueLedger;

        private Task _updateTask;
        private TimeSpan _updateTimeLimit = TimeSpan.FromSeconds(30);

        public Dictionary<Type, Node> Children { get; }
        public Dictionary<Type, Node> Parents { get; }

        public Node()
        {
            _valueLedger = new List<object>(2);
            Children = new Dictionary<Type, Node>();
            Parents = new Dictionary<Type, Node>();
        }

        public void AddChildren(params Node[] nodes)
        {
            foreach (var node in nodes)
            {
                if (node == null)
                    throw new ArgumentNullException(node.GetType().ToString());

                Children[node.GetType()] = node;
                node.Parents[this.GetType()] = this;
            }
        }

        internal bool HasChildren()
        {
            return Children.Count > 0;
        }

        internal bool HasParents()
        {
            return Parents.Count > 0;
        }

        internal async Task UpdateAsync()
        {
            await GetUpdateTask();

            Console.WriteLine($"Node._updateTask.Status = {_updateTask.Status.ToString()} ({this.GetType().ToString()}).");

            this.DisposeUpdateTask();
        }

        private Task GetUpdateTask()
        {
            if (_updateTask == null)
            {
                var cts = new CancellationTokenSource(_updateTimeLimit);

                _updateTask = Task.Run(() =>
                {
                    Console.WriteLine($"TODO: FetchData() + CalculateValue() ({this.GetType().ToString()}).");

                    _SetValue(Helpers.UnixTimestamp());

                    if (_ValueChanged())
                    {
                        // TODO: Issue cancel to all children (if IsCancelled -> DisposeUpdateTask())
                        Console.WriteLine($"Value changed ({this.GetType().ToString()}) ({GetPreviousValue()} -> {GetValue()}).");
                    }

                    var parents = Parents.ToList().Select((n) => { return n.Value; });
                    Parallel.ForEach(parents, async parent =>
                    {
                        await parent.GetUpdateTask();
                    });

                }, cts.Token);
            }

            return _updateTask;
        }

        internal TaskStatus GetUpdateTaskStatus()
        {
            if (_updateTask == null)
            {
                // TODO: Something else like a custom UpdateTaskStatus class which extends TaskStatus but has a custom null status.
                return TaskStatus.WaitingToRun;
            }

            return _updateTask.Status;
        }

        internal void DisposeUpdateTask()
        {
            Console.WriteLine($"DisposeUpdateTask() for {this.GetType().ToString()}.");
            _updateTask = null;
        }

        private void _SetValue(object v)
        {
            _valueLedger.Insert(0, v);
        }

        public object GetValue()
        {
            return GetPreviousValue(age: 0);
        }

        public object GetPreviousValue()
        {
            return GetPreviousValue(age: 1);
        }

        internal object GetPreviousValue(int age)
        {
            if (_valueLedger.Count <= age)
            {
                return default(object);
            }

            return _valueLedger.ElementAt(age);
        }

        private bool _ValueChanged()
        {
            return ! GetPreviousValue(age: 0).Equals(GetPreviousValue(age: 1));
        }
    }
}
