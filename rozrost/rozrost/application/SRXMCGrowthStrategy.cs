using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rozrost.application
{
    class SRXMCGrowthStrategy : GrowthStrategy
    {

        private StrategyHelper helper = new StrategyHelper();

        private Random rand = new Random();

        private List<int> colors;

        private int MCS = 10;
        private int MCSCount = 0;
         
        private int nucleationRate = 10;
        private int nucleationStart = 10;
        private int nucleationIncrease = 10;

        private int nucleationCounter;

        private bool nucleationOnBorder;

        public SRXMCGrowthStrategy() { }

        public void setEnergyFunction(Grain [,] grains, int w , int h , int H1, int H2) { 
            for (int i = 0; i < w; i++ ) {
                for (int j = 0; j < h; j++)
                {
                    if (helper.isBoundary(grains, w, h, i, j))
                    {
                        grains[i, j].Hi = H1;
                    }
                    else
                    {
                        grains[i, j].Hi = H2;
                    }
                   
                }
            }
        }

        public void setMCSCount(int count)
        {
            MCS = count;
            MCSCount = 0;
        }

        public void setColorsCount(int count) {
            colors = new List<int>();
            int c = 0xF00;
            for (int i = 0; i < count; i++)
            {
                colors.Add(0xF00 |( rand.Next(255 * 255 * 255) & 0x0FF));
            }
        }

        public void setConstNucleation(int start, int rate) {
            nucleationRate = rate;
            nucleationStart = start;
            nucleationIncrease = 0;
            nucleationCounter = 0;
        }

        public void setNucleationsOnBorders(bool bordersOnly) {
            nucleationOnBorder = bordersOnly;
        }

        public void setIncreaseNucleation(int start, int rate, int inc)
        {
            nucleationRate = rate;
            nucleationStart = start;
            nucleationIncrease = inc;
            nucleationCounter = 0;
        }

        public void setBeginingNucleation(int start)
        {
            nucleationRate = 0;
            nucleationStart = start;
            nucleationIncrease = 0;
            nucleationCounter = 0;
        }

        public bool canChangeGrain(Grain grain)
        {
            return grain.stan == Grain.TYPE_OLD_GRAIN;
        }

        public bool canContinue(bool hasEmptyGrain)
        {
            MCSCount += 1;
            return MCSCount <= MCS;
        }

        public Grain[] getNeighborhood(Grain[,] grains, int x, int y, int left, int top, int bottom, int right)
        {
            return helper.getMooreNeighborhood(grains, x, y, left, top, bottom, right);
        }

        public Grain apply(Grain[] neighborhood, Grain target)
        { 
            //select neighbor randomly
            Grain neighbor = neighborhood[rand.Next(0, neighborhood.Length)];
            if ( neighbor.stan == Grain.TYPE_OLD_GRAIN ) {
                return target;
            } 

            int E0 = helper.calcEnergy(target, neighborhood) + target.Hi;
            Grain randGrain = Grain.createNewGrain(neighbor.index);
            int E1 = helper.calcEnergy(randGrain, neighborhood);
            int dE = E1 - E0;
            if (dE < 0)
            {
                return helper.applyGrainColor(randGrain.index);
            }
            else
            {
                return target;
            } 
        }

        public void prepareLoop(Grain[,] grains, int width, int height) {
            if (nucleationCounter == 0)
            {
                randomNucleations(grains, width, height, nucleationStart);
            } else if (nucleationRate == 0)
            {
                //To nathing
            }
            else if(nucleationCounter % nucleationRate == 0) {
                randomNucleations(grains, width, height, nucleationStart + (nucleationCounter / nucleationRate) * nucleationIncrease);
            }
            nucleationCounter++;
        }

        private void randomNucleations(Grain[,] grains, int width, int height, int count) {
          
            for (int i = 0; i < count; i++)
            {

                int xpom = rand.Next(0, width - 1); // losowo wypełniamy ziarnami 
                int ypom = rand.Next(0, height - 1);
 
                if (nucleationOnBorder && !helper.isBoundary(grains, width, height, xpom, ypom))
                {
                    i--;
                    continue;
                } 

                if (grains[xpom, ypom] != null && grains[xpom, ypom].stan != Grain.TYPE_OLD_GRAIN)
                {
                    i--;
                    continue;
                }

                grains[xpom, ypom] = randomGrain();
                System.Console.WriteLine("X" + xpom + ": Y" + ypom);

            }
        }

        public Grain randomGrain()
        {
            Grain grain = new Grain();
            grain.stan = Grain.TYPE_GRAIN;
            grain.index = colors[rand.Next(0, colors.Count)];
            return grain;
        }

        internal void initialize(Grain[,] grains, int w, int h)
        {
            for (int i = 0; i < w; i++)
            {
                for (int j = 0; j < h; j++)
                {
                    if (grains[i, j] != null )
                    {
                        grains[i, j].stan = Grain.TYPE_OLD_GRAIN;
                    }
                }
            }
        }
    }
}
