using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rhino.Geometry;

namespace Workshop.ModelData
{
    class Wall
    {
        public long ID;  //梁号
        public long StdFlrID;    //标准层号
        public long SectID;  //截面编号
        public long GridID;  //网格编号

        public WallSect wallSect;
        public Grid grid;
        public Grid Grid
        {
            set
            {
                grid = value;
                vector = new Vector3d(grid.Jt2.Point.X - grid.Jt1.Point.X, grid.Jt2.Point.Y - grid.Jt1.Point.Y, grid.Jt2.Point.Z - grid.Jt1.Point.Z);
                normal = Vector3d.Divide(vector, vector.Length);
            }
        }
        public Vector3d ExtrudeDirection;

        public Vector3d vector = new Vector3d(0, 0, 1); //轴向向量
        public Vector3d normal = new Vector3d(0, 0, 1); //轴向向量归一化

        public Surface surface;
        public void GetSectPolyLineCurve()
        {
            if (wallSect == null || grid == null) return;

            //获取截面坐标
            List<Point3d> point3ds0 = new List<Point3d>();  //局部坐标


            double length = grid.Jt1.Point.DistanceTo(grid.Jt2.Point);

            point3ds0.Add(new Point3d(0, -0.5 * wallSect.B, 0));
            point3ds0.Add(new Point3d(length, -0.5 * wallSect.B, 0));
            point3ds0.Add(new Point3d(length, 0.5 * wallSect.B, 0));
            point3ds0.Add(new Point3d(0, 0.5 * wallSect.B, 0));
            point3ds0.Add(new Point3d(0, -0.5 * wallSect.B, 0));

            //获取转换向量
            //Vector3d vectorZZ = Vector3d.Subtract(new Vector3d(grid.Jt2), new Vector3d(grid.Jt1));
            Vector3d vectorX1 = normal;
            Vector3d vectorZ1 = new Vector3d(0, 0, 1);
            Vector3d vectorY1 = Vector3d.CrossProduct(vectorX1, vectorZ1);
            //转置
            Vector3d vectorX = new Vector3d(vectorX1.X, vectorY1.X, vectorZ1.X);
            Vector3d vectorY = new Vector3d(vectorX1.Y, vectorY1.Y, vectorZ1.Y);
            Vector3d vectorZ = new Vector3d(vectorX1.Z, vectorY1.Z, vectorZ1.Z);

            //坐标转换
            List<Point3d> point3ds = new List<Point3d>();   //整体坐标
            foreach (var point0 in point3ds0)
            {
                Point3d point = new Point3d();
                point.X = Vector3d.Multiply(new Vector3d(point0), vectorX);
                point.Y = Vector3d.Multiply(new Vector3d(point0), vectorY);
                point.Z = Vector3d.Multiply(new Vector3d(point0), vectorZ);
                point3ds.Add(Point3d.Add(point, grid.Jt1.Point));
                //point3ds.Add(point);
            }

            wallSect.curve = new PolylineCurve(point3ds);
        }
        public void GetWallSurface()
        {
            surface = Surface.CreateExtrusion(wallSect.curve, ExtrudeDirection);
        }
    }
}
