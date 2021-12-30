using ChiselDebug.GraphFIR.IO;
using FIRRTL;
using System;
using System.Collections.Generic;
using VCDReader;

namespace ChiselDebug.GraphFIR.Components
{
    public sealed class FirStop : FIRRTLNode
    {
        public readonly Input Clock;
        public readonly Input Enable;
        public readonly int ExitCode;
        public bool SignalStop { get; private set; }

        public FirStop(Output clock, Output enable, int exitCode, FirrtlNode defNode) : base(defNode)
        {
            Clock = new Input(this, new ClockType());
            Enable = new Input(this, new UIntType(1));
            ExitCode = exitCode;

            clock.ConnectToInput(Clock);
            enable.ConnectToInput(Enable);
        }

        public override void Compute()
        {
            ref BinaryVarValue clockVal = ref Clock.UpdateValueFromSourceFast();
            ref BinaryVarValue enableVal = ref Enable.UpdateValueFromSourceFast();

            SignalStop = clockVal.Bits[0] == BitState.One && enableVal.Bits[0] == BitState.One;
        }

        public override Input[] GetInputs()
        {
            return new Input[] { Clock, Enable };
        }

        public override Output[] GetOutputs()
        {
            return Array.Empty<Output>();
        }

        public override IEnumerable<FIRIO> GetIO()
        {
            yield return Clock;
            yield return Enable;
        }

        internal override void InferType()
        { }
    }
}
