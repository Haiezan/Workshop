using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rhino.Geometry;

namespace Workshop.ModelData
{
    class Joint
    {
        public long ID;
        public long StdFlrID;

        public Point3d Point;

        public double HDiff;
    }
}
