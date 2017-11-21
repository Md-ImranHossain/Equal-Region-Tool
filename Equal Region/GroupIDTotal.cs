using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Equal_Region
{
    public class GroupIDTotal
    {

        private short ID;
        private double CalculatedSplitValue;

        public GroupIDTotal(short ID, double CalculatedSplitValue)
        {
            this.ID = ID;
            this.CalculatedSplitValue = CalculatedSplitValue;
        }

        public GroupIDTotal()
        {

        }

        public double getCalculatedSplitValue()
        {
            return this.CalculatedSplitValue;

        }

        public void setCalculatedSplitValue(double Value)
        {
            this.CalculatedSplitValue = Value;

        }

        public short getID()
        {
            return this.ID;
        }

        public void setID(short ID)
        {
            this.ID = ID;

        }
	
    }
}
