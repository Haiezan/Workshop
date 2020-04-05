using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rhino.Geometry;

namespace Workshop.ModelData
{
    class Beam
    {
        public long ID;  //梁号
        public long StdFlrID;    //标准层号
        public long SectID;  //截面编号
        public long GridID;  //网格编号

        public BeamSect beamSect;

        public Grid grid;
        public Vector3d vector; //轴向向量
        public Vector3d normal; //轴向向量归一化
        public Grid Grid
        {
            set
            {
                grid = value;
                vector = new Vector3d(grid.Jt2.Point.X - grid.Jt1.Point.X, grid.Jt2.Point.Y - grid.Jt1.Point.Y, grid.Jt2.Point.Z - grid.Jt1.Point.Z);
                normal = Vector3d.Divide(vector, vector.Length);
            }
        }

        public Surface surface;
        /// <summary>
        /// 计算截面形状
        /// </summary>
        /// <returns></returns>
        public void GetSectPolyLineCurve()
        {
            if (beamSect == null || grid == null) return;

            //获取截面坐标
            List<Point3d> point3ds0 = new List<Point3d>();  //局部坐标
            switch (beamSect.Kind)
            {
                case 1:
                    point3ds0.Add(new Point3d(-0.5 * beamSect.B, -0.5 * beamSect.H, 0));
                    point3ds0.Add(new Point3d(-0.5 * beamSect.B,  0.5 * beamSect.H, 0));
                    point3ds0.Add(new Point3d( 0.5 * beamSect.B,  0.5 * beamSect.H, 0));
                    point3ds0.Add(new Point3d( 0.5 * beamSect.B, -0.5 * beamSect.H, 0));
                    point3ds0.Add(new Point3d(-0.5 * beamSect.B, -0.5 * beamSect.H, 0));
                    break;
                case 2:
                    point3ds0.Add(new Point3d(-0.5 * beamSect.U, -0.5 * beamSect.H, 0));
                    point3ds0.Add(new Point3d(-0.5 * beamSect.U,  0.5 * beamSect.H, 0));
                    point3ds0.Add(new Point3d( 0.5 * beamSect.U,  0.5 * beamSect.H, 0));
                    point3ds0.Add(new Point3d( 0.5 * beamSect.U, -0.5 * beamSect.H, 0));
                    point3ds0.Add(new Point3d(-0.5 * beamSect.U, -0.5 * beamSect.H, 0));
                    break;
                case 13:
                    point3ds0.Add(new Point3d(-0.5 * beamSect.B, -0.5 * beamSect.H, 0));
                    point3ds0.Add(new Point3d(-0.5 * beamSect.B, 0.5 * beamSect.H, 0));
                    point3ds0.Add(new Point3d(0.5 * beamSect.B, 0.5 * beamSect.H, 0));
                    point3ds0.Add(new Point3d(0.5 * beamSect.B, -0.5 * beamSect.H, 0));
                    point3ds0.Add(new Point3d(-0.5 * beamSect.B, -0.5 * beamSect.H, 0));
                    break;
                default:
                    point3ds0.Add(new Point3d(-0.5 * beamSect.B, -0.5 * beamSect.H, 0));
                    point3ds0.Add(new Point3d(-0.5 * beamSect.B, 0.5 * beamSect.H, 0));
                    point3ds0.Add(new Point3d(0.5 * beamSect.B, 0.5 * beamSect.H, 0));
                    point3ds0.Add(new Point3d(0.5 * beamSect.B, -0.5 * beamSect.H, 0));
                    point3ds0.Add(new Point3d(-0.5 * beamSect.B, -0.5 * beamSect.H, 0));
                    break;
            }

            //获取转换向量
            //Vector3d vectorZZ = Vector3d.Subtract(new Vector3d(grid.Jt2), new Vector3d(grid.Jt1));
            Vector3d vectorZ1 = normal;
            Vector3d vectorY1 = new Vector3d(0, 0, 1);
            Vector3d vectorX1 = Vector3d.CrossProduct(vectorY1, vectorZ1);
            //转置
            Vector3d vectorX = new Vector3d(vectorX1.X, vectorY1.X, vectorZ1.X);
            Vector3d vectorY = new Vector3d(vectorX1.Y, vectorY1.Y, vectorZ1.Y);
            Vector3d vectorZ = new Vector3d(vectorX1.Z, vectorY1.Z, vectorZ1.Z);

            //坐标转换
            List<Point3d> point3ds = new List<Point3d>();   //整体坐标
            foreach(var point0 in point3ds0)
            {
                Point3d point = new Point3d();
                point.X = Vector3d.Multiply(new Vector3d(point0), vectorX);
                point.Y = Vector3d.Multiply(new Vector3d(point0), vectorY);
                point.Z = Vector3d.Multiply(new Vector3d(point0), vectorZ);
                point3ds.Add(Point3d.Add(point, grid.Jt1.Point));
                //point3ds.Add(point);
            }

            //延伸得到面
            beamSect.curve = new PolylineCurve(point3ds);
        }
        public void GetBeamSurface()
        {
            surface=Surface.CreateExtrusion(beamSect.curve, vector);
        }
    }
}
