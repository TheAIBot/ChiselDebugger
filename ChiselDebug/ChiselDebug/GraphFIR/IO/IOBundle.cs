using System;
using System.Collections.Generic;
using System.Linq;

namespace ChiselDebug.GraphFIR.IO
{
    public class IOBundle : AggregateIO
    {
        private readonly FIRIO[] OrderedIO;
        private readonly Dictionary<string, FIRIO> IO = new Dictionary<string, FIRIO>();

        public IOBundle(string name, List<FIRIO> io, bool twoWayRelationship = true) : base(name)
        {
            this.OrderedIO = io.ToArray();

            foreach (var firIO in io)
            {
                IO.Add(firIO.Name, firIO);
            }

            if (twoWayRelationship)
            {
                foreach (var firIO in IO.Values)
                {
                    firIO.SetParentIO(this);
                }
            }
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
            return OrderedIO.ToArray();
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
                else if (a is Vector aVec && b is Vector bVec)
                {
                    aVec.ConnectToInput(bVec, allowPartial, asPassive);
                }
                else
                {
                    throw new Exception($"Can't connect IO of type {a.GetType()} to {b.GetType()}.");
                }
            }
        }

        public override FIRIO Flip(FIRRTLNode node = null)
        {
            List<FIRIO> flipped = OrderedIO.Select(x => x.Flip(node)).ToList();
            return new IOBundle(Name, flipped, IO.Values.FirstOrDefault()?.IsPartOfAggregateIO ?? false);
        }

        public override FIRIO Copy(FIRRTLNode node = null)
        {
            List<FIRIO> flipped = OrderedIO.Select(x => x.Copy(node)).ToList();
            return new IOBundle(Name, flipped, IO.Values.FirstOrDefault()?.IsPartOfAggregateIO ?? false);
        }

        public override IEnumerable<ScalarIO> Flatten()
        {
            foreach (var io in IO.Values)
            {
                foreach (var nested in io.Flatten())
                {
                    yield return nested;
                }
            }
        }

        public override bool IsPassiveOfType<T>()
        {
            foreach (var io in IO.Values)
            {
                if (!io.IsPassiveOfType<T>())
                {
                    return false;
                }
            }

            return true;
        }

        public override bool SameIO(FIRIO other)
        {
            if (other is IOBundle bundle)
            {
                if (OrderedIO.Length != bundle.OrderedIO.Length)
                {
                    return false;
                }

                for (int i = 0; i < OrderedIO.Length; i++)
                {
                    if (!OrderedIO[i].SameIO(bundle.OrderedIO[i]))
                    {
                        return false;
                    }
                }

                return true;
            }

            return false;
        }

        public override bool TryGetIO(string ioName, bool modulesOnly, out IContainerIO container)
        {
            if (IO.TryGetValue(ioName, out FIRIO innerIO))
            {
                container = innerIO;
                return true;
            }

            container = null;
            return false;
        }

        protected void ChangeIO(FIRIO toChange, FIRIO changeTo)
        {
            int index = Array.IndexOf(OrderedIO, toChange);
            OrderedIO[index] = changeTo;

            IO.Remove(toChange.Name);
            IO.Add(changeTo.Name, changeTo);
        }
    }
}
