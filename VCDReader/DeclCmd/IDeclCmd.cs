namespace VCDReader
{
    public interface IDeclCmd { }

    public record Comment(string Content) : IDeclCmd, ISimCmd;
    public record Date(string Content) : IDeclCmd;
    public record Scope(ScopeType Type, string Name) : IDeclCmd;
    public record TimeScale(int Scale, TimeUnit Unit) : IDeclCmd;
    public record UpScope() : IDeclCmd;
    public record VarDef(VarType Type, int Size, string ID, string Reference, Scope[] Scopes) : IDeclCmd;
    public record Version(string VersionText, string SystemTask) : IDeclCmd;
}
