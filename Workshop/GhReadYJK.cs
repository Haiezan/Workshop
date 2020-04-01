using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

using System.Data.SQLite;

namespace Workshop
{
    public class GhReadYJK : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the GhReadYJK class.
        /// </summary>
        public GhReadYJK()
          : base(
                "ReadYJK", 
                "YJK",
                "Description",
                "Workshop", 
                "Subcategory")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddNumberParameter("X1", "X", "X", GH_ParamAccess.list);
            pManager.AddNumberParameter("Y1", "Y", "Y", GH_ParamAccess.list);
            pManager.AddNumberParameter("X2", "X", "X", GH_ParamAccess.list);
            pManager.AddNumberParameter("Y2", "Y", "Y", GH_ParamAccess.list);
            pManager.AddCurveParameter("Display Points", "Display Points", "Display Points", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            List<double> x1Container = new List<double>();
            List<double> y1Container = new List<double>();
            List<double> x2Container = new List<double>();
            List<double> y2Container = new List<double>();
            SQLiteConnection conn;
            SQLiteCommand cmd;
            SQLiteDataReader reader;

            conn = new SQLiteConnection(@"Data Source = D:\dtlmodel.ydb");
            conn.Open();

            cmd = conn.CreateCommand();
            cmd.CommandText = @"SELECT joint1.X, joint1.Y, joint2.X, joint2.Y FROM tblGrid "
             + @"INNER JOIN tblJoint AS joint1 ON tblGrid.Jt1ID = joint1.ID "
             + @"INNER JOIN tblJoint AS joint2 ON tblGrid.Jt2ID = joint2.ID ";
            reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                x1Container.Add(reader.GetDouble(0));
                y1Container.Add(reader.GetDouble(1));
                x2Container.Add(reader.GetDouble(2));
                y2Container.Add(reader.GetDouble(3));
            }

            DA.SetDataList("X1", x1Container);
            DA.SetDataList("Y1", y1Container);
            DA.SetDataList("X2", x2Container);
            DA.SetDataList("Y2", y2Container);


            //读取梁信息


        }

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                //You can add image files to your project resources and access them like this:
                // return Resources.IconForThisComponent;
                return null;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("cb013a51-6920-427f-bbdb-398dc0e97be5"); }
        }
    }
}