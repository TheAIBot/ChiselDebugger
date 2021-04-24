using ChiselDebug.GraphFIR.IO;
using FIRRTL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChiselDebug.GraphFIR
{
    public record ConditionalModule(Input Enable, Module Mod);

    public class Conditional : FIRRTLNode, IContainerIO
    {
        private readonly List<ConditionalModule> ConditionalModules = new List<ConditionalModule>();
        public IReadOnlyList<ConditionalModule> CondMods => ConditionalModules;

        public Conditional(FirrtlNode defNode) : base(defNode) { }

        public void AddConditionalModule(Input enable, Module mod)
        {
            ConditionalModules.Add(new ConditionalModule(enable, mod));
        }

        public override Input[] GetInputs()
        {
            List<Input> inputs = new List<Input>();
            foreach (var condMod in ConditionalModules)
            {
                inputs.Add(condMod.Enable);
                inputs.AddRange(condMod.Mod.GetInputs());
            }

            return inputs.ToArray();
        }

        public override Output[] GetOutputs()
        {
            List<Output> outputs = new List<Output>();
            foreach (var condMod in ConditionalModules)
            {
                outputs.AddRange(condMod.Mod.GetOutputs());
            }

            return outputs.ToArray();
        }

        public override FIRIO[] GetIO()
        {
            List<FIRIO> io = new List<FIRIO>();
            foreach (var condMod in ConditionalModules)
            {
                io.Add(condMod.Enable);
                io.AddRange(condMod.Mod.GetIO());
            }

            return io.ToArray();
        }

        public bool TryGetIO(string ioName, bool modulesOnly, out IContainerIO container)
        {
            foreach (var condMod in ConditionalModules)
            {
                if (condMod.Mod.TryGetIO(ioName, modulesOnly, out container))
                {
                    return true;
                }
            }

            container = null;
            return false;
        }

        public IContainerIO GetIO(string ioName, bool modulesOnly = false)
        {
            if (TryGetIO(ioName, modulesOnly, out var container))
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
                condMod.Enable.InferType();
                condMod.Mod.InferType();
            }
        }
    }
}
