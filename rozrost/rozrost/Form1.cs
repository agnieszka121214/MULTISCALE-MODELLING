using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System;
using System.IO;
using rozrost.application;



namespace rozrost
{
    public partial class Form1 : Form
    {
        ziarno[,] tab1; //tablica początkowa - generuj ziarna START
        ziarno[,] tab2; //tablica do przechowywania kolejnych kroków czasowych
        Color[] kolor; // tablica kolorów
        int indeks;

        private GrowthProgram program;

        public Form1()
        {
            InitializeComponent();
            textBox1.Text = "100";
            textBox2.Text = "100";
            textBox3.Text = "20";
            textBox7.Text = "2";
            textBox6.Text = "2";

            program = new GrowthProgram();

            createEmptyTablice();
            refreshImage();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e) // generowanie wprowadzonych ziaren 
        {
            int w = int.Parse(textBox1.Text); // wprowadzanie szerokosc
            int h = int.Parse(textBox2.Text); //wprowadzanie wysokosci
            int ilosc = int.Parse(textBox3.Text); // maksymalna ilosc pkunktow ktore chcemy wygenerowac

            program.reset(w,h);
            program.randomizeColors(ilosc);
            pictureBox1.Image = program.display();
        }

        private void button2_Click(object sender, EventArgs e) // kolejne kroki czasowe dla wprowadzonych ziaren 
        {

            int x = int.Parse(textBox1.Text);
            int y = int.Parse(textBox2.Text);
            //
            int[] suma = new int[indeks];
            int lewy, prawy, gora, dol;
            int wartosc, pomocniczyIndeks;
            int puste = 10;

            while (puste > 0)
            {
                puste = 0;
                for (int i = 0; i < y; i++)
                {
                    for (int j = 0; j < x; j++)
                    {
                        //warunki brzegowe 

                        gora = i - 1;
                        dol = i + 1;
                        prawy = j + 1;
                        lewy = j - 1;
                        //perdiodyczny warunek brzegowy
                        if (i == 0) gora = y - 1; //lewy górny róg 
                        if (i == y - 1) dol = 0; // dolny lewy róg
                        if (j == 0) lewy = x - 1; // prawy góry róg
                        if (j == x - 1) prawy = 0; //dolny prawy róg

                        wartosc = 0;
                        pomocniczyIndeks = 0;
                        for (int k = 1; k < indeks; k++) //pętla po indeksach ziaren ktory został wczesniej obliczony
                        {
                            suma[k] = 0;
                            //sprawdzanie wystepujacych obok siebie kolorów
                            if (tab1[gora, lewy].indeks == k) suma[k]++; // 
                            if (tab1[gora, j].indeks == k) suma[k]++; //  
                            if (tab1[gora, prawy].indeks == k) suma[k]++;

                            if (tab1[i, lewy].indeks == k) suma[k]++;
                            if (tab1[i, prawy].indeks == k) suma[k]++;

                            if (tab1[dol, lewy].indeks == k) suma[k]++;
                            if (tab1[dol, j].indeks == k) suma[k]++;
                            if (tab1[dol, prawy].indeks == k) suma[k]++;

                            if (suma[k] > wartosc)
                            {
                                wartosc = suma[k];
                                pomocniczyIndeks = k; //zapamietanie dla ktorego k sprawdzalismy kolor
                            }
                        }
                        //warunek gdy martwe ziarno staje sie żywym ziarem
                        //sprawdzanie na ktory kolor bedzie zamalowywana
                        if (tab1[i, j].stan == 0 && wartosc > 0) //tab[i,j]- sprawdzana aktuanie komórka 
                        {
                            tab2[i, j].stan = 1;
                            tab2[i, j].indeks = pomocniczyIndeks; //numer rodzica rozszerzany jest o następne ziarna ( graficzny rozrost ziarna)
                        }
                    }
                }

                for (int i = 0; i < y; i++)
                {
                    for (int j = 0; j < x; j++)
                    {
                        tab1[i, j].stan = tab2[i, j].stan; //przepisanie tablicy pomocniczej do głownej tablicy 
                        tab1[i, j].indeks = tab2[i, j].indeks;
                        if (tab2[i, j].stan == 0)
                        {
                            puste++;
                        }
                    }
                }
                refreshImage();
                pictureBox1.Refresh();

                System.Threading.Thread.Sleep(30);
            }

        }

