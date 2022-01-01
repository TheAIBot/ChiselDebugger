using ChiselDebug.GraphFIR.Components;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ChiselDebug.GraphFIR.IO
{
    public sealed class Vector : AggregateIO
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

        public Vector(FIRRTLNode node, string name, FIRIO[] ios) : base(node, name)
        {
            if (!(ios.All(x => x.IsPassiveOfType<Sink>()) ||
                  ios.All(x => x.IsPassiveOfType<Source>())))
            {
                throw new Exception("IO type of vector must be passive.");
            }

            this.IO = ios.ToArray();
            for (int i = 0; i < IO.Length; i++)
            {
                IO[i].SetName(i.ToString());
                IO[i].SetParentIO(this);
            }
        }

        public override FIRIO[] GetIOInOrder()
        {
            return IO.ToArray();
        }

        public override void ConnectToInput(FIRIO input, bool allowPartial = false, bool asPassive = false, Source condition = null)
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
                if (IO[i] is ScalarIO ioScalar && other.IO[i] is ScalarIO otherScalar)
                {
                    var io = IOHelper.OrderIO(ioScalar, otherScalar);
                    io.output.ConnectToInput(io.input, allowPartial, asPassive, condition);
                }
                else if (IO[i] is AggregateIO && other.IO[i] is AggregateIO)
                {
                    IO[i].ConnectToInput(other.IO[i], allowPartial, asPassive, condition);
                }
                else
                {
                    throw new Exception("Two vector do not contain the same types.");
                }
            }

            base.ConnectToInput(input, allowPartial, asPassive, condition);
        }

        public override FIRIO ToFlow(FlowChange flow, FIRRTLNode node)
        {
            return new Vector(node, Name, Length, IO[0].ToFlow(flow, node));
        }

        internal override void FlattenOnly<T>(ref Span<T> list)
        {
            for (int i = 0; i < IO.Length; i++)
            {
                IO[i].FlattenOnly(ref list);
            }
        }

        public override List<T> FlattenTo<T>(List<T> list)
        {
            for (int i = 0; i < IO.Length; i++)
            {
                IO[i].FlattenTo(list);
            }

            return list;
        }

        public override int GetScalarsCount()
        {
            if (IO.Length == 0)
            {
                return 0;
            }

            return IO[0].GetScalarsCount() * IO.Length;
        }

        public override int GetScalarsCountOfType<T>()
        {
            if (IO.Length == 0)
            {
                return 0;
            }

            return IO[0].GetScalarsCountOfType<T>() * IO.Length;
        }

        public override bool IsPassiveOfType<T>()
        {
            return IO[0].IsPassiveOfType<T>();
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
