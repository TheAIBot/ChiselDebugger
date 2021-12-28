using System;
using System.Collections.Generic;
using System.Linq;

namespace ChiselDebug.GraphFIR.IO
{
    public abstract class AggregateIO : FIRIO 
    {
        private HashSet<AggregateIO> Connections = null;
        public AggregateIO(FIRRTLNode node, string name) : base(node, name) { }

        public override FIRIO GetInput()
        {
            return this;
        }

        public override FIRIO GetOutput()
        {
            return this;
        }

        public abstract FIRIO[] GetIOInOrder();

        public override void ConnectToInput(FIRIO input, bool allowPartial = false, bool asPassive = false, Output condition = null)
        {
            var aggIO = input as AggregateIO;
            AddConnection(aggIO);
            aggIO.AddConnection(this);
        }

        public void AddConnection(AggregateIO aggIO)
        {
            if (Connections == null)
            {
                Connections = new HashSet<AggregateIO>();
            }

            Connections.Add(aggIO);
        }

        public void ReplaceConnection(AggregateIO replace, AggregateIO replaceWith)
        {
            if (!replace.IsPassiveOfType<Output>())
            {
                throw new ArgumentException(nameof(replace), "IO to replace must be passive of type output");
            }

            if (!replaceWith.IsPassiveOfType<Output>())
            {
                throw new ArgumentException(nameof(replaceWith), "IO to replace with must be passive of type output");
            }

            if (Connections == null)
            {
                throw new InvalidOperationException("can't replace ana aggregates connection if it has no connections.");
            }

            if (!Connections.Contains(replace))
            {
                throw new ArgumentOutOfRangeException(nameof(replace), "This aggregate io is not connection to the io that will be replaced.");
            }

            if (replace.GetType() != replaceWith.GetType())
            {
                throw new ArgumentException(nameof(replaceWith), "The io being replaced and what it's being replaced with must be ofthe same type.");
            }

            List<Input> currentScalars = Flatten().Cast<Input>().ToList();
            List<Output> replaceScalars = replace.Flatten().Cast<Output>().ToList();
            List<Output> replaceWithScalars = replaceWith.Flatten().Cast<Output>().ToList();
            if (replaceScalars.Count != replaceWithScalars.Count)
            {
                throw new ArgumentException($"The size of {nameof(replace)} and {nameof(replaceWith)} must be the same when replacing");
            }

            for (int i = 0; i < replaceScalars.Count; i++)
            {
                var connection = currentScalars[i].GetConnection(replaceScalars[i]).Value;
                currentScalars[i].ReplaceConnection(connection, replaceWithScalars[i]);
            }

            Connections.Remove(replace);
            Connections.Add(replaceWith);
            replace.Connections.Remove(this);
            replaceWith.AddConnection(this);
        }

        public AggregateIO[] GetConnections()
        {
            if (Connections == null)
            {
                return Array.Empty<AggregateIO>();
            }

            return Connections.ToArray();
        }
    }
}
