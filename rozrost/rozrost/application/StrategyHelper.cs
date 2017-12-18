using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rozrost.application
{
    class StrategyHelper
    {
        public Dictionary<int, int> getColorsDictionary(Grain[] neighborhood)
        {
            Dictionary<int, int> colors = new Dictionary<int, int>();
            for (int i = 0; i < neighborhood.Length; i++)
            {
                if (neighborhood[i] != null && neighborhood[i].stan == Grain.TYPE_GRAIN)
                {
                    int color = neighborhood[i].index;
                    if (colors.ContainsKey(color))
                    {
                        colors[color]++;
                    }
                    else
                    {
                        colors[color] = 1;
                    }
                }
            }
            return colors;
        }

        public int getMostFrequentlyColor(Dictionary<int, int> colors)
        {
            return colors.FirstOrDefault(x => x.Value == colors.Values.Max()).Key;
        }

        public Grain applyGrainColor(int color)
        {
            Grain grain = new Grain();
            grain.stan = Grain.TYPE_GRAIN;
            grain.index = color;
            return grain;
        }

        public Grain[] getMooreNeighborhood(Grain[,] grains, int x, int y, int left, int top, int bottom, int right)
        {
            Grain[] neighborhood = new Grain[8];
            neighborhood[0] = grains[left, top];
            neighborhood[1] = grains[x, top];
            neighborhood[2] = grains[right, top];
            neighborhood[3] = grains[left, y];
            neighborhood[4] = grains[right, y];
            neighborhood[5] = grains[left, bottom];
            neighborhood[6] = grains[x, bottom];
            neighborhood[7] = grains[right, bottom];
            return neighborhood;
        }

        public bool isBoundary(Grain[,] grains, int width, int height, int xpom, int ypom)
        {
            int gora = ypom - 1;
            int dol = ypom + 1;
            int prawy = xpom + 1;
            int lewy = xpom - 1;
            //perdiodyczny warunek brzegowy
            if (xpom == 0) lewy = width - 1;
            if (xpom == width - 1) prawy = 0;
            if (ypom == 0) gora = height - 1;
            if (ypom == height - 1) dol = 0;
            //liczy sasiedztwo po to zeby policzyc tablice kolorow
            //jesli sa w sasiedztwie dwa rozne kolory to jest granica 
            return this.getColorsDictionary(grains, xpom, ypom, lewy, gora, dol, prawy).Count >= 2;
        }

        public Dictionary<int, int> getColorsDictionary(Grain[,] grains, int x, int y, int left, int top, int botom, int right)
        {

            Grain[] neighborhood = new Grain[8];
            neighborhood[0] = grains[left, top];
            neighborhood[1] = grains[x, top];
            neighborhood[2] = grains[right, top];
            neighborhood[3] = grains[left, y];
            neighborhood[4] = grains[right, y];
            neighborhood[5] = grains[left, botom];
            neighborhood[6] = grains[x, botom];
            neighborhood[7] = grains[right, botom];
            //wylicza sasiedztwo moora i zwraca slownik kolorow

            Dictionary<int, int> colors = new Dictionary<int, int>();
            for (int i = 0; i < neighborhood.Length; i++)
            {
                if (neighborhood[i] != null && (neighborhood[i].stan == Grain.TYPE_GRAIN || neighborhood[i].stan == Grain.TYPE_OLD_GRAIN))
                {
                    int color = neighborhood[i].index;
                    if (colors.ContainsKey(color))
                    {
                        colors[color]++;
                    }
                    else
                    {
                        colors[color] = 1;
                    }
                }
            }
            return colors;
        }

        public int calcEnergy(Grain currentGrain, Grain[] neighborhood)
        {
            Dictionary<int, int> colors = getColorsDictionary(neighborhood);
            return colors.Keys.Sum(x => (x != currentGrain.index) ? (colors[x]) : 0);
        }

    }
}
