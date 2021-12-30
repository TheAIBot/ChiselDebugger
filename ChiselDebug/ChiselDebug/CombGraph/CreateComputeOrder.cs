using ChiselDebug.GraphFIR.Components;
using ChiselDebug.GraphFIR.IO;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ChiselDebug.CombGraph
{
    internal static class CreateComputeOrder
    {
        public static CombComputeOrder<Computable> MakeMonoGraph(Module module)
        {
            List<Output> startingPoints = new List<Output>();
            foreach (var childMod in module.GetAllNestedNodesOfType<Module>())
            {
                foreach (var output in childMod.GetInternalOutputs())
                {
                    Input paired = output.GetPaired();
                    if (output.IsConnectedToAnything() && !paired.IsConnectedToAnything())
                    {
                        startingPoints.Add(output);
                    }
                }
                foreach (var input in childMod.GetInternalInputs())
                {
                    Output paired = input.GetPaired();
                    if (paired.IsConnectedToAnything() && !input.IsConnectedToAnything())
                    {
                        startingPoints.Add(paired);
                    }
                }
            }
            foreach (var wire in module.GetAllNestedNodesOfType<Wire>())
            {
                foreach (var output in wire.GetOutputs())
                {
                    Input paired = output.GetPaired();
                    if (!paired.IsConnectedToAnything())
                    {
                        startingPoints.Add(output);
                    }
                }
            }
            foreach (var output in module.GetInternalOutputs())
            {
                if (!startingPoints.Contains(output))
                {
                    startingPoints.Add(output);
                }
            }
            foreach (var constVal in module.GetAllNestedNodesOfType<ConstValue>())
            {
                startingPoints.AddRange(constVal.GetOutputs());
            }
            foreach (var statePres in module.GetAllNestedNodesOfType<IStatePreserving>())
            {
                startingPoints.AddRange(((FIRRTLNode)(statePres)).GetOutputs());
            }

            return MakeCombComputeNode(startingPoints.ToArray());
        }

        private static CombComputeOrder<Computable> MakeCombComputeNode(Output[] outputs)
        {
            HashSet<Output> seenCons = new HashSet<Output>();
            HashSet<SourceSinkCon> seenSourceSinkCons = new HashSet<SourceSinkCon>();
            List<Computable> computeOrder = new List<Computable>();

            BorderBlockers borderBlockers = new BorderBlockers();
            NodeBlockers nodeBlockers = new NodeBlockers();

            foreach (var computeFirst in outputs.Where(x => x.Node is not Module && (x.Node is not IStatePreserving)).Select(x => x.Node).Distinct())
            {
                computeOrder.Add(new Computable(computeFirst));
            }

            Stack<SourceSinkCon> toTraverse = new Stack<SourceSinkCon>();

            void AddSinkToSearch(Output output)
            {
                seenCons.Add(output);
                computeOrder.Add(new Computable(output));

                foreach (var input in output.GetConnectedInputs())
                {
                    SourceSinkCon con = new SourceSinkCon(output, input);
                    if (seenSourceSinkCons.Add(con))
                    {
                        toTraverse.Push(con);
                    }
                }
            }

            foreach (var output in outputs)
            {
                AddSinkToSearch(output);

                while (toTraverse.Count > 0)
                {
                    var conInput = toTraverse.Pop();
                    borderBlockers.TryUnblockWithSource(conInput.Source, AddSinkToSearch, computeOrder);
                    nodeBlockers.TryUnblockWithSource(conInput.Source, AddSinkToSearch, computeOrder);

                    //Punch through module border to continue search on the other side
                    if (conInput.Sink.Node is Module || conInput.Sink.Node is Wire)
                    {
                        borderBlockers.BlockIfAllInputsNotFound(conInput.Source, conInput.Sink, seenCons, AddSinkToSearch, computeOrder);
                    }
                    //Ignore state preserving components as a combinatorial graph
                    //shouldn't cross those
                    else if (conInput.Sink.Node is IStatePreserving)
                    {
                        continue;
                    }
                    else
                    {
                        nodeBlockers.BlockIfAllInputsNotFound(conInput.Source, conInput.Sink.Node, seenCons, AddSinkToSearch, computeOrder);
                    }
                }
            }

            return new CombComputeOrder<Computable>(outputs, computeOrder.ToArray());
        }
    }

    internal abstract class BaseBlockers<T>
    {
        protected readonly Dictionary<T, HashSet<Output>> SeenButMissingSources = new Dictionary<T, HashSet<Output>>();
        protected readonly Dictionary<Output, List<T>> InputBlockers = new Dictionary<Output, List<T>>();

        public void BlockIfAllInputsNotFound(Output source, T target, HashSet<Output> seenCons, Action<Output> addSinkToSearch, List<Computable> computeOrder)
        {
            HashSet<Output> missingCons = GetMissingSources(seenCons, target);
            if (HasFoundAllDeps(source, target, missingCons))
            {
                AddUnblockedToSearch(target, addSinkToSearch, computeOrder);
            }
        }

        private HashSet<Output> GetMissingSources(HashSet<Output> seenCons, T target)
        {
            HashSet<Output> missingCons;
            if (!SeenButMissingSources.TryGetValue(target, out missingCons))
            {
                missingCons = new HashSet<Output>();
                SeenButMissingSources.Add(target, missingCons);

                AddMissingCons(seenCons, missingCons, InputBlockers, target);
            }

            return missingCons;
        }

        protected abstract void AddMissingCons(HashSet<Output> seenCons, HashSet<Output> missingCons, Dictionary<Output, List<T>> blocker, T blocked);
        protected static void AddMissingConnections(HashSet<Output> seenCons, HashSet<Output> missingCons, Input input, Dictionary<Output, List<T>> blocker, T blocked)
        {
            foreach (var con in input.GetConnections())
            {
                if (!seenCons.Contains(con.From))
                {
                    missingCons.Add(con.From);
                }
                if (con.Condition != null && !seenCons.Contains(con.Condition))
                {
                    missingCons.Add(con.Condition);
                    blocker.TryAdd(con.Condition, new List<T>());
                    blocker[con.Condition].Add(blocked);
                }
            }
        }

        private bool HasFoundAllDeps(Output con, T target, HashSet<Output> missingCons)
        {
            missingCons.Remove(con);

            if (missingCons.Count == 0)
            {
                SeenButMissingSources.Remove(target);
                return true;
            }

            return false;
        }

        public void TryUnblockWithSource(Output source, Action<Output> addSinkToSearch, List<Computable> computeOrder)
        {
            if (InputBlockers.TryGetValue(source, out var blockedInputs))
            {
                foreach (var blocked in blockedInputs)
                {
                    if (HasFoundAllDeps(source, blocked, SeenButMissingSources[blocked]))
                    {
                        AddUnblockedToSearch(blocked, addSinkToSearch, computeOrder);
                    }
                }
                InputBlockers.Remove(source);
            }
        }
        protected abstract void AddUnblockedToSearch(T target, Action<Output> addSinkToSearch, List<Computable> computeOrder);
    }

    internal class NodeBlockers : BaseBlockers<FIRRTLNode>
    {
        protected override void AddMissingCons(HashSet<Output> seenCons, HashSet<Output> missingCons, Dictionary<Output, List<FIRRTLNode>> blocker, FIRRTLNode blocked)
        {
            foreach (Input input in blocked.GetInputs())
            {
                AddMissingConnections(seenCons, missingCons, input, blocker, blocked);
            }
        }

        protected override void  AddUnblockedToSearch(FIRRTLNode target, Action<Output> addSinkToSearch, List<Computable> computeOrder)
        {
            computeOrder.Add(new Computable(target));

            foreach (var nodeOutput in target.GetOutputs())
            {
                addSinkToSearch(nodeOutput);
            }
        }
    }

    internal class BorderBlockers : BaseBlockers<Input>
    {
        protected override void AddMissingCons(HashSet<Output> seenCons, HashSet<Output> missingCons, Dictionary<Output, List<Input>> blocker, Input blocked)
        {
            AddMissingConnections(seenCons, missingCons, blocked, blocker, blocked);
        }

        protected override void AddUnblockedToSearch(Input target, Action<Output> addSinkToSearch, List<Computable> computeOrder)
        {
            addSinkToSearch(target.GetPaired());
        }
    }

    internal readonly struct SourceSinkCon
    {
        public readonly Output Source;
        public readonly Input Sink;

        public SourceSinkCon(Output source, Input sink)
        {
            this.Source = source;
            this.Sink = sink;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Source, Sink);
        }
    }
}
