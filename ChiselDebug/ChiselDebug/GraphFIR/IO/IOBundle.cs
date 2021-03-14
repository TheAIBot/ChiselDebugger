using System;
using System.Collections.Generic;
using System.Linq;

namespace ChiselDebug.GraphFIR.IO
{
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

        public override FIRIO GetInput()
        {
            return this;
        }

        public override FIRIO GetOutput()
        {
            return this;
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

        public override FIRIO Copy()
        {
            List<FIRIO> flipped = IO.Values.Select(x => x.Copy()).ToList();
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

        public override bool IsPassiveOfType<T>()
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
    }
}
