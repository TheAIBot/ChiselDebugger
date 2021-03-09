using FIRRTL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ChiselDebug.GraphFIR
{
    public interface IContainerIO
    {
        public IContainerIO GetIO(string ioName);
    }

    public abstract class FIRIO : IContainerIO
    {
        public string Name { get; private set; }
        public IOBundle Bundle { get; private set; } = null;
        public bool IsPartOfBundle => Bundle != null;

        public FIRIO(string name)
        {
            this.Name = name;
        }

        public void SetName(string name)
        {
            Name = name;
        }

        public void SetBundle(IOBundle bundle)
        {
            Bundle = bundle;
        }

        public abstract FIRIO Flip();
        public abstract IContainerIO GetIO(string ioName);
    }

    public abstract class ScalarIO : FIRIO
    {
        public readonly FIRRTLNode Node;
        public IFIRType Type { get; protected set; }
        public Connection Con = null;

        public ScalarIO(FIRRTLNode node, IFIRType type) : this(node, string.Empty, type)
        { }

        public ScalarIO(FIRRTLNode node, string name, IFIRType type) : base(name)
        {
            this.Node = node;
            this.Type = type;
        }

        public bool IsConnected()
        {
            return Con != null;
        }

        public override IContainerIO GetIO(string ioName)
        {
            throw new Exception("Scalar IO can't contain additional io.");
        }

        public abstract void SetType(IFIRType type);
    }

    public class Output : ScalarIO
    {
        public Output(FIRRTLNode node, IFIRType type) : this(node, string.Empty, type)
        { }

        public Output(FIRRTLNode node, string name, IFIRType type) : base(node, name, type)
        {
            this.Con = new Connection(this);
        }

        public override void SetType(IFIRType type)
        {
            Type = type;
            Con.Value = new ValueType(type);
        }

        public override FIRIO Flip()
        {
            return new Input(Node, Name, Type);
        }

        public void ConnectToInput(Input input)
        {
            Con.ConnectToInput(input);
        }

        public void InferType()
        {
            if (Node != null && Type is UnknownType)
            {
                Node.InferType();
            }
        }
    }

    public class Input : ScalarIO
    {
        public Input(FIRRTLNode node, IFIRType type) : this(node, string.Empty, type)
        { }

        public Input(FIRRTLNode node, string name, IFIRType type) : base(node, name, type)
        { }

        public override void SetType(IFIRType type)
        {
            Type = type;
        }

        public override FIRIO Flip()
        {
            return new Output(Node, Name, Type);
        }

        public void InferType()
        {
            if (Con != null && Type is UnknownType)
            {
                Con.From.InferType();
                Type = Con.From.Type;
            }
        }
    }

    public class IOBundle : FIRIO
    {
        private readonly Dictionary<string, FIRIO> IO = new Dictionary<string, FIRIO>();

        public IOBundle(string name, List<FIRIO> io, bool twoWayRelationship = true) : base(name)
        {
            foreach (var firIO in io)
            {
                IO.Add(firIO.Name, firIO);
            }

            if (twoWayRelationship)
            {
                foreach (var firIO in IO.Values)
                {
                    firIO.SetBundle(this);
                }
            }
        }

        public override FIRIO Flip()
        {
            List<FIRIO> flipped = IO.Values.Select(x => x.Flip()).ToList();
            return new IOBundle(Name, flipped, IO.Values.FirstOrDefault()?.IsPartOfBundle ?? false);
        }

        public IEnumerable<FIRIO> Flatten()
        {
            foreach (var io in IO.Values)
            {
                if (io is IOBundle bundle)
                {
                    foreach (var nested in bundle.Flatten())
                    {
                        yield return nested;
                    }
                }
                else
                {
                    yield return io;
                }
            }
        }

        public override IContainerIO GetIO(string ioName)
        {
            if (IO.TryGetValue(ioName, out FIRIO innerIO))
            {
                return innerIO;
            }

            throw new Exception($"Failed to find io. IO name: {ioName}");
        }
    }
}
