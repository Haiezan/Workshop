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
        public long No;
        public long StdFlrID;
        public double Level;
        public double Height;
        public List<Surface> BeamSurface = new List<Surface>();
        public List<Surface> ColumnSurface = new List<Surface>();
        public List<Surface> WallSurface = new List<Surface>();
    }
}
