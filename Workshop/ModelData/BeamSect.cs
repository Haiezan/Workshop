using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rhino.Geometry;

namespace Workshop.ModelData
{
    class BeamSect
    {
        public long ID;
        public long No;
        public string Name;
        public int Mat;
        public int Kind;
        public double B;
        public double H;
        public double U;
        public double T;
        public double D;
        public double F;

        public PolylineCurve curve;
    }
}
