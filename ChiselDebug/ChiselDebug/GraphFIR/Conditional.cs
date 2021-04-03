using ChiselDebug.GraphFIR.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChiselDebug.GraphFIR
{

    public class Conditional : FIRRTLNode, IContainerIO
    {
        private readonly List<(Input enable, Module mod)> ConditionalModules = new List<(Input enable, Module mod)>();

        public void AddConditionalModule(Output enable, Module mod)
        {
            Input input = new Input(this, new FIRRTL.UIntType(1));
            enable.ConnectToInput(input);

            ConditionalModules.Add((input, mod));
        }

        public override ScalarIO[] GetInputs()
        {
            List<ScalarIO> inputs = new List<ScalarIO>();
            foreach (var condMod in ConditionalModules)
            {
                inputs.Add(condMod.enable);
                inputs.AddRange(condMod.mod.GetInputs());
            }

            return inputs.ToArray();
        }

        public override ScalarIO[] GetOutputs()
        {
            List<ScalarIO> outputs = new List<ScalarIO>();
            foreach (var condMod in ConditionalModules)
            {
                outputs.AddRange(condMod.mod.GetOutputs());
            }

            return outputs.ToArray();
        }

        public override FIRIO[] GetIO()
        {
            List<FIRIO> io = new List<FIRIO>();
            foreach (var condMod in ConditionalModules)
            {
                io.Add(condMod.enable);
                io.AddRange(condMod.mod.GetIO());
            }

            return io.ToArray();
        }

        public bool TryGetIO(string ioName, bool modulesOnly, out IContainerIO container)
        {
            foreach (var condMod in ConditionalModules)
            {
                if (condMod.mod.TryGetIO(ioName, modulesOnly, out container))
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

        public override void InferType()
        {
            foreach (var condMod in ConditionalModules)
            {
                condMod.enable.InferType();
                condMod.mod.InferType();
            }
        }
    }
}
