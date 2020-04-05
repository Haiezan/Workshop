using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace Workshop
{
    public class GhcCreatePyramid : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the GhcCreatePyramid class.
        /// </summary>
        public GhcCreatePyramid()
          : base(
                "Create Pyramid", 
                "Pyramid",
              "Description",
              "Workshop", "Subcategory")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddPlaneParameter("Base Plane", "Base Plane", "Base Plane", GH_ParamAccess.item);
            pManager.AddNumberParameter("Length", "Length", "Length", GH_ParamAccess.item);
            pManager.AddNumberParameter("Width", "Width", "Width", GH_ParamAccess.item);
            pManager.AddNumberParameter("Height", "Height", "Height", GH_ParamAccess.item);
            pManager.AddNumberParameter("Number", "Number", "Number", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddCurveParameter("Display Lines", "Display Lines", "Display Lines", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Plane iBasePlane = Plane.WorldXY;
            double iLength = 1.0;
            double iWidth = 1.0;
            double iHeight = 1.0;
            double iNo = 1;

            DA.GetData(0, ref iBasePlane);
            DA.GetData(1, ref iLength);
            DA.GetData(2, ref iWidth);
            DA.GetData(3, ref iHeight);
            DA.GetData(4, ref iNo);

            Pyramid myPyramid = new Pyramid(iBasePlane, iLength, iWidth, iHeight);
            List<LineCurve> displayLines = myPyramid.ComputeDisplayLines();

            DA.SetDataList("Display Lines", displayLines);
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
            get { return new Guid("3660fbad-aa43-4a2c-a2ba-79ea694c7f67"); }
        }
    }
}