using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Carto;
using System.Windows.Forms;

namespace Equal_Region
{
   public class StaticMethodsClass

    {

      
       public static IGeometry CreateConvexHull (IFeatureCursor pFeatureCursor){

           IGeometryCollection pGeomCollection = (IGeometryCollection)new Polygon();
           IGeometryCollection pGeomCollectionIn;

           IFeature pFeature = pFeatureCursor.NextFeature();

           while (pFeature != null) {

               pGeomCollectionIn = (IGeometryCollection) pFeature.ShapeCopy;

               for (int i = 0; i < pGeomCollectionIn.GeometryCount; i++)
               {
                   pGeomCollection.AddGeometry(pGeomCollectionIn.get_Geometry(i));

               }

               pFeature = pFeatureCursor.NextFeature();
   
           }

           ITopologicalOperator pTopOp = (ITopologicalOperator)pGeomCollection;
           //pTopOp.Simplify();
           IGeometry pGeom = pTopOp.ConvexHull();
           
           return pGeom;
       
           
       }

       public static IFeature FindNextStartingFeature(IGeometry RegionGeometry)
       {
           List<DistanceFeatureClass> DistFeatureList = new List<DistanceFeatureClass>();
           IEnumLayer pEnumLayer = Form1.pMap.Layers;
           ILayer pLayer;
           IFeatureSelection pFeatureSelection = null;

           IWorkspaceFactory workspaceFactory = new ESRI.ArcGIS.DataSourcesFile.ShapefileWorkspaceFactoryClass();
           IWorkspace workspace = workspaceFactory.OpenFromFile(System.IO.Path.GetTempPath(), 0);
           IFeatureWorkspace featureWorkspace = (ESRI.ArcGIS.Geodatabase.IFeatureWorkspace)workspace; // Explict Cast
           IFeatureClass featureClass = featureWorkspace.OpenFeatureClass("Temp_ConcaveHull");
           IFeatureCursor AllRegionFeatureCursor = featureClass.Search(null, false);
           IFeature AllRegionFeature = AllRegionFeatureCursor.NextFeature();
           IGeometry AllRegionGeometry = AllRegionFeature.Shape;
           
           IPolygon polygon = (IPolygon)AllRegionGeometry;
           IGeometryCollection pGeomColl = (IGeometryCollection)polygon;
           IGeometryCollection AllRegionPolyline = new PolylineClass();
           ISegmentCollection PsegPath = null;

           object obj = Type.Missing;

           for (int k=0; k<pGeomColl.GeometryCount;k++){
               PsegPath = new ESRI.ArcGIS.Geometry.PathClass();
               PsegPath.AddSegmentCollection( (ISegmentCollection)pGeomColl.get_Geometry(k));
               AllRegionPolyline.AddGeometry((IGeometry)PsegPath, ref obj,ref obj);
           
           }

           IPolyline pPolyline = (IPolyline)AllRegionPolyline;
           //MessageBox.Show("..." + pPolyline.Length, "hi");
           //polygon.

           for (int i = 0; i < Form1.pMap.LayerCount; i++)
           {

               pLayer = pEnumLayer.Next();


               if (pLayer.Name.Equals(Form1.OperationLayer))
               {
                   IFeatureLayer pFeatureLayer = (IFeatureLayer)pLayer;
                   pFeatureSelection = (IFeatureSelection)pFeatureLayer;
             
               }

           }
           
           
           ISpatialFilter pSpatialFilter = new SpatialFilter();
           pSpatialFilter.GeometryField ="SHAPE";
           pSpatialFilter.SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects;
           pSpatialFilter.WhereClause = "SpltGropus = 0";

           ITopologicalOperator pTopologicalOperator = (ITopologicalOperator)RegionGeometry;
           IPolygon pPolygon = (IPolygon)pTopologicalOperator.Buffer(5);
           pSpatialFilter.Geometry = pPolygon;

           pFeatureSelection.SelectFeatures(pSpatialFilter, esriSelectionResultEnum.esriSelectionResultAdd, false);

           ISelectionSet pSelectionSet = pFeatureSelection.SelectionSet;

           IQueryFilter pQueryFilter = new QueryFilter();
           pQueryFilter.WhereClause = "SpltGropus = 0";
           ICursor pCursor;
           pSelectionSet.Search(pQueryFilter, false, out pCursor);
           IFeatureCursor pFeatureCursor = (IFeatureCursor)pCursor;

           IFeature pFeature = pFeatureCursor.NextFeature();
          

           //sorting the feature according to the distance of the RerionGeometry

           while (pFeature != null)
           {
           
               IProximityOperator pProxOper = (IProximityOperator)pFeature.Shape;
               double distance = pProxOper.ReturnDistance(RegionGeometry);
               
               DistanceFeatureClass DistFeature = new DistanceFeatureClass(distance, pFeature);
               DistFeatureList.Add(DistFeature);

               pFeature = pFeatureCursor.NextFeature();
           }


           DistFeatureList = DistFeatureList.OrderBy(a => a.getDistance()).ToList();

           IRelationalOperator AllRegionFeatureGeom = (IRelationalOperator)pPolyline;
           IFeature ReturnFeature = null;
           for (int j = 0; j < DistFeatureList.Count; j++)
           {

               //IRelationalOperator RegionFeatureGeom = (IRelationalOperator)DistFeatureList[j].getFeature().Shape;
               //MessageBox.Show("..." + AllRegionFeatureGeom.Touches(DistFeatureList[j].getFeature().Shape), "hi");
               if (AllRegionFeatureGeom.Touches(DistFeatureList[j].getFeature().Shape) == true)
               {

                   ReturnFeature = DistFeatureList[j].getFeature();
                   break;
               }
       
           }
           //MessageBox.Show("..." + ReturnFeature.OID, "hi");
           return ReturnFeature;

       }

    }
}
