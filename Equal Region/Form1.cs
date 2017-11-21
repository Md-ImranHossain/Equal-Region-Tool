using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ESRI.ArcGIS.ArcMapUI;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.DataManagementTools;
using System.IO;
using ESRI.ArcGIS.Geoprocessor;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.Editor;
using ESRI.ArcGIS.CartographyTools;


namespace Equal_Region
{
    public partial class Form1 : Form
    {

        public static IMxDocument mxDoc;
        public static IBasicDocument2 pBasicDocument;
        public static IMap pMap;

        public static string OperationAttribute;
        public static string OperationLayer;
        public static double Tolerance;
        public static int NoOfSplit;
        public static string workSpacePath;
        //public static GroupIDTotal Group = new GroupIDTotal();
        public static List<GroupIDTotal> GroupIDTotalList = new List<GroupIDTotal>();
        public static IGeometry RegionGeomtry = null;

        private double FieldTotal = 0.00;
        public static double EqualPartValue = 0.00;
        public static int RegionFormed = 0;

        public Form1()
        {
            InitializeComponent();
        }

      
        private void Form1_Load(object sender, EventArgs e)
        {
            comboBox1.SelectedIndex = 0;
            
            //comboBox3.SelectedIndex = 0;

            mxDoc = ArcMap.Document;
            pBasicDocument = (IBasicDocument2)mxDoc;
            pMap = pBasicDocument.FocusMap;
            IEnumLayer pEnumLayer = pMap.Layers;

            ILayer pLayer;

            for (int i = 0; i < pMap.LayerCount; i++)
            {

                pLayer = pEnumLayer.Next();
                comboBox2.Items.Add(pLayer.Name);
            }

            comboBox2.SelectedIndex = 0;


        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.SelectedItem.Equals("Supervised"))
            {
                label6.Enabled = true;
                button1.Enabled = true;

            }
            else if (comboBox1.SelectedItem.Equals("Unsupervised"))
            {
                label6.Enabled = false;
                button1.Enabled = false;
            }
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            comboBox3.Items.Clear();
			comboBox3.Items.Add("-Select-");
           
            IEnumLayer pEnumLayer = pMap.Layers;
			ILayer pLayer;
					
			for (int i = 0; i < pMap.LayerCount; i++){
					
				pLayer = pEnumLayer.Next();
						
		
				if (pLayer.Name.Equals(comboBox2.Text)){
							
					IFeatureLayer2 pFeatureLayer2 = (IFeatureLayer2) pLayer;
					IFeatureClass pFeatureClass = pFeatureLayer2.FeatureClass;
					IFields pFields = pFeatureClass.Fields;
							
					for (int j = 0; j < pFields.FieldCount; j++){
								
						IField pField = pFields.get_Field(j);
						comboBox3.Items.Add(pField.Name);
                       		
					}
				
				}
			}

