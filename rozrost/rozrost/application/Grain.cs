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

        public static int TYPE_OLD_GRAIN = 3;

        public int index;

        public int stan;

        public int Hi;

        public static Grain createEmptyGrain() {
            return new Grain(TYPE_EMPTY,0);
        }

        public static Grain createOldGrain(int color)
        {
            return new Grain(TYPE_OLD_GRAIN, color);
        }

        public static Grain createNewGrain(int color)
        {
            return new Grain(TYPE_GRAIN, color);
        }

        private Grain(int type, int index)
        {
            this.index = index;
            this.stan = type;
        }

        public Grain() {
            index = 0;
            stan = 0;
        }


    }
}
