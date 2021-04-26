﻿namespace VCDReader
{
    public interface IDeclCmd { }

    public record Comment(string Content) : IDeclCmd, ISimCmd;
    public record Date(string Content) : IDeclCmd;
    public record Scope(ScopeType Type, string Name) : IDeclCmd;
    public record TimeScale(int Scale, TimeUnit Unit) : IDeclCmd;
    public record UpScope() : IDeclCmd;
    public record Version(string VersionText, string SystemTask) : IDeclCmd;

    public class VarDef : IDeclCmd
    {
        public readonly VarType Type;
        public readonly int Size;
        public readonly string ID;
        public readonly string Reference;
        public readonly Scope[] Scopes;

        public VarDef(VarType type, int size, string id, string reference, Scope[] scopes)
        {
            this.Type = type;
            this.Size = size;
            this.ID = id;
            this.Reference = reference;
            this.Scopes = scopes;
        }
    }
}
