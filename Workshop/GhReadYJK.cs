using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

using System.Data.SQLite;
using Workshop.ModelData;

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
            pManager.AddCurveParameter("DisplayGrids", "DisplayGrids", "DisplayGrids", GH_ParamAccess.list);
            pManager.AddSurfaceParameter("DisplayBeams", "DisplayBeams", "DisplayBeams", GH_ParamAccess.list);
            pManager.AddSurfaceParameter("DisplayColumns", "DisplayColumns", "DisplayColumns", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            
            string sPath = @"D:\dtlmodel.ydb";

            Model model = new Model();
            model.Path = sPath;
            model.ReadStdFlrData();
            model.ReadJointData();
            model.ReadGridtData();

            model.ReadBeamSect();
            model.ReadBeamdata();

            model.ReadColSect();
            model.ReadColData();
            

            List<LineCurve> displayGrids = model.GetGridLines();
            List<Surface> displayBeams = model.GetBeamModel();
            List<Surface> displayColumns = model.GetColumnModel();

            DA.SetDataList("DisplayGrids", displayGrids);
            DA.SetDataList("DisplayBeams", displayBeams);
            DA.SetDataList("DisplayColumns", displayColumns);

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