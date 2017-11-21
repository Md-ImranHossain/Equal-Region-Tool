using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geometry;
using System.Collections;
using System.Windows.Forms;

namespace Equal_Region
{
    class F2FDistance
    {

         

        public static void MakeDistanceArray(IFeature InputFeature, int GruopID) 
        {

            List<DistanceFeatureClass> DistFeatureList = new List<DistanceFeatureClass>();
            IEnumLayer pEnumLayer = Form1.pMap.Layers;
            ILayer pLayer;

            for (int i = 0; i < Form1.pMap.LayerCount; i++)
            {

                pLayer = pEnumLayer.Next();


                if (pLayer.Name.Equals(Form1.OperationLayer))
                {

                    IFeatureLayer2 pFeatureLayer = (IFeatureLayer2)pLayer;
                    IFeatureClass pFeatureClass = pFeatureLayer.FeatureClass;
                    
                    IQueryFilter pQueryFilter = new QueryFilter();
                    pQueryFilter.WhereClause = "SpltGropus = 0";

                    IFeatureCursor pFeatureCursor = pFeatureClass.Search(pQueryFilter, false);

                    IFeature CheckFeature = pFeatureCursor.NextFeature();

                    while (CheckFeature != null)
                    {
                        IProximityOperator pProxOper = (IProximityOperator)CheckFeature.Shape;
                        double distance = pProxOper.ReturnDistance(InputFeature.Shape);
                        CheckFeature.set_Value(CheckFeature.Fields.FindField("Distance"), distance);
                        CheckFeature.Store();


                        DistanceFeatureClass DistFeature = new DistanceFeatureClass(distance, CheckFeature);
                        DistFeatureList.Add(DistFeature);

                            

                        CheckFeature = pFeatureCursor.NextFeature();
                    }


                    DistFeatureList = DistFeatureList.OrderBy(a => a.getDistance()).ToList();
                    
                    double formarValue = 0;
                    double addValue = 0;

                    for (int j = 0; j < DistFeatureList.Count; j++)
                    {

                        //MessageBox.Show("..." + Form1.Group.getCalculatedSplitValue() + "..." + Form1.EqualPartValue, "hi");
                        if (Form1.GroupIDTotalList[GruopID-1].getCalculatedSplitValue() < Form1.EqualPartValue)
                        {


                            formarValue = Form1.GroupIDTotalList[GruopID-1].getCalculatedSplitValue();
                            addValue = Convert.ToDouble(DistFeatureList[j].getFeature().get_Value(DistFeatureList[j].getFeature().Fields.FindField(Form1.OperationAttribute)));

                            DistFeatureList[j].getFeature().set_Value(DistFeatureList[j].getFeature().Fields.FindField("SpltGropus"), GruopID);
                            DistFeatureList[j].getFeature().Store();

                            Form1.GroupIDTotalList[GruopID - 1].setCalculatedSplitValue(formarValue + addValue);

                        }
                        //Form1.Group.setCalculatedSplitValue(DistFeatureList[j].getFeature().get_Value(DistFeatureList[j].getFeature().Fields.FindField("SpltGropus")));
                        //DistFeatureList[j].set_Value(FeatureList[j].Fields.FindField("SpltGropus"), 10);
                        //FeatureList[j].Store();

                        //MessageBox.Show("..." + DistFeatureList[j].getDistance(), "hi"); 
                    }

                    Form1.RegionFormed = Form1.RegionFormed + 1;
                    //MessageBox.Show("..." + ((Form1.NoOfSplit - 1) == Form1.RegionFormed), "hi"); 
                    if ((Form1.NoOfSplit - 1) == Form1.RegionFormed)
                    {
                        IGeoFeatureLayer pGeoFetLayer = (IGeoFeatureLayer)pLayer;
                        Symbology.DefineUniqueValueRenderer(pGeoFetLayer, "SpltGropus");

                    }

                    //Creating convexhull of the generated region ??? if does not work you have to make the concave hull??
                    IQueryFilter RegionQueryFilter = new QueryFilter();
                    RegionQueryFilter.WhereClause = "SpltGropus = " + GruopID;
                    IFeatureCursor RegionFeatureCursor = pFeatureClass.Search(RegionQueryFilter, false);

                    Form1.RegionGeomtry = StaticMethodsClass.CreateConvexHull(RegionFeatureCursor);
                    
                    
                    //IFeature dd = pFeatureClass.CreateFeature();
                    //dd.Shape = pPol;
                    //dd.set_Value(dd.Fields.FindField("SpltGropus"), GruopID); 
                    //dd.Store();

                    //IActiveView activeView = Form1.pMap as IActiveView;
                    //activeView.PartialRefresh(esriViewDrawPhase.esriViewGraphics, null, null);
                    //MessageBox.Show("...", "hi");

                    //
                    //MessageBox.Show("..." , "hi"); 
                }
            }
        
        
        
        
        }








    }
}
