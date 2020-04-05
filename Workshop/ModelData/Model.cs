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
        List<Floor> Floors = new List<Floor>();

        public List<StoryModel> StdStoryModels = new List<StoryModel>();
        public List<StoryModel> StoryModels = new List<StoryModel>();


        //List<Joint> Joints = new List<Joint>();
        //List<Grid> Grids = new List<Grid>();

        //List<Beam> Beams=new List<Beam>();
        List<BeamSect> BeamSects = new List<BeamSect>();

        List<ColSect> ColSects = new List<ColSect>();
        //List<Column> Columns = new List<Column>();

        List<WallSect> WallSects = new List<WallSect>();
        //List<Wall> Walls = new List<Wall>();

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

                StoryModel storyModel = new StoryModel();
                storyModel.StdFlrID = stdFlr.ID;
                storyModel.FloorID = -1;
                storyModel.No = stdFlr.No;
                storyModel.Height = stdFlr.Height;
                storyModel.Level = 0;
                StdStoryModels.Add(storyModel);
            }
        }
        public StoryModel GetStdStoryModel(long StdFlr)
        {
            foreach(var storyModel in StdStoryModels)
            {
                if (storyModel.StdFlrID == StdFlr) return storyModel;
            }
            return null;
        }
        public void ReadFloorData()
        {
            SQLiteConnection conn;
            SQLiteCommand cmd;
            SQLiteDataReader reader;

            conn = new SQLiteConnection(@"Data Source = " + Path);
            conn.Open();

            cmd = conn.CreateCommand();
            cmd.CommandText = @"SELECT ID,No_,Name,StdFlrID,LevelB,Height FROM tblFloor";
            reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                Floor floor = new Floor();
                floor.ID = reader.GetInt64(0);
                floor.No = reader.GetInt64(1);
                floor.Name = reader.GetString(2);
                floor.StdFlrID = reader.GetInt64(3);
                floor.LevelB = reader.GetDouble(4);
                floor.Height = reader.GetDouble(5);
                Floors.Add(floor);

                StoryModel storyModel = new StoryModel();
                storyModel.StdFlrID = floor.StdFlrID;
                storyModel.FloorID = floor.ID;
                storyModel.No = floor.No;
                storyModel.Height = floor.Height;
                storyModel.Level = floor.LevelB;
                StoryModels.Add(storyModel);
            }
        }
        public StoryModel GetStoryModel(long FlrID)
        {
            foreach (var storyModel in StoryModels)
            {
                if (storyModel.FloorID == FlrID) return storyModel;
            }
            return null;
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

                StoryModel storyModel = GetStdStoryModel(joint.StdFlrID);
                double fZ = storyModel.Height;
                double fX= reader.GetDouble(2);
                double fY= reader.GetDouble(3);
                joint.Point = new Point3d(fX, fY, fZ);

                joint.HDiff = reader.GetDouble(4);

                storyModel.Joints.Add(joint);
            }
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

                StoryModel storyModel = GetStdStoryModel(grid.StdFlrID);
                grid.Jt1 = storyModel.GetJoint(grid.Jt1ID);
                grid.Jt2 = storyModel.GetJoint(grid.Jt2ID);

                storyModel.Grids.Add(grid);
            }
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

                StoryModel storyModel = GetStdStoryModel(beam.StdFlrID);
                beam.Grid = storyModel.GetGrid(beam.GridID);
                beam.beamSect = GetBeamSect(beam.SectID);

                beam.GetSectPolyLineCurve();
                beam.GetBeamSurface();

                storyModel.Beams.Add(beam);
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
                        beamSect.B = double.Parse(str[1]);
                        beamSect.H = double.Parse(str[2]);
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

                StoryModel storyModel = GetStdStoryModel(column.StdFlrID);
                column.Jt = storyModel.GetJoint(column.JtID);
                column.colSect = GetColSect(column.SectID);
                //column.Grid.Jt1 = column.Jt.Point;
                //column.Grid.Jt2 = new Point3d(column.Jt.Point.X, column.Jt.Point.Y, 0);
                column.ExtrudeDirection = new Vector3d(0, 0, -1 * storyModel.Height);

                column.GetSectPolyLineCurve();
                column.GetColumnSurface();

                storyModel.Columns.Add(column);
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
                    case 15:
                        colSect.B = double.Parse(str[1]);
                        colSect.H = double.Parse(str[2]);
                        colSect.U = double.Parse(str[3]);
                        colSect.T = double.Parse(str[4]);
                        colSect.D = double.Parse(str[5]);
                        colSect.F = double.Parse(str[6]);
                        break;
                    case 101:
                        colSect.B = double.Parse(str[1]);
                        colSect.H = double.Parse(str[2]);
                        colSect.U = double.Parse(str[3]);
                        colSect.T = double.Parse(str[4]);
                        colSect.D = double.Parse(str[5]);
                        colSect.F = double.Parse(str[6]);
                        colSect.U1 = double.Parse(str[7]);
                        colSect.T1 = double.Parse(str[8]);
                        colSect.D1 = double.Parse(str[9]);
                        colSect.F1 = double.Parse(str[10]);
                        break;
                    default:
                        colSect.B = double.Parse(str[1]);
                        colSect.H = double.Parse(str[2]);
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
        #endregion

        #region Wall
        public void ReadWallData()
        {
            SQLiteConnection conn;
            SQLiteCommand cmd;
            SQLiteDataReader reader;

            conn = new SQLiteConnection(@"Data Source = " + Path);
            conn.Open();

            cmd = conn.CreateCommand();
            cmd.CommandText = @"SELECT ID,StdFlrID,SectID,GridID FROM tblWallSeg";
            reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                Wall wall = new Wall();
                wall.ID = reader.GetInt64(0);
                wall.StdFlrID = reader.GetInt64(1);
                wall.SectID = reader.GetInt64(2);
                wall.GridID = reader.GetInt64(3);

                StoryModel storyModel = GetStdStoryModel(wall.StdFlrID);

                wall.Grid = storyModel.GetGrid(wall.GridID);
                wall.wallSect = GetWallSect(wall.SectID);
                wall.ExtrudeDirection = new Vector3d(0, 0, -1 * storyModel.Height);

                wall.GetSectPolyLineCurve();
                wall.GetWallSurface();

                storyModel.Walls.Add(wall);
            }
        }
        public void ReadWallSect()
        {
            SQLiteConnection conn;
            SQLiteCommand cmd;
            SQLiteDataReader reader;

            conn = new SQLiteConnection(@"Data Source = " + Path);
            conn.Open();

            cmd = conn.CreateCommand();
            cmd.CommandText = @"SELECT ID,No_,mat,Kind,B,H,T2 FROM tblWallSect";
            reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                WallSect wallSect = new WallSect();
                wallSect.ID = reader.GetInt64(0);
                wallSect.No = reader.GetInt32(1);
                wallSect.Mat = reader.GetInt32(2);
                wallSect.Kind = reader.GetInt32(3);
                wallSect.B = reader.GetDouble(4);
                wallSect.H = reader.GetDouble(5);
                wallSect.T2 = reader.GetDouble(6);

                WallSects.Add(wallSect);
            }
        }
        public WallSect GetWallSect(long ID)
        {
            foreach (var wallSect in WallSects)
            {
                if (ID == wallSect.ID) return wallSect;
            }
            return null;
        }
        #endregion
    }
}
