using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.Geodatabase;

namespace Equal_Region
{
    class DistanceFeatureClass
    {
        private double Distance;
        private IFeature Feature;

        public DistanceFeatureClass(double Distance,IFeature Feature) {

            this.Distance = Distance;
            this.Feature = Feature;
        
        }

        public double getDistance()
        {
            return this.Distance;

        }

        public IFeature getFeature()
        {
            return this.Feature;
        }
	





    }
}
