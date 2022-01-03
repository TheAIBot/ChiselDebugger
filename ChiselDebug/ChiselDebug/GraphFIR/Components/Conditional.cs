using ChiselDebug.GraphFIR.IO;
using FIRRTL;
using System;
using System.Collections.Generic;

namespace ChiselDebug.GraphFIR.Components
{
    public sealed class Conditional : FIRRTLNode, IContainerIO
    {
        private readonly List<Module> ConditionalModules = new List<Module>();
        public IReadOnlyList<Module> CondMods => ConditionalModules;

        public Conditional(FirrtlNode defNode) : base(defNode) { }

        public void AddConditionalModule(Module mod)
        {
            ConditionalModules.Add(mod);
        }

        public void RemoveConditionalModule(Module mod)
        {
            if (!ConditionalModules.Remove(mod))
            {
                throw new ArgumentException(nameof(mod), "The module is not part of this conditional.");
            }
        }

        public override Sink[] GetSinks()
        {
            List<Sink> inputs = new List<Sink>();
            foreach (var condMod in ConditionalModules)
            {
                inputs.AddRange(condMod.GetSinks());
            }

            return inputs.ToArray();
        }

        public override Source[] GetSources()
        {
            List<Source> outputs = new List<Source>();
            foreach (var condMod in ConditionalModules)
            {
                outputs.AddRange(condMod.GetSources());
            }

            return outputs.ToArray();
        }

        public override IEnumerable<FIRIO> GetIO()
        {
            foreach (var condMod in ConditionalModules)
            {
                foreach (var childIO in condMod.GetIO())
                {
                    yield return childIO;
                }
            }
        }

        public bool TryGetIO(string ioName, out IContainerIO container)
        {
            foreach (var condMod in ConditionalModules)
            {
                if (condMod.TryGetIO(ioName, out container))
                {
                    return true;
                }
            }

            container = null;
            return false;
        }

        public IContainerIO GetIO(string ioName)
        {
            if (TryGetIO(ioName, out var container))
            {
                return container;
            }

            throw new Exception($"Failed to find io. IO name: {ioName}");
        }

        public override void Compute()
        {
            throw new Exception("This node is not computable");
        }

        internal override void InferType()
        {
            foreach (var condMod in ConditionalModules)
            {
                condMod.InferType();
            }
        }
    }
}
