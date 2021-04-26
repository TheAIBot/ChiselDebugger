using ChiselDebug.GraphFIR.IO;
using FIRRTL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChiselDebug.GraphFIR
{
    public abstract class PairedIOFIRRTLNode : FIRRTLNode
    {
        private readonly Dictionary<FIRIO, List<FIRIO>> OneToManyPairs = new Dictionary<FIRIO, List<FIRIO>>();

        public PairedIOFIRRTLNode(FirrtlNode defNode) : base(defNode)
        { }

        internal void AddPairedIO(FIRIO io, FIRIO ioFlipped)
        {
            if (io is ScalarIO && ioFlipped is ScalarIO)
            {
                io.SetPaired(ioFlipped);
                ioFlipped.SetPaired(io);
            }
            else
            {
                var ioWalk = io.WalkIOTree();
                var ioFlipWalk = ioFlipped.WalkIOTree();
                foreach (var pair in ioWalk.Zip(ioFlipWalk))
                {
                    pair.First.SetPaired(pair.Second);
                    pair.Second.SetPaired(pair.First);
                }
            }
        }

        internal void AddOneToManyPairedIO(FIRIO io, List<FIRIO> flippedIOs)
        {
            FIRIO[] oneIO = io.WalkIOTree().ToArray();
            FIRIO[][] manyIOs = flippedIOs.Select(x => x.WalkIOTree().ToArray()).ToArray();
            if (!manyIOs.All(x => x.Length == oneIO.Length))
            {
                throw new Exception($"IO must be of the same size.");
            }

            for (int i = 0; i < oneIO.Length; i++)
            {
                List<FIRIO> pairs;
                if (!OneToManyPairs.TryGetValue(oneIO[i], out pairs))
                {
                    pairs = new List<FIRIO>();
                    OneToManyPairs.Add(oneIO[i], pairs);
                }

                for (int y = 0; y < manyIOs.Length; y++)
                {
                    pairs.Add(manyIOs[y][i]);
                }
            }
        }

        public FIRIO GetPairedIO(FIRIO io)
        {
            return io.GetPaired();
        }

        public FIRIO[] GetAllPairedIO(FIRIO io)
        {
            List<FIRIO> allPairs = new List<FIRIO>();
            if (io.GetPaired() != null)
            {
                allPairs.Add(io.GetPaired());
            }
            if (OneToManyPairs.TryGetValue(io, out var multiplePairs))
            {
                allPairs.AddRange(multiplePairs);
            }

            return allPairs.ToArray();
        }

        public bool IsPartOfPair(FIRIO io)
        {
            return io.GetPaired() != null || OneToManyPairs.ContainsKey(io);
        }
    }
}
