using System;
using System.Collections.Generic; 
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.IO;

namespace rozrost.application
{
    class GrowthModel
    {

        public interface Displayer {

            void display( Bitmap bitmap );

        }


        public static int INCLUSION_RECT = 0;
        public static int INCLUSION_CIRCLE = 1;

        private bool isCreateColorBoundaryOptionEnabled = false;

        private Grain[,] grains;
        private MonteCarloGrowthStrategy monteCarlo;
        private SRXMCGrowthStrategy srxmcGrowth;

        private int width = 0;
        private int height = 0;

        private bool displayEnergymode = false;


        private List<int> listColors;
        //lista kolorow ktore beda zapamietywane po klik

        public GrowthModel() {
            this.grains = new Grain[0, 0];
            this.listColors = new List<int>();
            //pusta lista - do zapisywania kolorow
        }

        public bool isGrainsInitialized() {
            return width != 0 && height != 0;
        }

        public void setDisplayEnergyMode(bool a)
        {
            displayEnergymode = a;
        }

        public bool getDisplayEnergyMode()
        {
            return displayEnergymode;
        }

        public void reset(int w, int h) {
            this.width = w;
            this.height = h;
        
            this.grains = new Grain[width, height];
            for (int i = 0; i < width; i++){
                for (int j = 0; j < height; j++){
                    grains[i, j] = new Grain(); 
                }
            }

            resetColors();
            resetBoundaryColor();
        }

        public void randomizeColors(int count) {

            Random rand = new Random();

            for (int i = 0; i < count; i++){
                int randX = rand.Next(0, width - 1);
                int randY = rand.Next(0, height - 1);

                if (!ifGrainIsInColorList(grains[randX, randY])) {
                    grains[randX, randY].stan = Grain.TYPE_GRAIN; 
                    grains[randX, randY].index = rand.Next(255*255*255);
                }
               
            }

        }

