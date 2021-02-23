using System.Collections.Generic;

namespace VCDReader
{
    public interface ISimCmd { }

    public record DumpAll(List<IValueChange> Values) : ISimCmd;
    public record DumpOn() : ISimCmd;
    public record DumpOff() : ISimCmd;
    public record DumpVars(List<IValueChange> InitialValues) : ISimCmd;
    public record SimTime(int Time) : ISimCmd;
}
