using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rhino.Geometry;

namespace Workshop.ModelData
{
    class Grid
    {
        public long ID;
        public long StdFlrID;
        public long Jt1ID;
        public long Jt2ID;
        public long AxidID;

        public Point3d Jt1;
        public Point3d Jt2;
    }
}
