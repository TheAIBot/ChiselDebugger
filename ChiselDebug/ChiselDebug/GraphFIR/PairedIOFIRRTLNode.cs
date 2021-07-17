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
            if (io is ScalarIO ioScalar && ioFlipped is ScalarIO ioFlipScalar)
            {
                ioScalar.SetPaired(ioFlipScalar);
                ioFlipScalar.SetPaired(ioScalar);
            }
            else
            {
                var ioWalk = io.WalkIOTree();
                var ioFlipWalk = ioFlipped.WalkIOTree();
                foreach (var pair in ioWalk.Zip(ioFlipWalk))
                {
                    if (pair.First is ScalarIO firstScalar && pair.Second is ScalarIO secondScalar)
                    {
                        firstScalar.SetPaired(secondScalar);
                        secondScalar.SetPaired(firstScalar);
                    }
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

        public FIRIO[] GetAllPairedIO(ScalarIO io)
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
    }
}
