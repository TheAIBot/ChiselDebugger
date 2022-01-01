using FIRRTL;
using System;
using System.Collections.Generic;

namespace ChiselDebug.GraphFIR.Circuit.Converter
{
    public static partial class CircuitToGraph
    {
        private static void VisitPort(VisitHelper helper, Port port)
        {
            var io = VisitType(helper, port.Direction, port.Name, port.Type, false);
            helper.Mod.AddExternalIO(io.Copy(helper.Mod));
        }

        private static IO.FIRIO VisitTypeAsPassive(VisitHelper helper, Dir direction, string name, IFIRType type)
        {
            return VisitType(helper, direction, name, type, true);
        }

        private static IO.FIRIO VisitType(VisitHelper helper, Dir direction, string name, IFIRType type, bool forcePassive) => (direction, type) switch
        {
            (_, BundleType bundle) => VisitBundle(helper, direction, name, bundle, forcePassive),
            (_, VectorType vec) => VisitVector(helper, direction, name, vec, forcePassive),
            (Dir.Input, _) => new IO.Input(null, name, type),
            (Dir.Output, _) => new IO.Output(null, name, type),
            _ => throw new NotImplementedException()
        };

        private static IO.FIRIO VisitBundle(VisitHelper helper, Dir direction, string bundleName, BundleType bundle, bool forcePassive)
        {
            List<IO.FIRIO> io = new List<IO.FIRIO>();
            foreach (var field in bundle.Fields)
            {
                //If passive then ignore flips so all flows are the same
                Dir fieldDir = !forcePassive ? direction.Flip(field.Flip) : direction;
                io.Add(VisitType(helper, fieldDir, field.Name, field.Type, forcePassive));
            }

            return new IO.IOBundle(null, bundleName, io);
        }

        private static IO.FIRIO VisitVector(VisitHelper helper, Dir direction, string vectorName, VectorType vec, bool forcePassive)
        {
            var type = VisitType(helper, direction, null, vec.Type, forcePassive);
            return new IO.Vector(null, vectorName, vec.Size, type);
        }
    }
}
