using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rozrost.application
{
    class MooreGrowthStrategy : GrowthStrategy
    { 

        private bool grainChanged = false;

       public MooreGrowthStrategy() { }

       public bool canChangeGrain(Grain grain) {
            return grain.stan == Grain.TYPE_EMPTY;
       }

       public bool canContinue(bool hasEmptyGrain) {
            bool ret = hasEmptyGrain && grainChanged;
            grainChanged = false;
            return ret;
       }

        public Grain[] getNeighborhood(Grain[,] grains, int x, int y, int left, int top, int bottom, int right) {
            Grain[] neighborhood = new Grain[8];
            neighborhood[0] = grains[left,top];
            neighborhood[1] = grains[x, top];
            neighborhood[2] = grains[right, top];
            neighborhood[3] = grains[left, y];
            neighborhood[4] = grains[right, y]; 
            neighborhood[5] = grains[left, bottom];
            neighborhood[6] = grains[x, bottom];
            neighborhood[7] = grains[right, bottom];
            return neighborhood;
        }

        public Grain apply(Grain[] neighborhood, Grain target) {
            Dictionary<int, int> colors = new Dictionary<int, int>();
            for (int i = 0; i < neighborhood.Length; i++) {
                if (neighborhood[i] != null && neighborhood[i].stan == Grain.TYPE_GRAIN) {
                    int color = neighborhood[i].index;
                    if (colors.ContainsKey(color)) {
                        colors[color]++;
                    }  else {
                        colors[color] = 1;
                    }
                }        
            }
            if (colors.Count == 0) {
                return target;
            }
             
            int maxColor = colors.FirstOrDefault(x => x.Value == colors.Values.Max()).Key;
            if (target.stan == Grain.TYPE_EMPTY) {
                grainChanged = true;
            }
            Grain grain = new Grain();
            grain.stan = Grain.TYPE_GRAIN;
            grain.index = maxColor;
       
            return grain;
        }

        public void prepareLoop(Grain[,] grains, int width, int height) { }

    }
}
