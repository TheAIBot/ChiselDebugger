using System.Collections.Generic;

namespace ChiselDebug.GraphFIR.IO
{
    public static class IOExtensions
    {
        public static IEnumerable<ScalarIO> FlattenMany(this IEnumerable<FIRIO> ios)
        {
            List<ScalarIO> scalars = new List<ScalarIO>();
            foreach (var io in ios)
            {
                io.Flatten(scalars);
            }

            return scalars;
        }
    }
}
