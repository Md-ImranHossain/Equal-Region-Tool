using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.ArcMapUI;

namespace Equal_Region
{
    public class EqualRegion : ESRI.ArcGIS.Desktop.AddIns.Button
    {


        public EqualRegion()
        {
        }




        protected override void OnClick()
        {
            //
            //  TODO: Sample code showing how to access button host
            //
            Form1 form1 = new Form1();
            form1.Show();
         
          

        }
        protected override void OnUpdate()
        {
            Enabled = ArcMap.Application != null;
        }
    }

}
