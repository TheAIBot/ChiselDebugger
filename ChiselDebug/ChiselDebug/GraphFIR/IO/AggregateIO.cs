﻿using ChiselDebug.GraphFIR.Components;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ChiselDebug.GraphFIR.IO
{
    public record AggregateConnection(AggregateIO To, Output Condition);
    public abstract class AggregateIO : FIRIO 
    {
        private HashSet<AggregateConnection> Connections = null;
        public bool HasAggregateConnections => Connections?.Count > 0;
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
            AddConnection(aggIO, condition);
            aggIO.AddConnection(this, condition);
        }

        public void AddConnection(AggregateIO aggIO, Output condition)
        {
            if (Connections == null)
            {
                Connections = new HashSet<AggregateConnection>();
            }

            Connections.Add(new AggregateConnection(aggIO, condition));
        }

        public void RemoveConnection(AggregateIO aggIO, Output condition)
        {
            if (Connections == null)
            {
                throw new InvalidOperationException("Can't remove connections from an io that never had any connections.");
            }

            Connections.Remove(new AggregateConnection(aggIO, condition));
        }

        public void ReplaceConnection(AggregateIO replace, AggregateIO replaceWith, Output condition)
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
                throw new InvalidOperationException("can't replace an aggregates connection if it has no connections.");
            }

            if (!Connections.Contains(new AggregateConnection(replace, condition)))
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
                var connection = currentScalars[i].GetConnection(replaceScalars[i], condition);
                currentScalars[i].ReplaceConnection(connection, replaceWithScalars[i]);
            }

            RemoveConnection(replace, condition);
            AddConnection(replaceWith, condition);
            replace.RemoveConnection(this, condition);
            replaceWith.AddConnection(this, condition);
        }

        public AggregateConnection[] GetConnections()
        {
            if (Connections == null)
            {
                return Array.Empty<AggregateConnection>();
            }

            return Connections.ToArray();
        }

        public bool OnlyConnectedWithAggregateConnections()
        {
            if (!HasAggregateConnections)
            {
                return false;
            }

            int connectionCount = Connections.Count;
            foreach (var scalar in Flatten())
            {
                if (!scalar.IsConnectedToAnything())
                {
                    continue;
                }

                if (scalar is Input input && input.GetConnections().Length != connectionCount)
                {
                    return false;
                }
                else if (scalar is Output output && output.GetConnectedInputs().Count() != connectionCount)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