            comboBox3.SelectedIndex = 0;
        }

  
 

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);
        }

        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);
        }

        private void button1_Click(object sender, EventArgs e)
        {

            UID pUID = new UID();
            UID pUID2 = new UID();
            UID pUID3 = new UID();
            UID pUID4 = new UID();

            pUID.Value= "{488CBE4F-CD34-4649-B1ED-8CD1B8149041}";
            pUID2.Value="{78FFF793-98B4-11D1-873B-0000F8751720}";
            pUID3.Value="{8841A0D9-4F49-11D2-AE2D-080009EC732A}";
            pUID4.Value="{8841A0DA-4F49-11D2-AE2D-080009EC732A}";
            pUID4.SubType=2;

            ICommandItem CommandItem = ArcMap.Application.Document.CommandBars.Find(pUID, false, true);
            ICommandItem CommandItem2 = ArcMap.Application.Document.CommandBars.Find(pUID2, false, true);
            ICommandItem CommandItem3 = ArcMap.Application.Document.CommandBars.Find(pUID3, false, true);
            ICommandItem CommandItem4 = ArcMap.Application.Document.CommandBars.Find(pUID4, false, true);

            CommandItem.Execute();
            CommandItem2.Execute();
            CommandItem3.Execute();
            CommandItem4.Execute();

        }

        private void button2_Click(object sender, EventArgs e)
        {

            if (pMap.SelectionCount > 1) {
                MessageBox.Show("Please select only one feature (starting point)", "Selection error...");
                return;
            
            }
                       
            OperationLayer = comboBox2.Text;
            OperationAttribute = comboBox3.Text;
            Tolerance= Convert.ToDouble(textBox2.Text);
            NoOfSplit = Convert.ToInt16(textBox1.Text);


            IEnumLayer pEnumLayer = pMap.Layers;

            ILayer pLayer;


            for (int i = 0; i < pMap.LayerCount; i++){


                pLayer = pEnumLayer.Next();

                if (pLayer.Name.Equals(OperationLayer)){


                    IDataset pDataSet = (IDataset)(pLayer);

                    Geoprocessor GP = new Geoprocessor();
                    GP.OverwriteOutput = true;

                    AddField SplitGroup = new AddField();
                    SplitGroup.in_table = pDataSet.Workspace.PathName + System.IO.Path.DirectorySeparatorChar + pDataSet.Name + ".shp";
                    //........
                    workSpacePath = pDataSet.Workspace.PathName;

                    //........
                    SplitGroup.field_name = "SpltGropus";
                    SplitGroup.field_type = "SHORT";

                    GP.Execute(SplitGroup, null);

                    CalculateField CalculateField = new CalculateField();
                    CalculateField.in_table = pDataSet.Workspace.PathName + System.IO.Path.DirectorySeparatorChar + pDataSet.Name + ".shp";
                    CalculateField.field = "SpltGropus";
                    CalculateField.expression = "0";
                    GP.Execute(CalculateField, null);


                    AddField TempGroup = new AddField();
                    TempGroup.in_table = pDataSet.Workspace.PathName + System.IO.Path.DirectorySeparatorChar + pDataSet.Name + ".shp";
                    TempGroup.field_name = "Distance";
                    TempGroup.field_type = "DOUBLE";
                    GP.Execute(TempGroup, null);

                    AggregatePolygons ConcaveHull = new AggregatePolygons();
                    ConcaveHull.aggregation_distance = Tolerance;
                    ConcaveHull.in_features = pDataSet.Workspace.PathName + System.IO.Path.DirectorySeparatorChar + pDataSet.Name + ".shp";
                    ConcaveHull.out_feature_class = System.IO.Path.GetTempPath() + System.IO.Path.DirectorySeparatorChar + "Temp_ConcaveHull.shp";

                    GP.AddOutputsToMap = false;

                    GP.Execute(ConcaveHull, null);

                    //IFeatureLayer2 pFeatureLayer21 = (IFeatureLayer2)pLayer;
                    //IFeatureClass pFeatureClass1 = pFeatureLayer21.FeatureClass;
                    
                    //IField spltgrp = new FieldClass();
                    //IFieldEdit spltgrp1 = (IFieldEdit)spltgrp;
                    //spltgrp1.Name_2 = "SpltGropus";
                    //spltgrp1.Type_2 = esriFieldType.esriFieldTypeSmallInteger;
                    //spltgrp = spltgrp1;

                    //IField tmpgrp = new FieldClass();
                    //IFieldEdit tmpgrp1 = (IFieldEdit)tmpgrp;
                    //tmpgrp1.Name_2 = "TempGroups";
                    //tmpgrp1.Type_2 = esriFieldType.esriFieldTypeSmallInteger;
                    //tmpgrp = tmpgrp1;

                    //pFeatureClass1.AddField(spltgrp);
                    //pFeatureClass1.AddField(tmpgrp);
                   
                    IFeatureLayer pFeatureLayer = (IFeatureLayer)pLayer;
                    IFeatureSelection pFeatureSelection = (IFeatureSelection)pFeatureLayer;
                    ISelectionSet pSelectionSet = pFeatureSelection.SelectionSet;

                    ICursor pCursor;
                    pSelectionSet.Search(null, false, out pCursor);
                    IFeatureCursor pFeatureCursor = (IFeatureCursor)pCursor;

                    IFeature pFeature = pFeatureCursor.NextFeature();
                    
                    GroupIDTotal Group = new GroupIDTotal();
                    Group.setID(1);
                    Group.setCalculatedSplitValue(Convert.ToDouble(pFeature.get_Value(pFeature.Fields.FindField(OperationAttribute))));
                 
                    pFeature.set_Value(pFeature.Fields.FindField("SpltGropus"), 1);
                    pFeature.Store();

                    GroupIDTotalList.Add(Group);

                    //Group = new GroupIDTotal (1,(double)pFeature.get_Value(pFeature.Fields.FindField(OperationAttribute)));


                    IFeatureLayer2 pFeatureLayer2 = (IFeatureLayer2)pLayer;
                    IFeatureClass pFeatureClass = pFeatureLayer2.FeatureClass;
                    IQueryFilter pQueryFilter = new QueryFilter();
                    pQueryFilter.WhereClause =null;

                    IFeatureCursor pFeatureCursor1 = pFeatureClass.Search(pQueryFilter, false);
                    IFeature pFeature1;

                    for (int j = 0; j < pFeatureClass.FeatureCount(pQueryFilter); j++)
                    {

                        pFeature1 = pFeatureCursor1.NextFeature();
                        FieldTotal = FieldTotal + Convert.ToDouble(pFeature1.get_Value(pFeature1.Fields.FindField(OperationAttribute)));

                    }

                    EqualPartValue = FieldTotal / Convert.ToInt16 (textBox1.Text);

                    F2FDistance.MakeDistanceArray(pFeature, GroupIDTotalList[0].getID());

                    int k = 2;
                    while ((NoOfSplit - 1) != RegionFormed) {

                        
                        
                        IFeature NextStartingFeature = StaticMethodsClass.FindNextStartingFeature(RegionGeomtry);

                        //MessageBox.Show("..." + NextStartingFeature.OID, "hi");
                        GroupIDTotal NextGroup = new GroupIDTotal();
                        NextGroup.setID((short)k);
                        NextGroup.setCalculatedSplitValue(Convert.ToDouble(NextStartingFeature.get_Value(NextStartingFeature.Fields.FindField(OperationAttribute))));

                        NextStartingFeature.set_Value(NextStartingFeature.Fields.FindField("SpltGropus"), k);
                        NextStartingFeature.Store();

                        GroupIDTotalList.Add(NextGroup);

                        F2FDistance.MakeDistanceArray(NextStartingFeature, GroupIDTotalList[k-1].getID());

                        k = k + 1;  
                    }


                    //
                    //IGeometry pPol = CreateConvexHull.Create(pFeatureCursor1);
                    //IFeature dd = pFeatureClass.CreateFeature();
                    //dd.Shape = pPol;

                    //dd.Store();

                    //IActiveView activeView = pMap as IActiveView;
                    //activeView.PartialRefresh(esriViewDrawPhase.esriViewGraphics, null, null);
                    //MessageBox.Show("...", "hi");

                    //

                }

                
              
            }
           

            MessageBox.Show("..." + FieldTotal + "..." + EqualPartValue , "hi");
            //MessageBox.Show("..." + pFeature1.OID, "hi");



        }

        
    }
}
