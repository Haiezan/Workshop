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

        //设置标高
        public void SetLevel(double Levelnew)
        {
            foreach(var joint in Joints)
            {
                joint.Point.Z = Levelnew;
            }
            foreach(var beam in Beams)
            {
                beam.GetSectPolyLineCurve();
                beam.GetBeamSurface();
            }
            foreach(var column in Columns)
            {
                column.GetSectPolyLineCurve();
                column.GetColumnSurface();
            }
            foreach(var wall in Walls)
            {
                wall.GetSectPolyLineCurve();
                wall.GetWallSurface();
            }
        }
    }
}
