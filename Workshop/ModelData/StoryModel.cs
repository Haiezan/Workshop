using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rhino.Geometry;

namespace Workshop.ModelData
{
    class StoryModel
    {
        public long StdFlrID;  //标准层号
        public long FloorID;   //楼层号 为-1说明是标准层
        public long No;
        public double Height;  //层高
        public double Level;   //楼面标高

        public List<Joint> Joints = new List<Joint>();
        public List<Grid> Grids = new List<Grid>();

        public List<Beam> Beams = new List<Beam>();
        public List<Column> Columns = new List<Column>();
        public List<Wall> Walls = new List<Wall>();

        public List<FloorSurface> FloorSurfaces = new List<FloorSurface>();
        public Joint GetJoint(long ID)
        {
            foreach(var joint in Joints)
            {
                if (joint.ID == ID) return joint;
            }
            return null;
        }
        public Grid GetGrid(long ID)
        {
            foreach (var grid in Grids)
            {
                if (grid.ID == ID) return grid;
            }
            return null;
        }
        public List<LineCurve> GetAllGridLines()
        {
            List<LineCurve> linelist = new List<LineCurve>();
            foreach (var grid in Grids)
            {
                Point3d A = grid.Jt1.Point;
                Point3d B = grid.Jt2.Point;
                linelist.Add(new LineCurve(A, B));
            }
            return linelist;
        }

        ///输出模型
        public List<Surface> GetStoryModel()
        {
            List<Surface> DisplaySurfaces = new List<Surface>();

            DisplaySurfaces.AddRange(GetBeamModel());
            DisplaySurfaces.AddRange(GetColumnModel());
            DisplaySurfaces.AddRange(GetWallModel());

            return DisplaySurfaces;
        }
        public List<Surface> GetBeamModel()
        {
            List<Surface> DisplaySurfaces = new List<Surface>();

            foreach (var beam in Beams)
            {
                DisplaySurfaces.Add(beam.surface);
            }

            return DisplaySurfaces;
        }
        public List<Surface> GetColumnModel()
        {
            List<Surface> DisplaySurfaces = new List<Surface>();

            foreach (var column in Columns)
            {
                DisplaySurfaces.Add(column.surface);
            }

            return DisplaySurfaces;
        }
        public List<Surface> GetWallModel()
        {
            List<Surface> DisplaySurfaces = new List<Surface>();

            foreach (var wall in Walls)
            {
                DisplaySurfaces.Add(wall.surface);
            }

            return DisplaySurfaces;
        }

        //设置楼层标高及层高
        public void SetLevel(double newLevel, double newHeight = 0)
        {
            if(newHeight<1E-3)  newHeight = Height;

            foreach(var joint in Joints)
            {
                joint.Point.Z = newLevel;
            }
            foreach (var beam in Beams)
            {
                
                beam.GetBeamSurface();
            }
            foreach (var column in Columns)
            {
                
                column.ExtrudeDirection = new Vector3d(0, 0, -1 * newHeight);
                column.GetColumnSurface();
            }
            foreach (var wall in Walls)
            {
                
                wall.ExtrudeDirection = new Vector3d(0, 0, -1 * newHeight);
                wall.GetWallSurface();
            }
        }
        //获取各普通层Surface
        public List<Surface> GetBeamSurfaces()
        {
            List<Surface> DisplaySurface = new List<Surface>();
            foreach (var beam in Beams)
            {
                beam.GetSectPolyLineCurve();
                DisplaySurface.Add(beam.GetBeamSurface());
            }
            return DisplaySurface;
        }
        public List<Surface> GetColumnSurfaces()
        {
            List<Surface> DisplaySurface = new List<Surface>();
            foreach (var column in Columns)
            {
                column.GetSectPolyLineCurve();
                DisplaySurface.Add(column.GetColumnSurface());
            }
            return DisplaySurface;
        }
        public List<Surface> GetWallSurfaces()
        {
            List<Surface> DisplaySurface = new List<Surface>();
            foreach (var wall in Walls)
            {
                wall.GetSectPolyLineCurve();
                DisplaySurface.Add(wall.GetWallSurface());
            }
            return DisplaySurface;
        }
    }
}
