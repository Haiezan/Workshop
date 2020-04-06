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
                "Read YJK ydb Model",
                "BeamNotes", 
                "Building")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("YDBfile", "YDBfile", "YDBfile", GH_ParamAccess.item);
            pManager.AddNumberParameter("Story", "Story", "Story Number", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            //pManager.AddCurveParameter("DisplayGrids", "DisplayGrids", "DisplayGrids", GH_ParamAccess.list);
            //pManager.AddSurfaceParameter("DisplayBeams", "DisplayBeams", "DisplayBeams", GH_ParamAccess.list);
            //pManager.AddSurfaceParameter("DisplayColumns", "DisplayColumns", "DisplayColumns", GH_ParamAccess.list);
            pManager.AddSurfaceParameter("DisplayBeam", "Beam", "Display Beam Component", GH_ParamAccess.list);
            pManager.AddSurfaceParameter("DisplayColumn", "Column", "Display Column Component", GH_ParamAccess.list);
            pManager.AddSurfaceParameter("DisplayWall", "Wall", "Display Wall Component", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            double iStory=0;
            string sAdd = null;
            DA.GetData(0, ref sAdd);
            DA.GetData(1, ref iStory);
            
            //DA.GetData(1, ref Levelnew);

            //string sPath = @"D:\dtlmodel.ydb";
            string sPath = sAdd;

            Model model = new Model();
            model.Path = sPath;


            model.ReadStdFlrData();
            model.ReadFloorData();

            model.ReadJointData();
            model.ReadGridtData();

            model.ReadBeamSect();
            model.ReadBeamdata();

            model.ReadColSect();
            model.ReadColData();

            model.ReadWallSect();
            model.ReadWallData();

            model.GetModel();

            List<Surface> displayBeam = new List<Surface>();
            List<Surface> displayColumn= new List<Surface>();
            List<Surface> displayWall = new List<Surface>();

            if(iStory < 0.001)
            {
                foreach (var storyMode in model.StdStoryModels)
                {
                    foreach (var floorSurface in storyMode.FloorSurfaces)
                    {
                        displayBeam.AddRange(floorSurface.BeamSurface);
                        displayColumn.AddRange(floorSurface.ColumnSurface);
                        displayWall.AddRange(floorSurface.WallSurface);
                    }
                }
            }
            else
            {
                FloorSurface floorSurface = model.GetFloorSurface(Convert.ToInt32(iStory));
                displayBeam.AddRange(floorSurface.BeamSurface);
                displayColumn.AddRange(floorSurface.ColumnSurface);
                displayWall.AddRange(floorSurface.WallSurface);
            }

            DA.SetDataList("DisplayBeam", displayBeam);
            DA.SetDataList("DisplayColumn", displayColumn);
            DA.SetDataList("DisplayWall", displayWall);
            //DA.SetDataList("Model", display);

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
                return Properties.Resources.readYJK;
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