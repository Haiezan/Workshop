﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Workshop.ModelData
{
    class StdFlr
    {
        public long ID;
        public long No;
        public double Height;
    }
    class Floor
    {
        public long ID;
        public long No;
        public string Name;
        public long StdFlrID;
        public double LevelB;
        public double Height;
    }
}
