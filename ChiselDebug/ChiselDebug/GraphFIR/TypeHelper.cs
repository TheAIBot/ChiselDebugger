﻿using ChiselDebug.GraphFIR.IO;
using FIRRTL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChiselDebug.GraphFIR
{
    internal static class TypeHelper
    {
        public static GroundType InferMaxWidthType(Output output, PairedIOFIRRTLNode pairNode)
        {
            return InferMaxWidthType(pairNode.GetAllPairedIO(output));
        }

        public static GroundType InferMaxWidthType(FIRIO[] ioEndPoints)
        {
            List<GroundType> inputTypes = new List<GroundType>();
            foreach (FIRIO endPoint in ioEndPoints)
            {
                if (endPoint is not ScalarIO scalar)
                {
                    continue;
                }

                scalar.InferType();
                if (scalar.Type != null)
                {
                    inputTypes.Add(scalar.Type);
                }
            }

            if (inputTypes.Count == 0)
            {
                return null;
            }

            int maxWidth = inputTypes.Max(x => x.Width);
            if (inputTypes.All(x => x is UIntType))
            {
                return new UIntType(maxWidth);
            }
            else if (inputTypes.All(x => x is SIntType))
            {
                return new SIntType(maxWidth);
            }
            else if (inputTypes.All(x => x is ClockType))
            {
                return new ClockType();
            }
            else
            {
                throw new Exception("All input types for mux output must be of the same ground type.");
            }
        }
    }
}
