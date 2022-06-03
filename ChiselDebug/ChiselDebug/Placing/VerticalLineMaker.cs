using ChiselDebug.Utilities;
using System.Collections.Generic;

namespace ChiselDebug.Placing
{
    internal sealed class VerticalLineMaker : ILineMaker
    {
        private readonly LineMaker _lineMaker = new LineMaker(x => new Point(x.Y, x.X));

        public AxisLineContainer CreateLines(List<Point> corners)
        {
            return _lineMaker.CreateLines(corners);
        }
    }
}
