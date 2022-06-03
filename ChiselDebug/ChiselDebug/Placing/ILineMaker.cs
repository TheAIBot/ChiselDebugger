using ChiselDebug.Utilities;
using System.Collections.Generic;

namespace ChiselDebug.Placing
{
    internal interface ILineMaker
    {
        AxisLineContainer CreateLines(List<Point> corners);
    }
}
