using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChiselDebug.GraphFIR.IO
{
    public class VectorAccess : IOBundle
    {
        private readonly FIRIO Access;
        private readonly FIRIO Index;

        public VectorAccess(FIRIO index, FIRIO access) : base(string.Empty, new List<FIRIO>() { index, access })
        {
            this.Access = access;
            this.Index = index;
        }

        public override FIRIO GetInput()
        {
            if (!Access.IsPassiveOfType<Input>())
            {
                throw new Exception("Vector access is not a passive input type.");
            }

            return Access;
        }

        public override FIRIO GetOutput()
        {
            if (!Access.IsPassiveOfType<Output>())
            {
                throw new Exception("Vector access is not a passive output type.");
            }

            return Access;
        }

        public override FIRIO ToFlow(FlowChange flow, FIRRTLNode node = null)
        {
            return new VectorAccess(Index.ToFlow(flow, node), Access.ToFlow(flow, node));
        }
    }

    public class Vector : AggregateIO, IHiddenPorts
    {
        private readonly FIRIO[] IO;
        private readonly List<VectorAccess> VisiblePorts = new List<VectorAccess>();
        private readonly List<VectorAccess> HiddenPorts = new List<VectorAccess>();
        private readonly FIRRTLNode Node;
        public int Length => IO.Length;

        public Vector(string name, int length, FIRIO firIO, FIRRTLNode node) : base(name)
        {
            if (!firIO.IsPassive())
            {
                throw new Exception("IO type of vector must be passive.");
            }

            this.IO = new FIRIO[length];
            for (int i = 0; i < IO.Length; i++)
            {
                IO[i] = firIO.Copy(node);
                IO[i].SetName(i.ToString());
                IO[i].SetParentIO(this);
            }

            this.Node = node;
        }

        public override FIRIO[] GetIOInOrder()
        {
            List<FIRIO> allIO = new List<FIRIO>();
            allIO.AddRange(IO);
            allIO.AddRange(VisiblePorts);

            return allIO.ToArray();
        }

        public override bool IsVisibleAggregate()
        {
            return IO.FirstOrDefault()?.IsPartOfAggregateIO ?? false;
        }

        public override void ConnectToInput(FIRIO input, bool allowPartial = false, bool asPassive = false, bool isConditional = false)
        {
            if (input is not Vector)
            {
                throw new Exception("Vector can only connect to other vector.");
            }
            Vector other = (Vector)input;

            if (!allowPartial && Length != other.Length)
            {
                throw new Exception("Vectors must have the same when when fully connecting them.");
            }

            int longestLength = Math.Max(Length, other.Length);
            for (int i = 0; i < longestLength; i++)
            {
                IO[i].ConnectToInput(other.IO[i], allowPartial, asPassive, isConditional);
            }
        }

        public override FIRIO ToFlow(FlowChange flow, FIRRTLNode node = null)
        {
            return new Vector(Name, Length, IO[0].ToFlow(flow, node), node);
        }

        public override IEnumerable<ScalarIO> Flatten()
        {
            for (int i = 0; i < IO.Length; i++)
            {
                foreach (var nested in IO[i].Flatten())
                {
                    yield return nested;
                }
            }
            for (int i = 0; i < VisiblePorts.Count; i++)
            {
                foreach (var nested in VisiblePorts[i].Flatten())
                {
                    yield return nested;
                }
            }
        }

        public override bool IsPassiveOfType<T>()
        {
            return IO[0].IsPassiveOfType<T>();
        }

        public override bool SameIO(FIRIO other)
        {
            if (other is Vector vec)
            {
                if (Length != vec.Length)
                {
                    return false;
                }

                return IO[0].SameIO(vec.IO[0]);
            }

            return false;
        }

        public override IEnumerable<T> GetAllIOOfType<T>()
        {
            if (this is T thisIsT)
            {
                yield return thisIsT;
            }

            foreach (var io in GetIOInOrder())
            {
                foreach (var nested in io.GetAllIOOfType<T>())
                {
                    yield return nested;
                }
            }
        }

        public override IEnumerable<FIRIO> WalkIOTree()
        {
            yield return this;

            foreach (var io in GetIOInOrder())
            {
                foreach (var nested in io.WalkIOTree())
                {
                    yield return nested;
                }
            }
        }

        public override bool TryGetIO(string ioName, bool modulesOnly, out IContainerIO container)
        {
            throw new NotImplementedException();
        }

        public FIRIO GetIndex(int index)
        {
            return IO[index];
        }

        internal VectorAccess MakeWriteAccess(Output index, IOGender gender)
        {
            FIRIO accessIO = IO[0].Copy();
            if (gender == IOGender.Male && !accessIO.IsPassiveOfType<Output>() ||
                gender == IOGender.Female && !accessIO.IsPassiveOfType<Input>())
            {
                accessIO = accessIO.Flip();
            }

            FIRIO accessIndex = index.Flip(Node);
            accessIndex.SetName("index");
            index.ConnectToInput(accessIndex);

            VectorAccess access = new VectorAccess(accessIndex, accessIO);
            access.SetParentIO(this);
            HiddenPorts.Add(access);

            return access;
        }

        bool IHiddenPorts.HasHiddenPorts()
        {
            return HiddenPorts.Count > 0;
        }

        FIRIO[] IHiddenPorts.GetHiddenPorts()
        {
            return HiddenPorts.ToArray();
        }

        List<FIRIO> IHiddenPorts.CopyHiddenPortsFrom(IHiddenPorts otherWithPorts)
        {
            List<FIRIO> newPorts = new List<FIRIO>();
            foreach (var port in otherWithPorts.GetHiddenPorts())
            {
                VectorAccess newPort = (VectorAccess)port.Flip(Node);
                HiddenPorts.Add(newPort);
                newPorts.Add(newPort);
            }

            return newPorts;
        }

        void IHiddenPorts.MakePortsVisible()
        {
            foreach (var port in HiddenPorts)
            {
                VisiblePorts.Add(port);
            }

            HiddenPorts.Clear();
        }
    }
}
