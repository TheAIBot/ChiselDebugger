using ChiselDebug.GraphFIR.IO;
using FIRRTL;
using System;
using System.Collections.Generic;
using VCDReader;
#nullable enable

namespace ChiselDebug.GraphFIR.Components
{
    public sealed class FirStop : FIRRTLNode
    {
        public readonly Sink Clock;
        public readonly Sink Enable;
        public readonly int ExitCode;
        public bool SignalStop { get; private set; }

        public FirStop(Source clock, Source enable, int exitCode, FirrtlNode defNode) : base(defNode)
        {
            Clock = new Sink(this, new ClockType());
            Enable = new Sink(this, new UIntType(1));
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

        public override Sink[] GetSinks()
        {
            return new Sink[] { Clock, Enable };
        }

        public override Source[] GetSources()
        {
            return Array.Empty<Source>();
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
