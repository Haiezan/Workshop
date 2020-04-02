﻿using System;
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

                Grids.Add(grid);
            }
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

                Beams.Add(beam);
       
            }
        }
    }
}