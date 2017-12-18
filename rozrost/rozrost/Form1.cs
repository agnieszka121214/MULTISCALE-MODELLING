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
    public partial class Form1 : Form , GrowthModel.Displayer
    {

        private GrowthModel model;

        public Form1()
        {
            InitializeComponent();
           // pictureBox1.MouseClick += new MouseEventHandler(pictureBox1_Click);

            textBox1.Text = "100";
            textBox2.Text = "100";
            textBox3.Text = "20";
            textBox7.Text = "2";
            textBox6.Text = "3";
            textBox4.Text = "1";

            model = new GrowthModel();

            
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

            model.reset(w,h);
            model.randomizeColors(ilosc);
            pictureBox1.Image = model.display();
        }

        private void button2_Click(object sender, EventArgs e) // kolejne kroki czasowe dla wprowadzonych ziaren 
        {
            model.solve( this, 0, 0 );
            model.resetColors();
        }

        public void display(Bitmap bitmap) {
            pictureBox1.Image = bitmap;
            pictureBox1.Refresh();
        }

        private void exportToFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {

                int x = int.Parse(textBox1.Text);
                int y = int.Parse(textBox2.Text);

                StreamWriter SW = new StreamWriter(saveFileDialog1.FileName);

                SW.WriteLine(x.ToString());
                SW.WriteLine(y.ToString());


                for (int i = 0; i < x; i++)
                {
                    for (int j = 0; j < y; j++)
                    {
                        SW.WriteLine(i + " " + j + " " + model.GetGrain(i,j).stan + " " + model.GetGrain(i, j).index);
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

                textBox1.Text = x.ToString();
                textBox2.Text = y.ToString();
                model.reset(x, y);
                //tworzyc nowe ziarno
                for (int i = 0; i < x; i++)
                {
                    for (int j = 0; j < y; j++)
                    {
                        s = SR.ReadLine();
                        String[] t = s.Split(' ');
                        model.GetGrain(i, j).index = Int32.Parse(t[3]);
                        model.GetGrain(i, j).stan = Int32.Parse(t[2]);



                    }
                }
                SR.Close();
                pictureBox1.Image = model.display();
                System.Threading.Thread.Sleep(30);
            }
            this.Refresh();
        }
        private void exportToBitmapaToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }
        private void importFromBitmapToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }
        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            int ilosc = int.Parse(textBox7.Text);
            model.createInclusions(ilosc, GrowthModel.INCLUSION_RECT, int.Parse(textBox6.Text),true);
            pictureBox1.Image = model.display();
        }


        private void button4_Click(object sender, EventArgs e)
        {

            int ilosc = int.Parse(textBox7.Text);
            model.createInclusions(ilosc, GrowthModel.INCLUSION_CIRCLE ,int.Parse(textBox6.Text), true);
            pictureBox1.Image = model.display();
        }

        private void button6_Click(object sender, EventArgs e)
        {

        }

        private void button5_Click(object sender, EventArgs e)
        {
            int w = int.Parse(textBox1.Text); // wprowadzanie szerokosc
            int h = int.Parse(textBox2.Text); //wprowadzanie wysokosci
            int ilosc = int.Parse(textBox3.Text); // maksymalna ilosc pkunktow ktore chcemy wygenerowac

            model.reset(w, h);
            model.randomizeColors(ilosc);
            pictureBox1.Image = model.display();
        }

        private void button6_Click_1(object sender, EventArgs e)
        {
            model.solve(this ,  2 , 10);
            model.resetColors();
        }

        private void pictureBox1_Click(object sender, MouseEventArgs e)
        {

            //((Bitmap)pictureBox1.Image).GetPixel(e.Location.X, e.Location.Y);
            if (model.getIsCreateColorBoundaryOptionEnabled())
            {
                model.createBoundaryFromColor(e.Location.Y, e.Location.X);
                pictureBox1.Image = model.display();

            }
            else
            {
                model.addColor(e.Location.Y, e.Location.X);
            }
            

        }

        private void button8_Click(object sender, EventArgs e)
        {
            int ilosc = int.Parse(textBox7.Text);
            model.createInclusions(ilosc, GrowthModel.INCLUSION_RECT, int.Parse(textBox6.Text), false);
            pictureBox1.Image = model.display();

        }

        private void button7_Click(object sender, EventArgs e)
        {
            int ilosc = int.Parse(textBox7.Text);
            model.createInclusions(ilosc, GrowthModel.INCLUSION_CIRCLE, int.Parse(textBox6.Text), false);
            pictureBox1.Image = model.display();

        }

        
        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            pictureBox1_Click(sender,e);
        }

        private void button10_Click(object sender, EventArgs e)
        {
            
            int ilosc = int.Parse(textBox3.Text); // maksymalna ilosc pkunktow ktore chcemy wygenerowac
            

            model.reset2();
            model.randomizeColors(ilosc);
            pictureBox1.Image = model.display();
        }

        private void textBox6_TextChanged(object sender, EventArgs e)
        {

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void button11_Click(object sender, EventArgs e)
        {
           
            model.createInclusions2( GrowthModel.INCLUSION_RECT, int.Parse(textBox4.Text), true);
            
            model.reset3();
            pictureBox1.Image = model.display();
        }

        private void button12_Click(object sender, EventArgs e)
        {
            model.createBoundaryColor();
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {

        }

        private void button13_Click(object sender, EventArgs e)
        {
            //Monte Carlo 
        }

        private void button14_Click(object sender, EventArgs e)
        {
            int ilosc = int.Parse(textBox8.Text);
            int w = int.Parse(textBox1.Text); // wprowadzanie szerokosc
            int h = int.Parse(textBox2.Text); //wprowadzanie wysokosci
            if (!model.isGrainsInitialized()) {
                model.reset(w, h);
            } 
            model.prepareMC(ilosc);
            pictureBox1.Image = model.display();
        }

        private void button15_Click(object sender, EventArgs e)
        {
            int mcs = int.Parse(textBox5.Text);
            model.solveMC(this,mcs);
            model.resetColors();
        }

        private void button16_Click(object sender, EventArgs e)
        {
            int ilosc = int.Parse(textBox8.Text);
            int w = int.Parse(textBox1.Text); // wprowadzanie szerokosc
            int h = int.Parse(textBox2.Text); //wprowadzanie wysokosci

            int mcs = int.Parse(textBox5.Text);
            model.resetMCCA(); 
            pictureBox1.Image = model.display(); 
        }

        private void button18_Click(object sender, EventArgs e)
        {

            int ilosc = int.Parse(textBox3.Text); // maksymalna ilosc pkunktow ktore chcemy wygenerowac


            model.reset2();
            model.randomizeColors(ilosc);
            pictureBox1.Image = model.display();
        }

        private void button17_Click(object sender, EventArgs e)
        { 
            int ilosc = int.Parse(textBox9.Text);
            model.setSRXMC_MCSCount(ilosc);
            model.solve(this, 4, 0);
            model.resetColors(); 
        }

        private void button19_Click(object sender, EventArgs e)
        {
            int nType = 0;
            if (radioButton3.Checked) nType = 0;
            if (radioButton5.Checked) nType = 1;
            if (radioButton4.Checked) nType = 2;

            model.prepareSRXMC(
                int.Parse(textBox10.Text),
                nType,
                int.Parse(textBox13.Text),
                int.Parse(textBox14.Text),
                int.Parse(textBox15.Text),
                checkBox1.Checked,
                int.Parse(textBox11.Text),
                int.Parse(textBox12.Text),
                radioButton1.Checked
            );
        }

        private void label8_Click(object sender, EventArgs e)
        {

        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void button20_Click(object sender, EventArgs e)
        {

            model.setDisplayEnergyMode(!model.getDisplayEnergyMode());
            pictureBox1.Image = model.display();

        }
    }

}

