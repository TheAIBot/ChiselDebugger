using ChiselDebug.GraphFIR.Components;

namespace ChiselDebug.GraphFIR.Circuit.Converter
{
    public static partial class CircuitToGraph
    {
        private static void VisitConnect(IVisitHelper helper, FIRRTL.IExpression exprFrom, FIRRTL.IExpression exprTo, bool isPartial)
        {
            IO.FIRIO from = VisitExp(helper, exprFrom, IO.FlowChange.Source);
            IO.FIRIO to = (IO.FIRIO)VisitRef(helper, exprTo, helper.Mod, IO.FlowChange.Sink);

            //Can only connect two aggregates. If any of the two are not an
            //aggregate type then try convert both to scalar io and connect them.
            if (from is not IO.AggregateIO || to is not IO.AggregateIO)
            {
                from = from.GetSource();
                to = to.GetSink();
            }

            ConnectIO(helper, from, to, isPartial);
        }

        private static void ConnectIO(IVisitHelper helper, IO.FIRIO from, IO.FIRIO to, bool isPartial, bool canBeConditional = true)
        {
            Module fromMod = from.GetModResideIn();
            Module toMod = to.GetModResideIn();

            //If going from inside to outside or outside to outside
            //then add condition to that connection if currently in
            //conditional module.
            IO.Source condition = null;
            if (canBeConditional &&
                (fromMod == helper.Mod && toMod != helper.Mod ||
                 fromMod != helper.Mod && toMod != helper.Mod))
            {
                condition = helper.Mod.EnableCon;
            }

            from.ConnectToInput(to, isPartial, false, condition);

            //If writing to a memory ports data in high level firrtl, then
            //the mask also has to be set to true for the part of the port data
            //that was written to.
            if (IO.IOHelper.TryGetParentMemPort(to, out var memPort) &&
                memPort.FromHighLevelFIRRTL &&
                IO.IOHelper.IsIOInMaskableMemPortData(to, memPort))
            {
                var scopeEnableCond = helper.ScopeEnabledCond;
                foreach (IO.Sink dataInputWrittenTo in to.Flatten())
                {
                    var dataInputMask = memPort.GetMaskFromDataInput(dataInputWrittenTo);
                    scopeEnableCond.ConnectToInput(dataInputMask, false, false, scopeEnableCond);
                }
            }
        }
    }
}
