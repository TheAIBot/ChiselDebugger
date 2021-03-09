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

        public IContainerIO GetIO(Span<string> names)
        {
            if (names.Length == 0)
            {
                return this;
            }

            return GetIO(names[0]).GetIO(names.Slice(1));
        }
    }

    public abstract class FIRIO : IContainerIO
    {
        public string Name { get; private set; }
        public bool IsAnonymous => Name == string.Empty;
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

        public abstract void ConnectToInput(FIRIO input, bool allowPartial = false, bool asPassive = false);
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

        public override void ConnectToInput(FIRIO input, bool allowPartial = false, bool asPassive = false)
        {
            if (input is Input ioIn)
            {
                Con.ConnectToInput(ioIn);
            }
            else
            {
                throw new Exception("Output can only be connected to input.");
            }
        }

        public override FIRIO Flip()
        {
            return new Input(Node, Name, Type);
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

        public override void ConnectToInput(FIRIO input, bool allowPartial = false, bool asPassive = false)
        {
            throw new Exception("Input can't be connected to output. Flow is reversed.");
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

        public override void ConnectToInput(FIRIO input, bool allowPartial = false, bool asPassive = false)
        {
            if (input is not IOBundle)
            {
                throw new Exception("Bundle can only connect to other bundle.");
            }
            IOBundle bundle = (IOBundle)input;

            if (asPassive && !IsPassiveOfType<Output>())
            {
                throw new Exception("Bundle must be a passive output bundle but it was not.");
            }

            if (asPassive && !bundle.IsPassiveOfType<Input>())
            {
                throw new Exception("Bundle must connect to a passive input bundle.");
            }

            if (!allowPartial && IO.Count != bundle.IO.Count)
            {
                throw new Exception("Trying to fully connect two bundles that don't match.");
            }

            IEnumerable<string> ioConnectNames = IO.Keys;
            if (allowPartial)
            {
                ioConnectNames = ioConnectNames.Intersect(bundle.IO.Keys);
            }

            foreach (var ioName in ioConnectNames)
            {
                var a = GetIO(ioName);
                var b = bundle.GetIO(ioName);

                if (a is Output aOut && b is Input bIn)
                {
                    aOut.ConnectToInput(bIn);
                }
                else if (a is Input aIn && b is Output bOut)
                {
                    bOut.ConnectToInput(aIn);
                }
                else if (a is IOBundle aBundle && b is IOBundle bBundle)
                {
                    aBundle.ConnectToInput(bBundle, allowPartial, asPassive);
                }

                throw new Exception($"Can't connect IO of type {a.GetType()} to {b.GetType()}.");
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

        private bool IsPassiveOfType<T>()
        {
            foreach (var io in IO.Values)
            {
                if (io is IOBundle bundle && !bundle.IsPassiveOfType<T>())
                {
                    return false;
                }
                else if (io is not T)
                {
                    return false;
                }
            }

            return true;
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
