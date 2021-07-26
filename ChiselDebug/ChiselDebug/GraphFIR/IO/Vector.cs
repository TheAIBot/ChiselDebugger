using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChiselDebug.GraphFIR.IO
{
    public class Vector : AggregateIO
    {
        private readonly FIRIO[] IO;
        public int Length => IO.Length;

        public Vector(FIRRTLNode node, string name, int length, FIRIO firIO) : base(node, name)
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
        }

        public override FIRIO[] GetIOInOrder()
        {
            return IO.ToArray();
        }

        public override void ConnectToInput(FIRIO input, bool allowPartial = false, bool asPassive = false, Output condition = null)
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

            int shortestLength = Math.Min(Length, other.Length);
            for (int i = 0; i < shortestLength; i++)
            {
                IO[i].ConnectToInput(other.IO[i], allowPartial, asPassive, condition);
            }
        }

        public override FIRIO ToFlow(FlowChange flow, FIRRTLNode node)
        {
            return new Vector(node, Name, Length, IO[0].ToFlow(flow, node));
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

        public override List<ScalarIO> Flatten(List<ScalarIO> list)
        {
            for (int i = 0; i < IO.Length; i++)
            {
                IO[i].Flatten(list);
            }

            return list;
        }

        public override bool IsPassiveOfType<T>()
        {
            return IO[0].IsPassiveOfType<T>();
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

        public override bool TryGetIO(string ioName, out IContainerIO container)
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
    }
}
