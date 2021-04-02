using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChiselDebug.GraphFIR.IO
{
    public class VectorAccess : IOBundle
    {
        private readonly FIRIO InputAccess;

        public VectorAccess(Output index, FIRIO inputAccess, FIRRTLNode node) : base(string.Empty, new List<FIRIO>() { new Input(node, "index", index.Type), inputAccess })
        {
            this.InputAccess = inputAccess;
            index.ConnectToInput((Input)GetIO("index"));
        }

        public override FIRIO GetInput()
        {
            return InputAccess;
        }

        public override FIRIO GetOutput()
        {
            FIRIO outputAccess = InputAccess.Flip();
            ChangeIO(InputAccess, outputAccess);

            return outputAccess;
        }
    }

    public class Vector : AggregateIO
    {
        private readonly FIRIO[] IO;
        private readonly List<VectorAccess> VectorPorts = new List<VectorAccess>();
        private readonly FIRRTLNode Node;
        public int Length => IO.Length;

        public Vector(string name, int length, FIRIO firIO, FIRRTLNode node) : base(name)
        {
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
            allIO.AddRange(VectorPorts);

            return allIO.ToArray();
        }

        public override bool IsVisibleAggregate()
        {
            return IO.FirstOrDefault()?.IsPartOfAggregateIO ?? false;
        }

        public override void ConnectToInput(FIRIO input, bool allowPartial = false, bool asPassive = false)
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
                IO[i].ConnectToInput(other.IO[i]);
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

        public override bool TryGetIO(string ioName, bool modulesOnly, out IContainerIO container)
        {
            throw new NotImplementedException();
        }

        public FIRIO GetIndex(int index)
        {
            return IO[index];
        }

        public VectorAccess MakeAccess(Output index)
        {
            VectorAccess access = new VectorAccess(index, IO[0].Copy(), Node);
            VectorPorts.Add(access);

            return access;
        }
    }
}
