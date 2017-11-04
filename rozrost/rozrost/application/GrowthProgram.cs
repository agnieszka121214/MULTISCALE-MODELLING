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
    class GrowthProgram
    {
        public static int INCLUSION_RECT = 0;
        public static int INCLUSION_CIRCLE = 1;


        private Grain[,] grains;

        private int width;
        private int height;

        public GrowthProgram() {
            this.grains = new Grain[0, 0];
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
        }

        public void randomizeColors(int count) {

            Random rand = new Random();

            for (int i = 0; i < count; i++){
                int randX = rand.Next(0, width - 1);
                int randY = rand.Next(0, height - 1);

                grains[randX, randY].stan = Grain.TYPE_GRAIN; 
                grains[randX, randY].index = rand.Next(255*255*255);
               
            }

        }

        public void createInclusions(int count, int type, int size) {

            Random losuj = new Random();

            for (int i = 0; i < count; i++)
            {
                int xpom = losuj.Next(0, width - 1); // losowo wypełniamy ziarnami 
                int ypom = losuj.Next(0, height - 1);
                grains[ypom, xpom].stan = Grain.TYPE_INCLUSION; // ustawienie wprowadzonych ziaren na zamalowanie
                grains[ypom, xpom].index = 0; // tworzenie numerów ziaren i++ , do kazdego ziarna przypisany nowy numer 

                if (type == INCLUSION_CIRCLE)
                {
                    for (int ii = 0; ii < width; ii++)
                    {
                        for (int jj = 0; jj < height; jj++)
                        {
                            int dt = (int)Math.Sqrt((ii - xpom) * (ii - xpom) + (jj - ypom) * (jj - ypom));
                            if (dt <= size)
                            {
                                grains[jj, ii].stan = Grain.TYPE_INCLUSION; // ustawienie wprowadzonych ziaren na zamalowanie
                                grains[jj, ii].index = 0;
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
                                grains[jj, ii].stan = Grain.TYPE_INCLUSION; // ustawienie wprowadzonych ziaren na zamalowanie
                                grains[jj, ii].index = 0;
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
                    if (grains[i, j].stan == Grain.TYPE_GRAIN){
                        graph.FillRectangle(new SolidBrush(Color.FromArgb(255,
                              grains[i,j].index % 255,
                              grains[i, j].index / 255 % 255,
                              grains[i, j].index / (255*255) % 255
                            )), j * 5, i * 5, 5, 5);
                    }
                    else if (grains[i, j].stan == Grain.TYPE_INCLUSION) {
                        graph.FillRectangle(new SolidBrush(Color.FromArgb(0, 0, 0)), j * 5, i * 5, 5, 5);
                    }
                    else {
                        graph.FillRectangle(Brushes.White, j * 5, i * 5, 5, 5);
                    }
                }
            }
            return mapa;
        }

      
    }
}
