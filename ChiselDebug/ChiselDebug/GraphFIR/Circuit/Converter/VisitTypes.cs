using FIRRTL;
using System;
using System.Collections.Generic;

namespace ChiselDebug.GraphFIR.Circuit.Converter
{
    public static partial class CircuitToGraph
    {
        private static void VisitPort(VisitHelper helper, FIRRTL.Port port)
        {
            var io = VisitType(helper, port.Direction, port.Name, port.Type, false);
            helper.Mod.AddExternalIO(io.Copy(helper.Mod));
        }

        private static IO.FIRIO VisitTypeAsPassive(VisitHelper helper, FIRRTL.Dir direction, string name, FIRRTL.IFIRType type)
        {
            return VisitType(helper, direction, name, type, true);
        }

        private static IO.FIRIO VisitType(VisitHelper helper, FIRRTL.Dir direction, string name, FIRRTL.IFIRType type, bool forcePassive)
        {
            if (type is FIRRTL.BundleType bundle)
            {
                return VisitBundle(helper, direction, name, bundle, forcePassive);
            }
            else if (type is FIRRTL.VectorType vec)
            {
                return VisitVector(helper, direction, name, vec, forcePassive);
            }
            else if (direction == FIRRTL.Dir.Input)
            {
                return new IO.Input(null, name, type);
            }
            else if (direction == FIRRTL.Dir.Output)
            {
                return new IO.Output(null, name, type);
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        private static IO.FIRIO VisitBundle(VisitHelper helper, FIRRTL.Dir direction, string bundleName, FIRRTL.BundleType bundle, bool forcePassive)
        {
            List<IO.FIRIO> io = new List<IO.FIRIO>();
            foreach (var field in bundle.Fields)
            {
                //If passive then ignore flips so all flows are the same
                FIRRTL.Dir fieldDir = !forcePassive ? direction.Flip(field.Flip) : direction;
                io.Add(VisitType(helper, fieldDir, field.Name, field.Type, forcePassive));
            }

            return new IO.IOBundle(null, bundleName, io);
        }

        private static IO.FIRIO VisitVector(VisitHelper helper, FIRRTL.Dir direction, string vectorName, FIRRTL.VectorType vec, bool forcePassive)
        {
            var type = VisitType(helper, direction, null, vec.Type, forcePassive);
            return new IO.Vector(null, vectorName, vec.Size, type);
        }
    }
}
