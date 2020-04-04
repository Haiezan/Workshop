using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rhino.Geometry;

namespace Workshop
{
    class WallSect
    {
        public long ID;
        public long No;
        public int Mat;
        public int Kind;
        public double B;
        public double H;
        public double T2;

        public PolylineCurve curve;
    }
}
