using System;

namespace VCDReader
{
    public class SimPass
    {
        public ISimCmd? SimCmd = null;
        public BinaryVarValue? BinValue = null;
        public RealVarValue? RealValue = null;

        internal bool HasCmd()
        {
            return SimCmd != null || BinValue.HasValue || RealValue.HasValue;
        }

        internal ISimCmd GetCmd()
        {
            if (SimCmd != null)
            {
                return SimCmd;
            }
            else if (BinValue.HasValue)
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
            BinValue = null;
            RealValue = null;
        }
    }
}
