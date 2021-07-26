namespace ChiselDebug.GraphFIR.IO
{
    public abstract class AggregateIO : FIRIO 
    { 
        public AggregateIO(FIRRTLNode node, string name) : base(node, name) { }

        public override FIRIO GetInput()
        {
            return this;
        }

        public override FIRIO GetOutput()
        {
            return this;
        }

        public abstract FIRIO[] GetIOInOrder();
    }
}
