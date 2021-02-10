using System.Collections.Generic;

namespace ChiselDebug.Routing
{
    public class WirePath
    {
        public readonly List<Point> Path = new List<Point>();
        public readonly bool StartsFromWire;

        public WirePath(List<Point> path, bool startsFromWire)
        {
            this.Path = path;
            this.StartsFromWire = startsFromWire;
        }


    }
}
