using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rhino.Geometry;

namespace Workshop.ModelData
{
    class FloorSurface
    {
        public long ID;
        public long StdFlrID;
        public double Level;
        public double Height;
        public List<Surface> DisplaySurface = new List<Surface>();
    }
}