        public void reset2()
        {
            //resetuje wszystkie ktore nie są zaznaczone na tej liscie z kolorami 

            //this.grains = new Grain[width, height];
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    if (!ifGrainIsInColorList(grains[i, j])) {
                        grains[i, j] = new Grain();
                    }
                    
                }
            }

        }

        public void resetMCCA()
        {
            Grain[,] tab = new Grain[width, height];
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    if (ifGrainIsInColorList(grains[i, j]))
                    {
                        tab[i, j] = Grain.createOldGrain(listColors.First());
                    }
                    else {
                        tab[i, j] = Grain.createEmptyGrain();
                    }

                }
            }
            this.grains = tab;

        }

        public void reset3()
        {
            //resetuje wszystkie ktore nie są zaznaczone na tej liscie z kolorami 

            //this.grains = new Grain[width, height];
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    if (changeState(grains[i, j]))
                    {
                        grains[i, j] = new Grain();
                    }

                }
            }

        }

        public Grain GetGrain(int x, int y)
        {
            return grains[x, y];
        }

        public void addColor(int x, int y) {
            x = x / 5;
            y = y / 5;
            //dostaje x i y z klikniecia - dziele przez 5 bo piksele
            //dostaje x y tablicy


            Grain grain = grains[x,y];

            if (grain.stan == Grain.TYPE_GRAIN) {
                listColors.Add(grain.index);
            }


        }

        public bool getIsCreateColorBoundaryOptionEnabled()
        {
            return isCreateColorBoundaryOptionEnabled;
        }
        //czy to ziarno jest na liscie kolorow 

        private bool ifGrainIsInColorList(Grain grain) { 
            for ( int i = 0; i < listColors.Count; i++ ) {
                if (listColors[i] == grain.index) {
                    return true;
                }
            }
            return false;
        }

        public void createBoundaryFromColor(int x, int y)
        {
           
                x = x / 5;
                y = y / 5;
            Grain[,] tab2 = new Grain[width, height];
                //dostaje x i y z klikniecia - dziele przez 5 bo piksele
                //dostaje x y tablicy

                Grain grain = grains[x, y];

                if (grain.stan == Grain.TYPE_GRAIN)
                {
                for (int i = 0; i < width; i++)
                {
                    for (int j = 0; j < height; j++)
                    {
                        Grain g = grains[i, j];


                        int gora = j - 1;
                        int dol = j + 1;
                        int prawy = i + 1;
                        int lewy = i - 1;
                        //perdiodyczny warunek brzegowy
                        if (i == 0) lewy = width - 1;
                        if (i == width - 1) prawy = 0;
                        if (j == 0) gora = height - 1;
                        if (j == height - 1) dol = 0;
                        //liczy sasiedztwo po to zeby policzyc tablice kolorow
                        //jesli sa w sasiedztwie dwa rozne kolory to jest granica

                        // ziarno ktore 
                        Dictionary<int, int> mapa =  getColorsDictionary(grains, i, j, lewy, gora, dol, prawy);

                        if( g.index == grain.index && mapa.Count>=2)
                        {
                            tab2[i, j] = new Grain();
                            tab2[i, j].index = grain.index;
                            tab2[i, j].stan = Grain.TYPE_INCLUSION;
                                                
                        }
                        else
                        {
                            tab2[i, j] = new Grain();
                            tab2[i, j].index = 0;
                            tab2[i, j].stan = Grain.TYPE_EMPTY;
                        }

                    }
                }


                rewriteGrains(tab2);
                resetBoundaryColor();

            }

            
        }

        private bool changeState(Grain grain)
        {
            bool change = true;
            if(grain.stan==Grain.TYPE_GRAIN)
                change = true;
            else
                change = false;

            return change;
        }
        //resetuje kolory
        public void resetColors() {
            this.listColors = new List<int>();
        }

        public int Inclusion(int w, int h, Grain grain) {
            int ile = 0; 
            for (int i = 0; i < w; i++)
            {
                for(int j= 0; j<h; j++)
                {
                    grains[i, j].stan = Grain.TYPE_GRAIN;
           
                }
            }
            return ile;
            
        }

        public void createInclusions(int count, int type, int size, bool onlyBorder) {

            Random losuj = new Random();

            for (int i = 0; i < count; i++)
            {
                int xpom = losuj.Next(0, width - 1); // losowo wypełniamy ziarnami 
                int ypom = losuj.Next(0, height - 1);
                
                if (onlyBorder) {
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
                    if (getColorsDictionary(grains, xpom,ypom,lewy,gora,dol,prawy).Count < 2) {
                        i--;
                        continue;
                    }
                }

                grains[xpom, ypom].stan = Grain.TYPE_INCLUSION; // ustawienie wprowadzonych ziaren na zamalowanie
                grains[xpom, ypom].index = 0; // tworzenie numerów ziaren i++ , do kazdego ziarna przypisany nowy numer 


                if (type == INCLUSION_CIRCLE)
                {
                    for (int ii = 0; ii < width; ii++)
                    {
                        for (int jj = 0; jj < height; jj++)
                        {
                            int dt = (int)Math.Sqrt((ii - xpom) * (ii - xpom) + (jj - ypom) * (jj - ypom));
                            if (dt <= size)
                            {
                                grains[ii, jj].stan = Grain.TYPE_INCLUSION; // ustawienie wprowadzonych ziaren na zamalowanie
                                grains[ii, jj].index = 0;
                            }
                        }
                    }
                }
                else if (INCLUSION_RECT == type)
                {
                    for (int ii = 0; ii < width; ii++)
                    {
                        for (int jj = 0; jj < height; jj++)
                        {
                            int dt = (int)Math.Sqrt((ii - xpom) * (ii - xpom) + (jj - ypom) * (jj - ypom));
                            if (Math.Abs(ii - xpom) <= size && Math.Abs(jj - ypom) <= size)
                            {
                                grains[ii, jj].stan = Grain.TYPE_INCLUSION; // ustawienie wprowadzonych ziaren na zamalowanie
                                grains[ii, jj].index = 0;
                            }
                        }
                    }
                }
            }

        }

        public void createInclusions2(int type, int size, bool onlyBorder)
        {

            Random losuj = new Random();

            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {

                    int xpom = i;
                    int ypom = j;

                    if (onlyBorder)
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
                        if (getColorsDictionary(grains, xpom, ypom, lewy, gora, dol, prawy).Count < 2)
                        {
                            
                            continue;
                        }
                    }

                    grains[xpom, ypom].stan = Grain.TYPE_INCLUSION; // ustawienie wprowadzonych ziaren na zamalowanie
                    grains[xpom, ypom].index = 0; // tworzenie numerów ziaren i++ , do kazdego ziarna przypisany nowy numer 


                    if (type == INCLUSION_CIRCLE)
                    {
                        for (int ii = 0; ii < width; ii++)
                        {
                            for (int jj = 0; jj < height; jj++)
                            {
                                int dt = (int)Math.Sqrt((ii - xpom) * (ii - xpom) + (jj - ypom) * (jj - ypom));
                                if (dt <= size)
                                {
                                    grains[ii, jj].stan = Grain.TYPE_INCLUSION; // ustawienie wprowadzonych ziaren na zamalowanie
                                    grains[ii, jj].index = 0;
                                }
                            }
                        }
                    }
                    else if (INCLUSION_RECT == type)
                    {
                        for (int ii = 0; ii < width; ii++)
                        {
                            for (int jj = 0; jj < height; jj++)
                            {
                                int dt = (int)Math.Sqrt((ii - xpom) * (ii - xpom) + (jj - ypom) * (jj - ypom));
                                if (Math.Abs(ii - xpom) <= size && Math.Abs(jj - ypom) <= size)
                                {
                                    grains[ii, jj].stan = Grain.TYPE_INCLUSION; // ustawienie wprowadzonych ziaren na zamalowanie
                                    grains[ii, jj].index = 0;
                                }
                            }
                        }
                    }
                }
            }
        

        }

        public Bitmap display() {
            Bitmap mapa = new Bitmap(width * 5, height * 5);
            Graphics graph = Graphics.FromImage(mapa);


            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {

                    if (grains[i, j].stan == Grain.TYPE_GRAIN || grains[i, j].stan == 9)
                    {
                        graph.FillRectangle(new SolidBrush(Color.FromArgb(255,
                              grains[i, j].index % 255,
                              grains[i, j].index / 255 % 255,
                              grains[i, j].index / (255 * 255) % 255
                            )), j * 5, i * 5, 5, 5);
                        //liczymy kolor z indeksu
                    }
                    else if (grains[i, j].stan == Grain.TYPE_INCLUSION)
                    {
                        graph.FillRectangle(new SolidBrush(Color.FromArgb(0, 0, 0)), j * 5, i * 5, 5, 5);
                    }
                    else if (grains[i, j].stan == Grain.TYPE_OLD_GRAIN)
                    {
                        if (displayEnergymode)
                        {
                            int color = (int) (255  * 255*255* (grains[i, j].Hi/10.0));
                            graph.FillRectangle(new SolidBrush(Color.FromArgb(255,
                                                         color % 255,
                                                         color / 255 % 255,
                                                         color / (255 * 255) % 255
                                                        )), j * 5, i * 5, 5, 5);
                        }
                        else
                        {
                            graph.FillRectangle(new SolidBrush(Color.FromArgb(255,
                                  grains[i, j].index % 255,
                                  grains[i, j].index / 255 % 255,
                                  grains[i, j].index / (255 * 255) % 255
                                )), j * 5, i * 5, 5, 5);
                        }
                    }
                    else
                    {
                        graph.FillRectangle(Brushes.White, j * 5, i * 5, 5, 5);
                    }
                }
            }
            return mapa;
        }


        public void solve(Displayer displayer, int param, int percent) {
            GrowthStrategy growthStrategy;
            bool doRandomGrainPick = false;
            if (param == 3)
            {
                growthStrategy = monteCarlo;
            }
            else if (param == 2 )
            { 
                growthStrategy = new Moore2GrowthStrategy(percent);
            }
            else if (param == 4)
            {
                growthStrategy = srxmcGrowth;
                doRandomGrainPick = true;
            }
            else
            {
                growthStrategy = new MooreGrowthStrategy();
            }
              
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    if (grains[i, j] != null && grains[i, j].stan == Grain.TYPE_OLD_GRAIN) {
                        //...
                    } else if (ifGrainIsInColorList(grains[i,j])) {
                        grains[i, j].stan = 9;
                        //przypisuje ziarną stan 9 zey nie byly edytowane 
                        
                    }
                }
            }

            Grain[,] tab2 = new Grain[width, height];

            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    tab2[i, j] = new Grain();
                }
            }
            bool hasEmptyGrain = true;
            do {
                growthStrategy.prepareLoop(grains,width,height);

                if (doRandomGrainPick) {
                    Random rand = new Random();
                 
                    IList<int[]> pozList = new List<int[]>();

                    for (int i = 0; i < width; i++)
                    {
                        for (int j = 0; j < height; j++)
                        { 
                            pozList.Add(new int[2] { i, j }); 
                        } 
                    }
                    
                    while (pozList.Count > 0 ) {
                        int randX = rand.Next(0, pozList.Count); 
                        int[] X = pozList[randX]; 
                        solveInternal(growthStrategy, tab2, X[0], X[1]);
                        pozList.Remove(X); 
                    } 

                } else {
                    for (int i = 0; i < width; i++)
                    {
                        for (int j = 0; j < height; j++)
                        {
                            solveInternal(growthStrategy, tab2, i, j);
                        }
                    }
                }

                hasEmptyGrain = rewriteGrains(tab2);
                displayer.display( display() );
                System.Threading.Thread.Sleep(10); 
            } while (growthStrategy.canContinue(hasEmptyGrain)); 

        }

        private void solveInternal(GrowthStrategy growthStrategy,Grain[,] tab2, int i, int j) {
            if (!growthStrategy.canChangeGrain(grains[i, j]))
            {
                tab2[i, j].index = grains[i, j].index;
                tab2[i, j].stan = grains[i, j].stan;
                return;
            }

            int gora = j - 1;
            int dol = j + 1;
            int prawy = i + 1;
            int lewy = i - 1;
            //perdiodyczny warunek brzegowy
            if (i == 0) lewy = width - 1;
            if (i == width - 1) prawy = 0;
            if (j == 0) gora = height - 1;
            if (j == height - 1) dol = 0;

            Grain[] neighborhood = growthStrategy.getNeighborhood(grains, i, j, lewy, gora, dol, prawy);

            Grain grain = growthStrategy.apply(neighborhood, grains[i, j]);

            tab2[i, j].index = grain.index;
            tab2[i, j].stan = grain.stan;
        }


        /**
         * rewriteGrains - przepisanie tablicy pomocniczej do głownej tablicy,
         * zwraca true jeśli tablica zawiera jeszcze puste ziarno
         */
        private bool rewriteGrains(Grain[,] tab2) {
            int puste = 0;
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    grains[i, j].stan = tab2[i, j].stan; //
                    grains[i, j].index = tab2[i, j].index;
                    if (tab2[i, j].stan == Grain.TYPE_EMPTY)
                    {
                        puste++;
                    }
                }
            }
            return puste != 0;
        }

        public void createBoundaryColor()
        {
            isCreateColorBoundaryOptionEnabled = true;

        }
        public void resetBoundaryColor()
        {
            isCreateColorBoundaryOptionEnabled = false;

        }


        private Dictionary<int, int> getColorsDictionary(Grain[,] grains, int x, int y, int left, int top, int botom, int right)
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

        public void prepareMC(int ilosc) { 
            monteCarlo = new MonteCarloGrowthStrategy();
            monteCarlo.randomGrains(grains,width,height, ilosc);
        }

        public void prepareSRXMC(int colors, int nType, int nStart, int nRate, int nInc, bool nOnlyBorders, int h1, int h2, bool isHomogeous)
        {
            srxmcGrowth = new SRXMCGrowthStrategy();
            srxmcGrowth.initialize(grains,width,height);
            if (nType == 0)
            {
                srxmcGrowth.setConstNucleation(nStart, nRate);
            }
            else if (nType == 1)
            {
                srxmcGrowth.setIncreaseNucleation(nStart, nRate, nInc);
            }
            else if (nType == 2)
            {
                srxmcGrowth.setBeginingNucleation(nStart);
            }
            if (isHomogeous)
            {
                srxmcGrowth.setEnergyFunction(grains, width, height, h1, h1);
            }
            else
            {
                srxmcGrowth.setEnergyFunction(grains, width, height, h1, h2);
            }
            srxmcGrowth.setColorsCount(colors);
            srxmcGrowth.setNucleationsOnBorders(nOnlyBorders);
            
        }

        public void setSRXMC_MCSCount(int ilosc)
        {
            srxmcGrowth.setMCSCount(ilosc);
        }

        public void solveMC(Displayer displayer, int mcs) {
            monteCarlo.setMCSCount(mcs);
            solve(displayer,3,0);
        }
    }
}
