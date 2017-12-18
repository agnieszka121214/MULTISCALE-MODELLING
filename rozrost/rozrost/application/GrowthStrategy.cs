using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rozrost.application
{
    interface GrowthStrategy
    {

        bool canChangeGrain(Grain grain);

        bool canContinue(bool hasEmptyGrain);

        Grain[] getNeighborhood( Grain[,] grains,int x, int y, int left, int top, int botom, int right );

        Grain apply( Grain[] neighborhood, Grain target);

        void prepareLoop(Grain[,] grains, int width, int height);
    }
}
