using ChiselDebug.GraphFIR.IO;
using FIRRTL;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ChiselDebug.GraphFIR.Components
{
    public abstract class PairedIOFIRRTLNode : FIRRTLNode
    {
        private readonly Dictionary<ScalarIO, List<ScalarIO>> OneToManyPairs = new Dictionary<ScalarIO, List<ScalarIO>>();

        public PairedIOFIRRTLNode(FirrtlNode defNode) : base(defNode)
        { }

        internal void AddPairedIO(FIRIO io, FIRIO ioFlipped)
        {
            //This is here to avoid allocations when both a scalars
            if (io is ScalarIO ioScalar && ioFlipped is ScalarIO ioFlipScalar)
            {
                ioScalar.SetPaired(ioFlipScalar);
                ioFlipScalar.SetPaired(ioScalar);
                return;
            }

            foreach (var pair in io.Flatten().Zip(ioFlipped.Flatten()))
            {
                pair.First.SetPaired(pair.Second);
                pair.Second.SetPaired(pair.First);
            }
        }

        internal void AddOneToManyPairedIO(FIRIO io, List<FIRIO> flippedIOs)
        {
            List<ScalarIO> oneIO = io.Flatten();
            List<ScalarIO>[] manyIOs = flippedIOs.Select(x => x.Flatten()).ToArray();
            if (!manyIOs.All(x => x.Count == oneIO.Count))
            {
                throw new Exception($"IO must be of the same size.");
            }

            for (int i = 0; i < oneIO.Count; i++)
            {
                List<ScalarIO> pairs;
                if (!OneToManyPairs.TryGetValue(oneIO[i], out pairs))
                {
                    pairs = new List<ScalarIO>();
                    OneToManyPairs.Add(oneIO[i], pairs);
                }

                for (int y = 0; y < manyIOs.Length; y++)
                {
                    pairs.Add(manyIOs[y][i]);
                }
            }
        }

        public ScalarIO[] GetAllPairedIO(ScalarIO io)
        {
            List<ScalarIO> allPairs = new List<ScalarIO>();
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
    }
}
