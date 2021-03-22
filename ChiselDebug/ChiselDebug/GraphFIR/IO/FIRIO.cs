using System;
using System.Collections.Generic;
using System.Text;

namespace ChiselDebug.GraphFIR.IO
{
    public abstract class FIRIO : IContainerIO
    {
        public string Name { get; private set; }
        public bool IsAnonymous => Name == string.Empty;
        public AggregateIO ParentIO { get; private set; } = null;
        public bool IsPartOfAggregateIO => ParentIO != null;

        public FIRIO(string name)
        {
            this.Name = name;
        }

        public void SetName(string name)
        {
            Name = name ?? string.Empty;
        }

        public void SetParentIO(AggregateIO aggIO)
        {
            ParentIO = aggIO;
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

        public abstract void ConnectToInput(FIRIO input, bool allowPartial = false, bool asPassive = false);
        public abstract FIRIO Flip(FIRRTLNode node = null);
        public abstract FIRIO Copy(FIRRTLNode node = null);
        public abstract IEnumerable<ScalarIO> Flatten();
        public abstract bool IsPassiveOfType<T>();
        public abstract bool SameIO(FIRIO other);
        public abstract bool TryGetIO(string ioName, bool modulesOnly, out IContainerIO container);

        public IContainerIO GetIO(string ioName, bool modulesOnly = false)
        {
            if (TryGetIO(ioName, modulesOnly, out var container))
            {
                return container;
            }

            throw new Exception($"Failed to find io. IO name: {ioName}");
        }
    }
}
