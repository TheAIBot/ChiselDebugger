using System.Collections.Generic;

namespace VCDReader
{
    public interface ISimCmd { }

    public record DumpAll(List<IValueChange> Values) : ISimCmd;
    public record DumpOn(List<IValueChange> Values) : ISimCmd;
    public record DumpOff(List<IValueChange> Values) : ISimCmd;
    public record DumpVars(List<IValueChange> InitialValues) : ISimCmd;
    public record SimTime(int Time) : ISimCmd;
}
