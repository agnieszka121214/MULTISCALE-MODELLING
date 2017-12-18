using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rozrost.application
{
    class MonteCarloGrowthStrategy : GrowthStrategy
    {

        private StrategyHelper helper = new StrategyHelper();
        private Random rand = new Random();

        private List<int> colors;

        private int MCS = 10;
        private int MCSCount = 0;

        public MonteCarloGrowthStrategy() { }

        public void setMCSCount(int count) {
            MCS = count;
            MCSCount = 0;
        }

        public bool canChangeGrain(Grain grain) {
            return true;
        }

        public bool canContinue(bool hasEmptyGrain)
        {
            MCSCount += 1; 
            return MCSCount <= MCS;
        }

        public void randomGrains(Grain[,] grains, int w, int h, int colorsNum) {
            MCSCount = 0;
            colors = new List<int>();
            for (int i = 0; i < colorsNum; i++) {
                colors.Add(rand.Next(255 * 255 * 255));
            }
            for (int i = 0; i < w; i++)
            {
                for (int j = 0; j < h; j++)
                {
                    if (grains[i, j] != null && grains[i, j].stan != Grain.TYPE_OLD_GRAIN) {
                        grains[i, j] = randomGrain();
                    }
                }
            }

        }

        public Grain[] getNeighborhood(Grain[,] grains, int x, int y, int left, int top, int bottom, int right)
        {
            return helper.getMooreNeighborhood(grains, x, y, left, top, bottom, right);
        }

        public Grain apply(Grain[] mooreNeighborhood, Grain target)
        {
            if (target.stan == Grain.TYPE_OLD_GRAIN)
            {
                return Grain.createOldGrain(target.index);
            }

            int E0 = helper.calcEnergy(target, mooreNeighborhood);
            Grain randGrain = randomGrain();
            int E1 = helper.calcEnergy(randGrain, mooreNeighborhood);
            int dE = E1 - E0;
            if (dE <= 0)
            {
                return helper.applyGrainColor(randGrain.index);
            }
            else
            {
                return target;
            }
        }
         

        public Grain randomGrain() {
            Grain grain =  new Grain();
            grain.stan = Grain.TYPE_GRAIN;
            grain.index = colors[rand.Next(0, colors.Count)];
            return grain;
        }

        public void prepareLoop(Grain[,] grains, int width, int height) { }

    }
}
