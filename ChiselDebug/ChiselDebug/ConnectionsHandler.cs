using ChiselDebug.FIRRTL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChiselDebug
{
    public class ConnectionsHandler
    {
        private readonly Module Mod;
        private readonly List<Connection> UsedModuleConnections;
        private readonly Dictionary<Input, Point> InputPositions = new Dictionary<Input, Point>();
        private readonly Dictionary<Output, Point> OutputPositions = new Dictionary<Output, Point>();
        private readonly HashSet<FIRRTLNode> MissingIOFromNodes = new HashSet<FIRRTLNode>();

        public delegate void HasIOFromAllNodesHandler();
        public event HasIOFromAllNodesHandler OnHasIOFromAllNodes;

        public ConnectionsHandler(Module mod)
        {
            this.Mod = mod;
            this.UsedModuleConnections = Mod.GetAllModuleConnections();
            ResetMissingNodes();
        }

        public void UpdateIOFromNode(FIRRTLNode node, List<Positioned<Input>> inputPoses, List<Positioned<Output>> outputPoses)
        {
            foreach (var inputPos in inputPoses)
            {
                InputPositions[inputPos.Value] = inputPos.Position;
            }
            foreach (var outputPos in outputPoses)
            {
                OutputPositions[outputPos.Value] = outputPos.Position;
            }

            MissingIOFromNodes.Remove(node);
            if (MissingIOFromNodes.Count == 0)
            {
                OnHasIOFromAllNodes?.Invoke();
            }
        }

        public List<Line> GetAllConnectionLines()
        {
            List<Line> lines = new List<Line>();
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
                    lines.Add(new Line(start, end));
                }
            }

            return lines;
        }

        public void ResetMissingNodes()
        {
            MissingIOFromNodes.Clear();
            foreach (var node in Mod.GetAllNodes())
            {
                MissingIOFromNodes.Add(node);
            }
            MissingIOFromNodes.Add(Mod);
        }
    }
}
