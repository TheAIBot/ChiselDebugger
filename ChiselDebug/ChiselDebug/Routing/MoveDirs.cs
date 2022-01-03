using System;

namespace ChiselDebug.Routing
{
    [Flags]
    public enum MoveDirs : byte
    {
        None = 0b0000,
        Up = 0b0001,
        Down = 0b0010,
        Left = 0b0100,
        Right = 0b1000,
        All = 0b1111,
        ExceptUp = unchecked((byte)~Up),
        ExceptDown = unchecked((byte)~Down),
        ExceptLeft = unchecked((byte)~Left),
        ExceptRight = unchecked((byte)~Right),
        FriendWire = 0b0001_0000,
        EnemyWire = 0b0010_0000,
        WireCorner = 0b0100_0000
    }
}
