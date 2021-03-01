using System.Collections.Generic;

namespace VCDReader
{
    public interface ISimCmd { }

    public record DumpAll(List<VarValue> Values) : ISimCmd;
    public record DumpOn(List<VarValue> Values) : ISimCmd;
    public record DumpOff(List<VarValue> Values) : ISimCmd;
    public record DumpVars(List<VarValue> InitialValues) : ISimCmd;
    public record SimTime(ulong Time) : ISimCmd;
}
