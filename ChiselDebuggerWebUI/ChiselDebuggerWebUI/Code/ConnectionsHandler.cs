using ChiselDebug;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChiselDebuggerWebUI.Code
{
    public class ConnectionsHandler
    {
        private readonly List<Connection> UsedModuleConnections;
        private readonly Dictionary<Input, Point> InputPositions = new Dictionary<Input, Point>();
        private readonly Dictionary<Output, Point> OutputPositions = new Dictionary<Output, Point>();
        

        public ConnectionsHandler(List<Connection> usedModuleConnections)
        {
            this.UsedModuleConnections = usedModuleConnections;
        }

        public void UpdateInputPos(List<Positioned<Input>> inputPoses)
        {
            foreach (var inputPos in inputPoses)
            {
                UpdateInputPos(inputPos);
            }
        }

        public void UpdateOutputPos(List<Positioned<Output>> outputPoses)
        {
            foreach (var outputPos in outputPoses)
            {
                UpdateOutputPos(outputPos);
            }
        }

        public void UpdateInputPos(Positioned<Input> inputPos)
        {
            InputPositions[inputPos.Value] = inputPos.Position;
        }

        public void UpdateOutputPos(Positioned<Output> outputPos)
        {
            OutputPositions[outputPos.Value] = outputPos.Position;
        }

        public List<(Point start, Point end)> GetAllConnectionLines()
        {
            List<(Point, Point)> lines = new List<(Point, Point)>();
            foreach (var connection in UsedModuleConnections)
            {
                if (!OutputPositions.ContainsKey(connection.From))
                {
                    continue;
                }

                Point start = OutputPositions[connection.From];
                foreach (var input in connection.To)
                {
                    if (!InputPositions.ContainsKey(input))
                    {
                        continue;
                    }

                    Point end = InputPositions[input];
                    lines.Add((start, end));
                }
            }

            return lines;
        }
    }
}
