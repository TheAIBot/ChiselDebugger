using ChiselDebug.GraphFIR.Components;
using System;
using System.Collections.Generic;

namespace ChiselDebug.GraphFIR.IO
{
    public abstract class FIRIO : IContainerIO
    {
        public readonly FIRRTLNode Node;
        public string Name { get; private set; }
        public bool IsAnonymous => Name == null;
        public AggregateIO ParentIO { get; private set; } = null;
        public bool IsPartOfAggregateIO => ParentIO != null;

        public FIRIO(FIRRTLNode node, string name)
        {
            this.Node = node;
            this.Name = name;
        }

        public void SetName(string name)
        {
            Name = name;
        }

        public void SetParentIO(AggregateIO aggIO)
        {
            ParentIO = aggIO;
        }

        public Module GetModResideIn()
        {
            if (Node is Module mod)
            {
                return mod;
            }

            return Node.ResideIn;
        }

        public string GetFullName()
        {
            List<string> pathToRoot = new List<string>();
            if (Name != null)
            {
                pathToRoot.Add(Name);
            }

            FIRIO node = this;
            while (node.ParentIO != null)
            {
                node = node.ParentIO;
                if (Name != null)
                {
                    pathToRoot.Add(node.Name);
                }
            }

            pathToRoot.Reverse();
            return string.Join('.', pathToRoot);
        }

        public virtual FIRIO GetInput()
        {
            throw new Exception("Can't get input from this IO.");
        }
        public virtual FIRIO GetOutput()
        {
            throw new Exception("Can't get output from this IO.");
        }

        internal FIRIO GetAsFlow(FlowChange flow)
        {
            return flow switch
            {
                FlowChange.Source => GetOutput(),
                FlowChange.Sink => GetInput(),
                var error => throw new Exception($"Flow must either be source or sink. Flow: {error}")
            };
        }

        public abstract void ConnectToInput(FIRIO input, bool allowPartial = false, bool asPassive = false, Output condition = null);
        public abstract FIRIO ToFlow(FlowChange flow, FIRRTLNode node);
        public FIRIO Flip(FIRRTLNode node = null)
        {
            return ToFlow(FlowChange.Flipped, node ?? Node);
        }
        public FIRIO Copy(FIRRTLNode node = null)
        {
            return ToFlow(FlowChange.Preserve, node ?? Node);
        }
        public List<ScalarIO> Flatten()
        {
            return Flatten(new List<ScalarIO>());
        }
        public abstract List<ScalarIO> Flatten(List<ScalarIO> list);
        public abstract bool IsPassiveOfType<T>();
        public bool IsPassive()
        {
            return IsPassiveOfType<Input>() || IsPassiveOfType<Output>();
        }
        public abstract bool TryGetIO(string ioName, out IContainerIO container);

        public IContainerIO GetIO(string ioName)
        {
            if (TryGetIO(ioName, out var container))
            {
                return container;
            }

            throw new Exception($"Failed to find io. IO name: {ioName}");
        }
    }
}
