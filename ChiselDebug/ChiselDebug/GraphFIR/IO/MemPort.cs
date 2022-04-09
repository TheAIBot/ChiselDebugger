using ChiselDebug.GraphFIR.Components;
using FIRRTL;
using System.Collections.Generic;

namespace ChiselDebug.GraphFIR.IO
{
    public abstract class MemPort : IOBundle
    {
        internal readonly FIRIO Address;
        internal readonly FIRIO Enabled;
        internal readonly FIRIO Clock;
        internal bool FromHighLevelFIRRTL = false;
        private Dictionary<Sink, Sink> DataToMask;

        public MemPort(FIRRTLNode node, string name, List<FIRIO> io) : base(node, name, io)
        {
            this.Address = (FIRIO)GetIO("addr");
            this.Enabled = (FIRIO)GetIO("en");
            this.Clock = (FIRIO)GetIO("clk");
        }

        protected void InitDataToMask()
        {
            DataToMask = new Dictionary<Sink, Sink>();
            Sink[] dataIO = GetSink().FlattenTo<Sink>();
            Sink[] maskIO = GetMask().FlattenTo<Sink>();

            for (int i = 0; i < dataIO.Length; i++)
            {
                DataToMask.Add(dataIO[i], maskIO[i]);
            }
        }

        protected static void AsMaskType(FIRIO maskFrom)
        {
            foreach (ScalarIO scalar in maskFrom.Flatten())
            {
                scalar.SetType(new UIntType(1));
            }
        }

        internal abstract bool HasMask();
        internal abstract FIRIO GetMask();

        internal Sink GetMaskFromDataInput(Sink input)
        {
            return DataToMask[input];
        }
    }
}
