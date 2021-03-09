using FIRRTL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ChiselDebug.GraphFIR
{
    public abstract class FIRIO
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
        public readonly List<FIRIO> IO = new List<FIRIO>();

        public IOBundle(string name, List<FIRIO> io, bool twoWayRelationship = true) : base(name)
        {
            this.IO = io;

            if (twoWayRelationship)
            {
                foreach (var firIO in IO)
                {
                    firIO.SetBundle(this);
                }
            }
        }

        public override FIRIO Flip()
        {
            List<FIRIO> flipped = IO.Select(x => x.Flip()).ToList();
            return new IOBundle(Name, flipped, IO.FirstOrDefault()?.IsPartOfBundle ?? false);
        }

        public IEnumerable<FIRIO> Flatten()
        {
            foreach (var io in IO)
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
    }
}
