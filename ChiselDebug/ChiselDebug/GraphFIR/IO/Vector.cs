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

        public VectorAccess(FIRRTLNode node, FIRIO index, FIRIO access) : base(node, null, new List<FIRIO>() { index, access })
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

        public override FIRIO ToFlow(FlowChange flow, FIRRTLNode node)
        {
            return new VectorAccess(node, Index.ToFlow(flow, node), Access.ToFlow(flow, node));
        }
    }

    public class Vector : AggregateIO, IPortsIO
    {
        private readonly FIRIO[] IO;
        private readonly List<VectorAccess> VisiblePorts = new List<VectorAccess>();
        public int Length => IO.Length;

        public Vector(FIRRTLNode node, string name, int length, FIRIO firIO) : this(node, name, length, firIO, new List<VectorAccess>())
        { }

        private Vector(FIRRTLNode node, string name, int length, FIRIO firIO, List<VectorAccess> visiblePorts) : base(node, name)
        {
            if (!firIO.IsPassive())
            {
                throw new Exception("IO type of vector must be passive.");
            }

            this.IO = new FIRIO[length];
            this.VisiblePorts = visiblePorts;
            for (int i = 0; i < IO.Length; i++)
            {
                IO[i] = firIO.Copy(node);
                IO[i].SetName(i.ToString());
                IO[i].SetParentIO(this);
            }
        }

        public override FIRIO[] GetIOInOrder()
        {
            List<FIRIO> allIO = new List<FIRIO>();
            allIO.AddRange(IO);
            allIO.AddRange(VisiblePorts);

            return allIO.ToArray();
        }

        public FIRIO[] GetIndexesInOrder()
        {
            return IO.ToArray();
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

        public override FIRIO ToFlow(FlowChange flow, FIRRTLNode node)
        {
            return new Vector(node, Name, Length, IO[0].ToFlow(flow, node), VisiblePorts.Select(x => (VectorAccess)x.ToFlow(flow, node)).ToList());
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

        public override List<ScalarIO> Flatten(List<ScalarIO> list)
        {
            for (int i = 0; i < IO.Length; i++)
            {
                IO[i].Flatten(list);
            }
            for (int i = 0; i < VisiblePorts.Count; i++)
            {
                VisiblePorts[i].Flatten(list);
            }

            return list;
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

        public override List<T> GetAllIOOfType<T>(List<T> list)
        {
            if (this is T tVal)
            {
                list.Add(tVal);
            }

            foreach (var io in IO)
            {
                io.GetAllIOOfType<T>(list);
            }
            foreach (var io in VisiblePorts)
            {
                io.GetAllIOOfType<T>(list);
            }

            return list;
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
            if (int.TryParse(ioName, out int index))
            {
                if (index >= IO.Length || index < 0)
                {
                    container = null;
                    return false;
                }

                container = IO[index];
                return true;
            }

            container = null;
            return false;
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

            VectorAccess access = new VectorAccess(Node, accessIndex, accessIO);
            access.SetParentIO(this);
            VisiblePorts.Add(access);

            return access;
        }

        FIRIO[] IPortsIO.GetAllPorts()
        {
            return VisiblePorts.ToArray();
        }

        FIRIO[] IPortsIO.GetOrMakeFlippedPortsFrom(FIRIO[] otherPorts)
        {
            FIRIO[] newPorts = new FIRIO[otherPorts.Length];

            for (int i = 0; i < newPorts.Length; i++)
            {
                VectorAccess newPort = (VectorAccess)otherPorts[i].Flip(Node);
                newPort.SetParentIO(this);
                VisiblePorts.Add(newPort);
                newPorts[i] = newPort;
            }

            return newPorts;
        }
    }
}
