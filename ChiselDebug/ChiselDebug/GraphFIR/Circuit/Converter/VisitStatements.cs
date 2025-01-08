using ChiselDebug.GraphFIR.Components;
using System;

namespace ChiselDebug.GraphFIR.Circuit.Converter
{
    public static partial class CircuitToGraph
    {
        private static void VisitStatement(IVisitHelper helper, FIRRTL.IStatement statement)
        {
            if (statement is FIRRTL.EmptyStmt)
            {
                return;
            }
            else if (statement is FIRRTL.Block block)
            {
                for (int i = 0; i < block.Statements.Count; i++)
                {
                    VisitStatement(helper, block.Statements[i]);
                }
            }
            else if (statement is FIRRTL.Conditionally conditional)
            {
                VisitConditional(helper, conditional);
            }
            else if (statement is FIRRTL.Stop stop)
            {
                var clock = (IO.Source)VisitExp(helper, stop.Clk, IO.FlowChange.Source);
                var enable = (IO.Source)VisitExp(helper, stop.Enabled, IO.FlowChange.Source);

                var firStop = new FirStop(clock, enable, stop.Ret, stop);
                helper.AddNodeToModule(firStop);
            }
            else if (statement is FIRRTL.Attach)
            {
                return;
                throw new NotImplementedException();
            }
            else if (statement is FIRRTL.Print)
            {
                return;
                throw new NotImplementedException();
            }
            else if (statement is FIRRTL.Verification)
            {
                return;
                throw new NotImplementedException();
            }
            else if (statement is FIRRTL.Connect connect)
            {
                VisitConnect(helper, connect.Expr, connect.Loc, false);
            }
            else if (statement is FIRRTL.PartialConnect parConnected)
            {
                VisitConnect(helper, parConnected.Expr, parConnected.Loc, true);
            }
            else if (statement is FIRRTL.IsInvalid)
            {
                return;
                throw new NotImplementedException();
            }
            else if (statement is FIRRTL.CDefMemory cmem)
            {
                //If have access to low firrth graph then get memory definition
                //from it as it includes all port definitions. This avoids having
                //to infer memory port types.
                if (helper.HasLowFirGraph())
                {
                    var lowFirMem = (FIRRTL.DefMemory)helper.GetDefNodeFromLowFirrtlGraph(cmem.Name);
                    VisitStatement(helper, lowFirMem);

                    //Low level firrtl addresses the ports through the memory but
                    //high level firrtl directly addreses the ports. Need to
                    //make the ports directly addresseable which is why this is done.
                    var lowMem = (IO.MemoryIO)helper.Mod.GetIO(cmem.Name);
                    foreach (IO.MemPort port in lowMem.GetAllPorts())
                    {
                        port.FromHighLevelFIRRTL = true;
                        helper.Mod.AddMemoryPort(port);
                    }
                }
                else
                {
                    IO.FIRIO inputType = VisitTypeAsPassive(helper, FIRRTL.Dir.Input, null, cmem.Type);
                    var memory = new Memory(cmem.Name, inputType, cmem.Size, 0, 0, cmem.Ruw, cmem);

                    helper.AddNodeToModule(memory);
                }
            }
            else if (statement is FIRRTL.CDefMPort memPort)
            {
                var memory = (IO.MemoryIO)helper.Mod.GetIO(memPort.Mem);

                //Port may already have been created if the memory used the low firrtl
                //memory definition which contain all ports that will be used
                IO.MemPort port;
                if (memory.TryGetIO(memPort.Name, out var existingPort))
                {
                    port = (IO.MemPort)existingPort;
                }
                else
                {
                    port = memPort.Direction switch
                    {
                        FIRRTL.MPortDir.MInfer => throw new NotImplementedException(),
                        FIRRTL.MPortDir.MRead => memory.AddReadPort(memPort.Name),
                        FIRRTL.MPortDir.MWrite => memory.AddWritePort(memPort.Name),
                        FIRRTL.MPortDir.MReadWrite => memory.AddReadWritePort(memPort.Name),
                        var error => throw new Exception($"Unknown memory port type. Type: {error}")
                    };

                    port.FromHighLevelFIRRTL = true;
                    helper.Mod.AddMemoryPort(port);
                }

                ConnectIO(helper, VisitExp(helper, memPort.Exps[0], IO.FlowChange.Source), port.Address, false);
                ConnectIO(helper, VisitExp(helper, memPort.Exps[1], IO.FlowChange.Source), port.Clock, false, false);
                ConnectIO(helper, helper.ScopeEnabledCond, port.Enabled, false);

                //if port has mask then by default set whole mask to true
                if (port.HasMask())
                {
                    IO.FIRIO mask = port.GetMask();
                    IO.Source const1 = (IO.Source)VisitExp(helper, new FIRRTL.UIntLiteral(0, 1), IO.FlowChange.Source);
                    foreach (var maskInput in mask.Flatten())
                    {
                        ConnectIO(helper, const1, maskInput, false);
                    }
                }
            }
            else if (statement is FIRRTL.DefWire defWire)
            {
                IO.FIRIO inputType = VisitTypeAsPassive(helper, FIRRTL.Dir.Output, null, defWire.Type);
                inputType = inputType.ToFlow(IO.FlowChange.Sink, null);
                Wire wire = new Wire(defWire.Name, inputType, defWire);

                helper.AddNodeToModule(wire);
            }
            else if (statement is FIRRTL.DefRegister reg)
            {
                IO.Source clock = (IO.Source)VisitExp(helper, reg.Clock, IO.FlowChange.Source);
                IO.Source reset = null;
                IO.FIRIO initValue = null;

                if (reg.HasResetAndInit())
                {
                    reset = (IO.Source)VisitExp(helper, reg.Reset, IO.FlowChange.Source);
                    initValue = VisitExp(helper, reg.Init, IO.FlowChange.Source);
                }

                IO.FIRIO inputType = VisitTypeAsPassive(helper, FIRRTL.Dir.Input, null, reg.Type);
                Register register = new Register(reg.Name, inputType, clock, reset, initValue, reg);
                helper.AddNodeToModule(register);
            }
            else if (statement is FIRRTL.DefInstance instance)
            {
                Module mod = VisitModule(helper, instance.Name, helper.ModuleRoots[instance.Module]);
                helper.AddNodeToModule(mod);
            }
            else if (statement is FIRRTL.DefNode node)
            {
                var nodeOut = VisitExp(helper, node.Value, IO.FlowChange.Source);

                if (node.Value is not FIRRTL.IRefLikeExpression)
                {
                    nodeOut.SetName(node.Name);
                }

                helper.Mod.AddIORename(node.Name, nodeOut);
            }
            else if (statement is FIRRTL.DefMemory mem)
            {
                IO.FIRIO inputType = VisitTypeAsPassive(helper, FIRRTL.Dir.Input, null, mem.Type);
                var memory = new Memory(mem.Name, inputType, mem.Depth, mem.ReadLatency, mem.WriteLatency, mem.Ruw, mem);

                foreach (var portName in mem.Readers)
                {
                    memory.AddReadPort(portName);
                }
                foreach (var portName in mem.Writers)
                {
                    memory.AddWritePort(portName);
                }
                foreach (var portName in mem.ReadWriters)
                {
                    memory.AddReadWritePort(portName);
                }

                helper.AddNodeToModule(memory);
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        private static void VisitConditional(IVisitHelper parentHelper, FIRRTL.Conditionally conditional)
        {
            Conditional cond = new Conditional(conditional);

            void AddCondModule(IO.Source ena, FIRRTL.IStatement body)
            {
                IVisitHelper helper = parentHelper.ForNewCondModule(parentHelper.GetUniqueName(), null);

                var internalEnaDummy = new DummyPassthrough(ena);
                var internalUseEna = new DummySink(internalEnaDummy.Result);
                helper.AddNodeToModule(internalEnaDummy);
                helper.AddNodeToModule(internalUseEna);
                helper.Mod.SetEnableCond(internalEnaDummy.Result);

                //Set signal that enables this scope as things like memory
                //ports need it
                helper.EnterEnabledScope(internalEnaDummy.Result);

                //Fill out module
                VisitStatement(helper, body);

                cond.AddConditionalModule(helper.Mod);

                helper.ExitEnabledScope();
            }

            IO.Source enableCond = (IO.Source)VisitExp(parentHelper, conditional.Pred, IO.FlowChange.Source);

            if (conditional.HasIf())
            {
                IO.Source ifEnableCond = enableCond;
                if (parentHelper.Mod.IsConditional)
                {
                    FIRAnd chainConditions = new FIRAnd(parentHelper.Mod.EnableCon, enableCond, new FIRRTL.UIntType(1), null);
                    parentHelper.AddNodeToModule(chainConditions);

                    ifEnableCond = chainConditions.Result;
                }

                AddCondModule(ifEnableCond, conditional.WhenTrue);
            }
            if (conditional.HasElse())
            {
                FIRNot notEnableComponent = new FIRNot(enableCond, new FIRRTL.UIntType(1), null);
                parentHelper.AddNodeToModule(notEnableComponent);

                IO.Source elseEnableCond = notEnableComponent.Result;
                if (parentHelper.Mod.IsConditional)
                {
                    FIRAnd chainConditions = new FIRAnd(parentHelper.Mod.EnableCon, elseEnableCond, new FIRRTL.UIntType(1), null);
                    parentHelper.AddNodeToModule(chainConditions);

                    elseEnableCond = chainConditions.Result;
                }

                AddCondModule(elseEnableCond, conditional.Alt);
            }

            parentHelper.AddNodeToModule(cond);
        }
    }
}