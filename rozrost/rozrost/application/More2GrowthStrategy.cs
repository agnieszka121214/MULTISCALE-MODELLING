using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rozrost.application
{
    class Moore2GrowthStrategy : GrowthStrategy
    {
        private int p;
        private Random rand;

        private StrategyHelper helper = new StrategyHelper();

        public Moore2GrowthStrategy(int p) {
            this.p = p;
            this.rand = new Random();
        }

        public bool canChangeGrain(Grain grain)
        {
            return grain.stan == Grain.TYPE_EMPTY;
        }

        public bool canContinue(bool hasEmptyGrain)
        {
            return hasEmptyGrain;
        }

        public Grain[] getNeighborhood(Grain[,] grains, int x, int y, int left, int top, int bottom, int right)
        { 
            return helper.getMooreNeighborhood(grains,x,y,left,top,bottom,right);
        }

        public Grain apply(Grain[] mooreNeighborhood, Grain target)
        {
            Dictionary<int, int> colors = helper.getColorsDictionary(mooreNeighborhood);

            if (colors.Count == 0) {
                return target;
            }

            if (rule1(colors))
            {
               return helper.applyGrainColor(helper.getMostFrequentlyColor(colors));
            }

            colors = helper.getColorsDictionary(getMooreNerestNeighborhood(mooreNeighborhood));
            if (rule2(colors))
            {
                return helper.applyGrainColor(helper.getMostFrequentlyColor(colors));
            }

            colors = helper.getColorsDictionary(getMooreFurtherNeighborhood(mooreNeighborhood));
            if (rule3(colors))
            {
                return helper.applyGrainColor(helper.getMostFrequentlyColor(colors));
            }

            colors = helper.getColorsDictionary(mooreNeighborhood);
            if (rule4(colors))
            {
                return helper.applyGrainColor(helper.getMostFrequentlyColor(colors));
            }

            return target;
        }

        private bool rule1(Dictionary<int, int> colors) {
            if (colors.Count == 0) return false;
            return colors[helper.getMostFrequentlyColor(colors)] >= 5;
        }

        private bool rule2(Dictionary<int, int> colors)
        {
            if (colors.Count == 0) return false;
            return colors[helper.getMostFrequentlyColor(colors)] >= 3;
        }

        private bool rule3(Dictionary<int, int> colors)
        {
            if (colors.Count == 0) return false;
            return colors[helper.getMostFrequentlyColor(colors)] >= 3;
        }

        private bool rule4(Dictionary<int, int> colors)
        {
            if (colors.Count == 0) return false; 
            int randX = rand.Next(0,100);
            return randX < p;
        }
         
        public Grain[] getMooreFurtherNeighborhood(Grain[] mooreNeighborhood) {
            Grain[] neighborhood = new Grain[8];
            neighborhood[0] = mooreNeighborhood[0];
            neighborhood[1] = null;
            neighborhood[2] = mooreNeighborhood[2];
            neighborhood[3] = null;
            neighborhood[4] = null;
            neighborhood[5] = mooreNeighborhood[5];
            neighborhood[6] = null;
            neighborhood[7] = mooreNeighborhood[7];
            return neighborhood;
        }

        public Grain[] getMooreNerestNeighborhood(Grain[] mooreNeighborhood)
        {
            Grain[] neighborhood = new Grain[8];
            neighborhood[0] = null;
            neighborhood[1] = mooreNeighborhood[1];
            neighborhood[2] = null;
            neighborhood[3] = mooreNeighborhood[3];
            neighborhood[4] = mooreNeighborhood[4];
            neighborhood[5] = null;
            neighborhood[6] = mooreNeighborhood[6];
            neighborhood[7] = null;
            return neighborhood;
        }

        public void prepareLoop(Grain[,] grains, int width, int height) { }

    }
}