        private void exportToFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {

                int x = int.Parse(textBox1.Text);
                int y = int.Parse(textBox2.Text);
                int z = int.Parse(textBox3.Text);

                StreamWriter SW = new StreamWriter(saveFileDialog1.FileName);

                SW.WriteLine(x.ToString());
                SW.WriteLine(y.ToString());
                SW.WriteLine(z.ToString());

                for (int i = 0; i < z; i++)
                {
                    SW.WriteLine(((Int32)(kolor[i].ToArgb())).ToString());
                }

                for (int i = 0; i < x; i++)
                {
                    for (int j = 0; j < y; j++)
                    {
                        SW.WriteLine(i + " " + j + " " + tab2[i, j].stan + " " + tab2[i, j].indeks);
                    }
                }

                SW.Close();
            }
        }

        private void importFtomTxtToolStripMenuItem_Click(object sender, EventArgs e)
        {

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                StreamReader SR = new StreamReader(openFileDialog1.FileName);

                String s;
                int x = int.Parse(SR.ReadLine());
                int y = int.Parse(SR.ReadLine());
                int z = int.Parse(SR.ReadLine());

                textBox1.Text = x.ToString();
                textBox2.Text = y.ToString();
                textBox3.Text = z.ToString();

                for (int i = 0; i < z; i++)
                {
                    s = SR.ReadLine();
                    kolor[z] = Color.FromArgb(Int32.Parse(s));
                }

                for (int i = 0; i < x; i++)
                {
                    for (int j = 0; j < y; j++)
                    {
                        s = SR.ReadLine();
                        tab2[i, j].stan = 1;
                        tab2[i, j].indeks = Int32.Parse(s.Substring(s.LastIndexOf(' '), (s.Length - s.LastIndexOf(' '))));
                    }
                }
                SR.Close();

                refreshImage();
                System.Threading.Thread.Sleep(30);
            }
            this.Refresh();
        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            int ilosc = int.Parse(textBox7.Text);
            program.createInclusions(ilosc, GrowthProgram.INCLUSION_RECT, int.Parse(textBox6.Text));
            pictureBox1.Image = program.display();
        }

        private void createEmptyTablice()
        {

            int x = int.Parse(textBox1.Text); // wprowadzanie szerokosc
            int y = int.Parse(textBox2.Text); //wprowadzanie wysokosci
            int ilosc = int.Parse(textBox3.Text); // maksymalna ilosc pkunktow ktore chcemy wygenerowac

            Random losuj = new Random();
            pictureBox1.Size = new Size(x * 5, y * 5); //rozmiar picture Boxa 

            this.Controls.Add(pictureBox1);
            tab1 = new ziarno[y, x]; //tworzymy tablice przechowujące miejsce w ktorych bedzie rozrost ziaren
            tab2 = new ziarno[y, x];
            kolor = new Color[500]; //tablica kolorów

            for (int i = 0; i < 500; i++)
            {
              
                kolor[i] = Color.FromArgb(losuj.Next(256), losuj.Next(256), losuj.Next(256));//losowanie kolorów
            }
            indeks = 1; // najwyzszy aktualny indeks ziarna 

            for (int i = 0; i < y; i++)
            {
                for (int j = 0; j < x; j++)
                {
                    tab1[i, j] = new ziarno(); //inicjalizator wypełniania danymi , tablice obiektów typu ziarno
                    tab2[i, j] = new ziarno();

                    tab2[i, j].indeks = 0; // zerowanie
                    tab2[i, j].stan = 0;   // 
                }
            }
        }

        private void refreshImage()
        {
            int x = int.Parse(textBox1.Text); // wprowadzanie szerokosc
            int y = int.Parse(textBox2.Text); //wprowadzanie wysokosci
            Bitmap mapa = new Bitmap(x * 5, y * 5);
            Graphics graph = Graphics.FromImage(mapa);


            for (int i = 0; i < y; i++)
            {
                for (int j = 0; j < x; j++)
                {
                    if (tab1[i, j].stan == 1)
                        graph.FillRectangle(new SolidBrush(kolor[tab1[i, j].indeks]), j * 5, i * 5, 5, 5);
                    else if (tab1[i, j].stan == 5)
                        graph.FillRectangle(new SolidBrush(Color.FromArgb(0, 0, 0)), j * 5, i * 5, 5, 5);
                    else graph.FillRectangle(Brushes.White, j * 5, i * 5, 5, 5);

                }
                pictureBox1.Image = mapa;
                //pictureBox1.Refresh();
            }
        }

        private void rewriteTab()
        {
            int x = int.Parse(textBox1.Text); // wprowadzanie szerokosc
            int y = int.Parse(textBox2.Text); //wprowadzanie wysokosci
            for (int i = 0; i < y; i++)
            {
                for (int j = 0; j < x; j++)
                {
                    tab1[i, j].stan = tab2[i, j].stan; //przepisanie tablicy pomocniczej do głownej
                    tab1[i, j].indeks = tab2[i, j].indeks;
                }
            }
        }


        private void button4_Click(object sender, EventArgs e)
        {

            int ilosc = int.Parse(textBox7.Text);
            program.createInclusions(ilosc, GrowthProgram.INCLUSION_CIRCLE ,int.Parse(textBox6.Text));
            pictureBox1.Image = program.display();
        }

        private void button6_Click(object sender, EventArgs e)
        {

        }

        private void button5_Click(object sender, EventArgs e)
        {
            createEmptyTablice();
            refreshImage();
        }

        private void button6_Click_1(object sender, EventArgs e)
        {

            /*  int x = int.Parse(textBox1.Text);
              int y = int.Parse(textBox2.Text);
              //
              int[] suma = new int[indeks];
              int lewy, prawy, gora, dol;
              int wartosc, pomocniczyIndeks;
              int puste = 10;
              int max = 0;


              while (puste > 0)
              {
                  puste = 0;
                  for (int i = 0; i < y; i++)
                  {
                      for (int j = 0; j < x; j++)
                      {
                          //warunki brzegowe 

                          gora = i - 1;
                          dol = i + 1;
                          prawy = j + 1;
                          lewy = j - 1;

                          if (i == 0) gora = y - 1; //lewy górny róg 
                          if (i == y - 1) dol = 0; // dolny lewy róg
                          if (j == 0) lewy = x - 1; // prawy góry róg
                          if (j == x - 1) prawy = 0; //dolny prawy róg

                          wartosc = 0;
                          pomocniczyIndeks = 0;
                          for (int k = 1; k < indeks; k++) //
                          {
                              suma[k] = 0;

                              if (tab1[gora, lewy].indeks == k) suma[k]++; // 
                              if (tab1[gora, j].indeks == k) suma[k]++; //  
                              if (tab1[gora, prawy].indeks == k) suma[k]++;

                              if (tab1[i, lewy].indeks == k) suma[k]++;
                              if (tab1[i, prawy].indeks == k) suma[k]++;

                              if (tab1[dol, lewy].indeks == k) suma[k]++;
                              if (tab1[dol, j].indeks == k) suma[k]++;
                              if (tab1[dol, prawy].indeks == k) suma[k]++;

                              if (suma[k] >= 5 && suma[k]<8)
                              {
                                  wartosc = suma[k];
                                  pomocniczyIndeks = k; //zapamietanie dla ktorego k sprawdzalismy kolor
                              }
                              if (suma[k] >=3 && suma[k] <=4)
                              {
                                  wartosc = suma[k];
                                  pomocniczyIndeks = k; //zapamietanie dla ktorego k sprawdzalismy kolor

                              }


                          }

                          if (tab1[i, j].stan == 0 && wartosc > 0) //tab[i,j]- sprawdzana aktuanie komórka 
                          {
                              tab2[i, j].stan = 1;
                              tab2[i, j].indeks = pomocniczyIndeks; //numer rodzica rozszerzany jest o następne ziarna ( graficzny rozrost ziarna)
                          }
                      }
                  }/*
              int ind;
              int ilosc = int.Parse(textBox3.Text);
              int[] tMax = new int[indeks];

              for (int i = 0; i < x; i++)
              {
                  for (int j = 0; j < y; j++)
                  {
                      for (int k = 0; k < ilosc; k++)
                      {
                          if (tab1[i, j].indeks == k)
                          {
                              max++;

                          }



                                  }
                  }
        }*/
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }
    }

}

