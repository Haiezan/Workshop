using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using Rhino.Geometry;

namespace Workshop.ModelData
{
    class Model
    {
        public string Path;

        List<StdFlr> StdFlrs = new List<StdFlr>();

        List<Joint> Joints = new List<Joint>();
        List<Grid> Grids = new List<Grid>();

        List<Beam> Beams=new List<Beam>();
        List<BeamSect> BeamSects = new List<BeamSect>();

        List<ColSect> ColSects = new List<ColSect>();
        List<Column> Columns = new List<Column>();

        #region StdFlr
        /// <summary>
        /// 读取标准层信息
        /// </summary>
        public void ReadStdFlrData()
        {
            SQLiteConnection conn;
            SQLiteCommand cmd;
            SQLiteDataReader reader;

            conn = new SQLiteConnection(@"Data Source = " + Path);
            conn.Open();

            cmd = conn.CreateCommand();
            cmd.CommandText = @"SELECT ID,No_,Height FROM tblStdFlr";
            reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                StdFlr stdFlr = new StdFlr();

                stdFlr.ID = reader.GetInt64(0);
                stdFlr.No = reader.GetInt64(1);
                stdFlr.Height = reader.GetDouble(2);

                StdFlrs.Add(stdFlr);
            }
        }
        #endregion


