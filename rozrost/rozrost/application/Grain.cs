using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rozrost.application
{
    class Grain
    {

        public static int TYPE_EMPTY = 0;

        public static int TYPE_INCLUSION = 2;

        public static int TYPE_GRAIN = 1;

        public int index;

        public int stan;

        public Grain() {
            index = 0;
            stan = 0;
        }


    }
}
