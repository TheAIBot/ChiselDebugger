using ChiselDebug.Utilities;
using System.Collections.Generic;

namespace ChiselDebug.Placing
{
    internal sealed class HorizontalLineMaker : ILineMaker
    {
        private readonly LineMaker _lineMaker = new LineMaker(x => new Point(x.X, x.Y));

        public AxisLineContainer CreateLines(List<Point> corners)
        {
            return _lineMaker.CreateLines(corners);
        }
    }
}
