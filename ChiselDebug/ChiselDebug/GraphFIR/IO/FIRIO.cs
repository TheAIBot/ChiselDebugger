using System;
using System.Collections.Generic;
using System.Text;

namespace ChiselDebug.GraphFIR.IO
{
    public abstract class FIRIO : IContainerIO
    {
        public readonly FIRRTLNode Node;
        public string Name { get; private set; }
        public bool IsAnonymous => Name == null;
        public AggregateIO ParentIO { get; private set; } = null;
        public bool IsPartOfAggregateIO => ParentIO != null;
        private FIRIO Paired;

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
            return Node.ResideIn;
        }

        public string GetFullName()
        {
            List<string> pathToRoot = new List<string>();
            pathToRoot.Add(Name);

            FIRIO node = this;
            while (node.ParentIO != null)
            {
                node = node.ParentIO;
                pathToRoot.Add(node.Name);
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

        internal FIRIO GetAsGender(IOGender gender)
        {
            return gender switch
            {
                IOGender.Male => GetOutput(),
                IOGender.Female => GetInput(),
                IOGender.BiGender => this,
                var error => throw new Exception($"Invalid gender. Gender: {error}")
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
        public abstract IEnumerable<ScalarIO> Flatten();
        public abstract List<ScalarIO> Flatten(List<ScalarIO> list);
        public abstract bool IsPassiveOfType<T>();
        public bool IsPassive()
        {
            return IsPassiveOfType<Input>() || IsPassiveOfType<Output>();
        }
        public abstract bool SameIO(FIRIO other);
        public abstract IEnumerable<T> GetAllIOOfType<T>();
        public abstract List<T> GetAllIOOfType<T>(List<T> list);
        public abstract IEnumerable<FIRIO> WalkIOTree();
        public abstract bool TryGetIO(string ioName, bool modulesOnly, out IContainerIO container);

        public IContainerIO GetIO(string ioName, bool modulesOnly = false)
        {
            if (TryGetIO(ioName, modulesOnly, out var container))
            {
                return container;
            }

            throw new Exception($"Failed to find io. IO name: {ioName}");
        }

        public void SetPaired(FIRIO paired)
        {
            Paired = paired;
        }

        public FIRIO GetPaired()
        {
            return Paired;
        }
    }
}
