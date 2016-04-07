
using System;
using Microsoft.SPOT;

namespace GadgeteerApp5
{
    public class UnitBound
    {
        public Enums.MinMax MinMax;
        public Enums.Unit Unit;

        public double Value;

        public UnitBound() { }

        public UnitBound(Enums.MinMax minMax, Enums.Unit unit, double value)
        {
            this.MinMax = minMax;
            this.Unit = unit;
            this.Value = value;
        }
    }
}
