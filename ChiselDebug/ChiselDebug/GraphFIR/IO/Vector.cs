using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChiselDebug.GraphFIR.IO
{
    public class VectorIO : IOBundle
    {
        private readonly FIRIO VecIn;
        private readonly FIRIO VecOut;

        public VectorIO(string name, FIRIO vecIn, FIRIO vecOut) : base(name, new List<FIRIO>() { vecIn, vecOut }, false)
        {
            this.VecIn = vecIn;
            this.VecOut = vecOut;
        }

        public override FIRIO GetInput()
        {
            return VecIn;
        }

        public override FIRIO GetOutput()
        {
            return VecOut;
        }
    }

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

    public class Vector : FIRIO
    {
        private readonly VectorIO[] IO;
        private readonly List<VectorAccess> VectorPorts = new List<VectorAccess>();
        private readonly FIRRTLNode Node;
        public int Length => IO.Length;

        public Vector(string name, int length, FIRIO firIO, FIRRTLNode node) : base(name)
        {
            this.IO = new VectorIO[length];
            for (int i = 0; i < IO.Length; i++)
            {
                var input = firIO.Copy(node);
                var output = firIO.Flip(node);

                input.SetName(i.ToString());
                output.SetName(null);

                IO[i] = new VectorIO(i.ToString(), input, output);
            }

            this.Node = node;
        }

        public override FIRIO GetInput()
        {
            return this;
        }

        public override FIRIO GetOutput()
        {
            return this;
        }

        public FIRIO[] GetIOInOrder()
        {
            List<FIRIO> allIO = new List<FIRIO>();
            allIO.AddRange(IO);
            allIO.AddRange(VectorPorts);

            return allIO.ToArray();
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
                IO[i].GetOutput().ConnectToInput(other.IO[i].GetInput());
            }
        }

        public override FIRIO Flip(FIRRTLNode node = null)
        {
            return Copy(node);// new Vector(Name, Length, IO[0].GetInput().Flip(node), node);
        }

        public override FIRIO Copy(FIRRTLNode node = null)
        {
            return new Vector(Name, Length, IO[0].GetInput().Copy(node), node);
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
            return IO[0].GetInput().IsPassiveOfType<T>();
        }

        public override bool SameIO(FIRIO other)
        {
            if (other is Vector vec)
            {
                if (Length != vec.Length)
                {
                    return false;
                }

                return IO[0].GetInput().SameIO(vec.IO[0].GetInput());
            }

            return false;
        }

        public override bool TryGetIO(string ioName, bool modulesOnly, out IContainerIO container)
        {
            throw new NotImplementedException();
        }

        public VectorIO GetIndex(int index)
        {
            return IO[index];
        }

        public VectorAccess MakeAccess(Output index)
        {
            VectorAccess access = new VectorAccess(index, IO[0].GetInput().Copy(), Node);
            VectorPorts.Add(access);

            return access;
        }
    }
}
