﻿namespace ChiselDebug.GraphFIR.IO
{
    public abstract class AggregateIO : FIRIO 
    { 
        public AggregateIO(string name) : base(name) { }

        public override FIRIO GetInput()
        {
            return this;
        }

        public override FIRIO GetOutput()
        {
            return this;
        }

        public abstract FIRIO[] GetIOInOrder();

        public abstract bool IsVisibleAggregate();
    }
}
