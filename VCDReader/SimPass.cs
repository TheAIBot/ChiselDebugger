using System;

namespace VCDReader
{
    public class SimPass
    {
        public ISimCmd? SimCmd = null;
        public BinaryVarValue BinValue;
        public bool HasBinValue = false;
        public RealVarValue? RealValue = null;

        internal bool HasCmd()
        {
            return SimCmd != null || HasBinValue || RealValue.HasValue;
        }

        internal ISimCmd GetCmd()
        {
            if (SimCmd != null)
            {
                return SimCmd;
            }
            else if (HasBinValue)
            {
                return BinValue;
            }
            else if (RealValue.HasValue)
            {
                return RealValue;
            }
            else
            {
                throw new Exception("Contains to command.");
            }
        }

        internal void Reset()
        {
            SimCmd = null;
            HasBinValue = false;
            RealValue = null;
        }
    }
}
