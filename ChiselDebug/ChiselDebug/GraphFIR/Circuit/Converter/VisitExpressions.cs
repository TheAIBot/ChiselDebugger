using ChiselDebug.GraphFIR.Components;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ChiselDebug.GraphFIR.Circuit.Converter
{
    public static partial class CircuitToGraph
    {
        private static IO.FIRIO VisitExp(VisitHelper helper, FIRRTL.Expression exp, IO.FlowChange ioFlow)
        {
            if (exp is FIRRTL.RefLikeExpression)
            {
                return (IO.FIRIO)VisitRef(helper, exp, helper.Mod, ioFlow);
            }

            if (exp is FIRRTL.Literal lit)
            {
                ConstValue value = new ConstValue(lit);

                helper.AddNodeToModule(value);
                return value.Result;
            }
            else if (exp is FIRRTL.DoPrim prim)
            {
                var args = prim.Args.Select(x => VisitExp(helper, x, IO.FlowChange.Source)).Cast<IO.Source>().ToArray();
                FIRRTLPrimOP RemainingPrimOps(FIRRTL.PrimOp op)
                {
                    if (prim.Op is FIRRTL.Shl)
                    {
                        var constLit = new FIRRTL.UIntLiteral(prim.Consts[0], (int)prim.Consts[0].GetBitLength());
                        var constOutput = (IO.Source)VisitExp(helper, constLit, IO.FlowChange.Source);
                        return new FIRShl(args[0], constOutput, prim.Type, prim);
                    }
                    else if (prim.Op is FIRRTL.Shr)
                    {
                        var constLit = new FIRRTL.UIntLiteral(prim.Consts[0], (int)prim.Consts[0].GetBitLength());
                        var constOutput = (IO.Source)VisitExp(helper, constLit, IO.FlowChange.Source);
                        return new FIRShr(args[0], constOutput, prim.Type, prim);
                    }
                    else
                    {
                        throw new NotImplementedException();
                    }
                }
                FIRRTLPrimOP nodePrim = prim.Op switch
                {
                    FIRRTL.Add => new FIRAdd(args[0], args[1], prim.Type, prim),
                    FIRRTL.Sub => new FIRSub(args[0], args[1], prim.Type, prim),
                    FIRRTL.Mul => new FIRMul(args[0], args[1], prim.Type, prim),
                    FIRRTL.Div => new FIRDiv(args[0], args[1], prim.Type, prim),
                    FIRRTL.Rem => new FIRRem(args[0], args[1], prim.Type, prim),
                    FIRRTL.Dshl => new FIRDshl(args[0], args[1], prim.Type, prim),
                    FIRRTL.Dshr => new FIRDshr(args[0], args[1], prim.Type, prim),
                    FIRRTL.Cat => new FIRCat(args[0], args[1], prim.Type, prim),
                    FIRRTL.Eq => new FIREq(args[0], args[1], prim.Type, prim),
                    FIRRTL.Neq => new FIRNeq(args[0], args[1], prim.Type, prim),
                    FIRRTL.Geq => new FIRGeq(args[0], args[1], prim.Type, prim),
                    FIRRTL.Leq => new FIRLeq(args[0], args[1], prim.Type, prim),
                    FIRRTL.Gt => new FIRGt(args[0], args[1], prim.Type, prim),
                    FIRRTL.Lt => new FIRLt(args[0], args[1], prim.Type, prim),
                    FIRRTL.And => new FIRAnd(args[0], args[1], prim.Type, prim),
                    FIRRTL.Or => new FIROr(args[0], args[1], prim.Type, prim),
                    FIRRTL.Xor => new FIRXor(args[0], args[1], prim.Type, prim),
                    FIRRTL.Head => new Head(args[0], prim.Type, (int)prim.Consts[0], prim),
                    FIRRTL.Tail => new Tail(args[0], prim.Type, (int)prim.Consts[0], prim),
                    FIRRTL.Bits => new BitExtract(args[0], prim.Type, (int)prim.Consts[1], (int)prim.Consts[0], prim),
                    FIRRTL.Pad => new Pad(args[0], prim.Type, (int)prim.Consts[0], prim),
                    FIRRTL.AsUInt => new FIRAsUInt(args[0], prim.Type, prim),
                    FIRRTL.AsSInt => new FIRAsSInt(args[0], prim.Type, prim),
                    FIRRTL.AsClock => new FIRAsClock(args[0], prim.Type, prim),
                    FIRRTL.Cvt => new FIRCvt(args[0], prim.Type, prim),
                    FIRRTL.Neg => new FIRNeg(args[0], prim.Type, prim),
                    FIRRTL.Not => new FIRNot(args[0], prim.Type, prim),
                    FIRRTL.Andr => new FIRAndr(args[0], prim.Type, prim),
                    FIRRTL.Orr => new FIROrr(args[0], prim.Type, prim),
                    FIRRTL.Xorr => new FIRXorr(args[0], prim.Type, prim),
                    _ => RemainingPrimOps(prim.Op)
                };

                helper.AddNodeToModule(nodePrim);
                return nodePrim.Result;
            }
            else if (exp is FIRRTL.Mux mux)
            {
                var cond = (IO.Source)VisitExp(helper, mux.Cond, IO.FlowChange.Source);
                var ifTrue = VisitExp(helper, mux.TrueValue, IO.FlowChange.Source);
                var ifFalse = VisitExp(helper, mux.FalseValue, IO.FlowChange.Source);

                Mux node = new Mux(new List<IO.FIRIO>() { ifTrue, ifFalse }, cond, mux);

                helper.AddNodeToModule(node);
                return node.Result;
            }
            else if (exp is FIRRTL.ValidIf validIf)
            {
                var cond = (IO.Source)VisitExp(helper, validIf.Cond, IO.FlowChange.Source);
                var ifValid = VisitExp(helper, validIf.Value, IO.FlowChange.Source);

                Mux node = new Mux(new List<IO.FIRIO>() { ifValid }, cond, validIf);

                helper.AddNodeToModule(node);
                return node.Result;
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        private static IO.IContainerIO VisitRef(VisitHelper helper, FIRRTL.Expression exp, IO.IContainerIO currContainer, IO.FlowChange ioFlow)
        {
            IO.IContainerIO refContainer;
            if (exp is FIRRTL.Reference reference)
            {
                refContainer = currContainer.GetIO(reference.Name);
            }
            else if (exp is FIRRTL.SubField subField)
            {
                var subContainer = VisitExp(helper, subField.Expr, ioFlow);
                refContainer = subContainer.GetIO(subField.Name);
            }
            else if (exp is FIRRTL.SubIndex subIndex)
            {
                var subVec = VisitExp(helper, subIndex.Expr, ioFlow);
                var vec = (IO.Vector)subVec;

                refContainer = vec.GetIndex(subIndex.Value);
            }
            else if (exp is FIRRTL.SubAccess subAccess)
            {
                var subVec = VisitExp(helper, subAccess.Expr, ioFlow);
                var vec = (IO.Vector)subVec;
                var index = (IO.Source)VisitExp(helper, subAccess.Index, IO.FlowChange.Source);

                if (ioFlow == IO.FlowChange.Source)
                {
                    Mux node = new Mux(vec.GetIOInOrder().ToList(), index, null, true);
                    helper.AddNodeToModule(node);

                    refContainer = node.Result;
                }
                else
                {
                    VectorAssign vecAssign = new VectorAssign(vec, index, helper.Mod.EnableCon, null);
                    helper.AddNodeToModule(vecAssign);

                    refContainer = vecAssign.GetAssignIO();
                }
            }
            else
            {
                throw new NotImplementedException();
            }

            if (refContainer is IO.MemPort memPort)
            {
                //Memory ports in high level firrtl are acceses in a different
                //way compared to low level firrtl. In high level firrtl, a
                //memory port is treated like a wire connected to its datain/out
                //sub field whereas in low level firrtl the subfield has to be
                //specified.
                if (memPort.FromHighLevelFIRRTL)
                {
                    return GetIOGender(helper, memPort, ioFlow);
                }
            }
            else
            {
                //Never return bigender io. Only this method should have to deal
                //with that mess so the rest of the code doesn't have to.
                //Dealing with it is ugly which is why i want to contain it.
                if (refContainer is IO.FIRIO firIO)
                {
                    return GetIOGender(helper, firIO, ioFlow);
                }
            }

            return refContainer;
        }

        private static IO.FIRIO GetIOGender(VisitHelper helper, IO.FIRIO io, IO.FlowChange ioFlow)
        {
            if (io is IO.Sink input && ioFlow == IO.FlowChange.Source)
            {
                string duplexOutputName = helper.Mod.GetDuplexOutputName(input);

                //Try see if it was already created
                if (input.GetModResideIn().TryGetIO(duplexOutputName, out var wireOut))
                {
                    return (IO.Source)wireOut;
                }

                //Duplex output for this input wasn't created before so make it now.
                //Make it in the module that the input comes from so there won't
                //be multiple duplex inputs residing in different cond modules.
                return input.GetModResideIn().AddDuplexOuputWire(input);
            }

            return io.GetAsFlow(ioFlow);
        }
    }
}
