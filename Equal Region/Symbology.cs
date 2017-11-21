using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.esriSystem;

namespace Equal_Region
{
    public class Symbology
    {
        public static void DefineUniqueValueRenderer(IGeoFeatureLayer pGeoFeatureLayer, string fieldName)
        {

            IRandomColorRamp pRandomColorRamp = new RandomColorRampClass();
            //Create the color ramp for the symbols in the renderer.
            pRandomColorRamp.MinSaturation = 20;
            pRandomColorRamp.MaxSaturation = 40;
            pRandomColorRamp.MinValue = 85;
            pRandomColorRamp.MaxValue = 100;
            pRandomColorRamp.StartHue = 76;
            pRandomColorRamp.EndHue = 188;
            pRandomColorRamp.UseSeed = true;
            pRandomColorRamp.Seed = 43;
            
            //Create the renderer.
            IUniqueValueRenderer pUniqueValueRenderer = new UniqueValueRendererClass();

            ISimpleFillSymbol pSimpleFillSymbol = new SimpleFillSymbolClass();
            pSimpleFillSymbol.Style = esriSimpleFillStyle.esriSFSSolid;
            pSimpleFillSymbol.Outline.Width = 0.4;

            //These properties should be set prior to adding values.
            pUniqueValueRenderer.FieldCount = 1;
            pUniqueValueRenderer.set_Field(0, fieldName);
            pUniqueValueRenderer.DefaultSymbol = pSimpleFillSymbol as ISymbol;
            pUniqueValueRenderer.UseDefaultSymbol = true;

            IDisplayTable pDisplayTable = pGeoFeatureLayer as IDisplayTable;
            IFeatureCursor pFeatureCursor = pDisplayTable.SearchDisplayTable(null, false) as IFeatureCursor;
            IFeature pFeature = pFeatureCursor.NextFeature();


            bool ValFound;
            int fieldIndex;

            IFields pFields = pFeatureCursor.Fields;
            fieldIndex = pFields.FindField(fieldName);
            while (pFeature != null)
            {
                ISimpleFillSymbol pClassSymbol = new SimpleFillSymbolClass();
                pClassSymbol.Style = esriSimpleFillStyle.esriSFSSolid;
                pClassSymbol.Outline.Width = 0.4;

                string classValue;
                classValue = pFeature.get_Value(fieldIndex) as string;

                //Test to see if this value was added to the renderer. If not, add it.
                ValFound = false;
                for (int i = 0; i <= pUniqueValueRenderer.ValueCount - 1; i++)
                {
                    if (pUniqueValueRenderer.get_Value(i) == classValue)
                    {
                        ValFound = true;
                        break; //Exit the loop if the value was found.
                    }
                }
                //If the value was not found, it's new and will be added.
                if (ValFound == false)
                {
                    pUniqueValueRenderer.AddValue(classValue, fieldName, pClassSymbol as
                        ISymbol);
                    pUniqueValueRenderer.set_Label(classValue, classValue);
                    pUniqueValueRenderer.set_Symbol(classValue, pClassSymbol as ISymbol);
                }
                pFeature = pFeatureCursor.NextFeature();
            }
            //Since the number of unique values is known, the color ramp can be sized and the colors assigned.
            pRandomColorRamp.Size = pUniqueValueRenderer.ValueCount;
            bool bOK;
            pRandomColorRamp.CreateRamp(out bOK);

            IEnumColors pEnumColors = pRandomColorRamp.Colors;
            pEnumColors.Reset();
            for (int j = 0; j <= pUniqueValueRenderer.ValueCount - 1; j++)
            {
                string xv;
                xv = pUniqueValueRenderer.get_Value(j);
                if (xv != "")
                {
                    ISimpleFillSymbol pSimpleFillColor = pUniqueValueRenderer.get_Symbol(xv)
                        as ISimpleFillSymbol;
                    pSimpleFillColor.Color = pEnumColors.Next();
                    pUniqueValueRenderer.set_Symbol(xv, pSimpleFillColor as ISymbol);

                }
            }

            //'** If you didn't use a predefined color ramp in a style, use "Custom" here. 
            //'** Otherwise, use the name of the color ramp you selected.

            pUniqueValueRenderer.ColorScheme = pRandomColorRamp.Name;//"Custom";
            
            ITable pTable = pDisplayTable as ITable;
            bool isString = pTable.Fields.get_Field(fieldIndex).Type ==
                esriFieldType.esriFieldTypeString;
            pUniqueValueRenderer.set_FieldType(0, isString);
            pGeoFeatureLayer.Renderer = pUniqueValueRenderer as IFeatureRenderer;

            //This makes the layer properties symbology tab show the correct interface.
            IUID pUID = new UIDClass();
            pUID.Value = "{683C994E-A17B-11D1-8816-080009EC732A}";
            pGeoFeatureLayer.RendererPropertyPageClassID = pUID as UIDClass;

        }
    }
}