        #region Joint
        /// <summary>
        /// 读取节点信息
        /// </summary>
        public void ReadJointData()
        {
            SQLiteConnection conn;
            SQLiteCommand cmd;
            SQLiteDataReader reader;

            conn = new SQLiteConnection(@"Data Source = " + Path);
            conn.Open();

            cmd = conn.CreateCommand();
            cmd.CommandText = @"SELECT ID,StdFlrID,X,Y,HDiff FROM tblJoint";
            reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                Joint joint = new Joint();
                joint.ID = reader.GetInt64(0);
                joint.StdFlrID = reader.GetInt64(1);
                joint.X = reader.GetDouble(2);
                joint.Y = reader.GetDouble(3);
                joint.HDiff = reader.GetDouble(4);

                Joints.Add(joint);

            }
        }
        public Joint GetJoint(long ID)
        {
            foreach (var joint in Joints)
            {
                if (ID == joint.ID) return joint;
            }
            return null;
        }
        public double GetX(long ID)
        {
            foreach(var joint in Joints)
            {
                if (joint.ID == ID) return joint.X;
            }
            return 0;
        }
        public double GetY(long ID)
        {
            foreach (var joint in Joints)
            {
                if (joint.ID == ID) return joint.Y;
            }
            return 0;
        }
        public double GetZ(long ID)
        {
            foreach (var joint in Joints)
            {
                if (joint.ID == ID)
                {
                    double fZ = 0;
                    foreach(var stdflr in StdFlrs)
                    {
                        if (stdflr.ID == joint.StdFlrID) return fZ + stdflr.Height;
                        else fZ += stdflr.Height;
                    }
                }
            }
            return 0;
        }
        #endregion

        #region Grid
        public void ReadGridtData()
        {
            SQLiteConnection conn;
            SQLiteCommand cmd;
            SQLiteDataReader reader;

            conn = new SQLiteConnection(@"Data Source = " + Path);
            conn.Open();

            cmd = conn.CreateCommand();
            cmd.CommandText = @"SELECT ID,StdFlrID,Jt1ID,Jt2ID,AxisID FROM tblGrid";
            reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                Grid grid = new Grid();

                grid.ID = reader.GetInt64(0);
                grid.StdFlrID = reader.GetInt64(1);
                grid.Jt1ID = reader.GetInt64(2);
                grid.Jt2ID = reader.GetInt64(3);

                grid.Jt1 = new Point3d(GetX(grid.Jt1ID), GetY(grid.Jt1ID), GetZ(grid.Jt1ID));
                grid.Jt2 = new Point3d(GetX(grid.Jt2ID), GetY(grid.Jt2ID), GetZ(grid.Jt2ID));

                Grids.Add(grid);
            }
        }
        public Grid GetGrid(long ID)
        {
            foreach(var grid in Grids)
            {
                if (ID == grid.ID) return grid;
            }
            return null;
        }
        public List<LineCurve> GetGridLines()
        {
            List<LineCurve> displayLines = new List<LineCurve>();

            foreach (var grid in Grids)
            {
                double fX1 = GetX(grid.Jt1ID);
                double fY1 = GetY(grid.Jt1ID);
                double fZ1 = GetZ(grid.Jt1ID);

                double fX2 = GetX(grid.Jt2ID);
                double fY2 = GetY(grid.Jt2ID);
                double fZ2 = GetZ(grid.Jt2ID);

                Point3d A = new Point3d(fX1, fY1, fZ1);
                Point3d B = new Point3d(fX2, fY2, fZ2);

                displayLines.Add(new LineCurve(A, B));
            }
            return displayLines;
        }
        #endregion
        #region Beam
        /// <summary>
        /// 读取梁构件信息
        /// </summary>
        public void ReadBeamdata()
        {
            SQLiteConnection conn;
            SQLiteCommand cmd;
            SQLiteDataReader reader;

            conn = new SQLiteConnection(@"Data Source = " + Path);
            conn.Open();

            cmd = conn.CreateCommand();
            cmd.CommandText = @"SELECT ID,StdFlrID,SectID,GridID FROM tblBeamSeg";
            reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                Beam beam=new Beam();
                beam.ID = reader.GetInt64(0);
                beam.StdFlrID = reader.GetInt64(1);
                beam.SectID = reader.GetInt64(2);
                beam.GridID = reader.GetInt64(3);

                beam.Grid = GetGrid(beam.GridID);
                beam.beamSect = GetBeamSect(beam.SectID);

                beam.GetSectPolyLineCurve();
                beam.GetBeamSurface();

                Beams.Add(beam);
            }
        }
        public void ReadBeamSect()
        {
            SQLiteConnection conn;
            SQLiteCommand cmd;
            SQLiteDataReader reader;

            conn = new SQLiteConnection(@"Data Source = " + Path);
            conn.Open();

            cmd = conn.CreateCommand();
            cmd.CommandText = @"SELECT ID,No_,Name,mat,Kind,ShapeVal FROM tblBeamSect";
            reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                BeamSect beamSect = new BeamSect();
                beamSect.ID = reader.GetInt64(0);
                beamSect.No = reader.GetInt32(1);
                beamSect.Name = reader.GetString(2);
                beamSect.Mat = reader.GetInt32(3);
                beamSect.Kind = reader.GetInt32(4);
                string shapeVal = reader.GetString(5);
                string[] str = shapeVal.Split(',');
                switch(beamSect.Kind)
                {
                    case 1:
                        beamSect.B = double.Parse(str[1]);
                        beamSect.H = double.Parse(str[2]);
                        break;
                    case 2:
                        beamSect.B = double.Parse(str[1]);
                        beamSect.H = double.Parse(str[2]);
                        beamSect.U = double.Parse(str[3]);
                        beamSect.T = double.Parse(str[4]);
                        beamSect.D = double.Parse(str[5]);
                        beamSect.F = double.Parse(str[6]);
                        break;
                    default:
                        break;    
                }
                BeamSects.Add(beamSect);
            } 
            
        }
        public BeamSect GetBeamSect(long ID)
        {
            foreach (var beamSect in BeamSects)
            {
                if (ID == beamSect.ID) return beamSect;
            }
            return null;
        }
        #endregion

        #region Column
        /// <summary>
        /// 读取柱构件信息
        /// </summary>
        public void ReadColData()
        {
            SQLiteConnection conn;
            SQLiteCommand cmd;
            SQLiteDataReader reader;

            conn = new SQLiteConnection(@"Data Source = " + Path);
            conn.Open();

            cmd = conn.CreateCommand();
            cmd.CommandText = @"SELECT ID,StdFlrID,SectID,JtID FROM tblColSeg";
            reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                Column column = new Column();
                column.ID = reader.GetInt64(0);
                column.StdFlrID = reader.GetInt64(1);
                column.SectID = reader.GetInt64(2);
                column.JtID = reader.GetInt64(3);

                column.Jt = GetJoint(column.JtID);
                column.colSect = GetColSect(column.SectID);
                column.Grid = GetColGrid(column.StdFlrID,column.Jt);

                column.GetSectPolyLineCurve();
                column.GetColumnSurface();

                Columns.Add(column);
            }
        }
        public void ReadColSect()
        {
            SQLiteConnection conn;
            SQLiteCommand cmd;
            SQLiteDataReader reader;

            conn = new SQLiteConnection(@"Data Source = " + Path);
            conn.Open();

            cmd = conn.CreateCommand();
            cmd.CommandText = @"SELECT ID,No_,Name,mat,Kind,ShapeVal FROM tblColSect";
            reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                ColSect colSect = new ColSect();
                colSect.ID = reader.GetInt64(0);
                colSect.No = reader.GetInt32(1);
                colSect.Name = reader.GetString(2);
                colSect.Mat = reader.GetInt32(3);
                colSect.Kind = reader.GetInt32(4);
                string shapeVal = reader.GetString(5);
                string[] str = shapeVal.Split(',');
                switch (colSect.Kind)
                {
                    case 1:
                        colSect.B = double.Parse(str[1]);
                        colSect.H = double.Parse(str[2]);
                        break;
                    case 2:
                        colSect.B = double.Parse(str[1]);
                        colSect.H = double.Parse(str[2]);
                        colSect.U = double.Parse(str[3]);
                        colSect.T = double.Parse(str[4]);
                        colSect.D = double.Parse(str[5]);
                        colSect.F = double.Parse(str[6]);
                        break;
                    default:
                        break;
                }
                ColSects.Add(colSect);
            }
        }
        public ColSect GetColSect(long ID)
        {
            foreach (var colSect in ColSects)
            {
                if (ID == colSect.ID) return colSect;
            }
            return null;
        }
        public Grid GetColGrid(long StdFlrID,Joint Jt)
        {
            Grid grid=new Grid();

            double fZ2 = 0, fZ1 = 0;
            foreach (var stdflr in StdFlrs)
            {
                if (stdflr.ID == StdFlrID)
                {
                    fZ1 = fZ2;
                    fZ2 += stdflr.Height;
                    break;
                }
                
                else
                {
                    fZ2 += stdflr.Height;
                }
                    
            }

            grid.Jt1 = new Point3d(Jt.X, Jt.Y, fZ1);
            grid.Jt2 = new Point3d(Jt.X, Jt.Y, fZ2);
            return grid;
        }

        #endregion

        public List<Surface>  GetBeamModel()
        {
            List < Surface > DisplaySurfaces= new List<Surface>();

            foreach(var beam in Beams)
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
    }
}
