using Accord.Math.Decompositions;
using SharpGL;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static Zobrazovani.Matice;
using static Zobrazovani.Mracna;

namespace Zobrazovani
{
    public partial class Form1 : Form
    {
        Thread spust;
        bool run;
        bool mracno1load = false;
        bool mracno2load = false;
        static int modulo = 1;
        int vmodulo = 60;
        string cesta1;
        string cesta2;
        List<double[]> mracno1 = new List<double[]>();
        List<double[]> mracno2 = new List<double[]>();
        public float x = 0f;
        float y = 0f;
        float z = -90f;
        float rx = 0f;
        float ry = 0f;
        float rz = 0f;
        Point pozice;
        double r;
        
        double[,] MatRot = new double[3, 3];
        double[,] transl = new double[3, 1];
        double[] centr = new double[3];

        public Form1()
        {

            InitializeComponent();

        }

        private void OpenglControl1_OpenglDraw(object sender, RenderEventArgs e)
        {

            OpenGL gl = this.openglControl1.OpenGL;
            gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);  
            gl.LoadIdentity();
            gl.Translate(x, y, z);
            gl.Rotate(rx, ry, rz);
            gl.PointSize(1f);
            gl.LineWidth(0.1f);

            gl.Begin(OpenGL.GL_POINTS);

            gl.Color(0.8f, 0.9f, 1f);
            foreach (var bod in mracno1)
            {

                gl.Vertex(bod);

            }

            gl.Color(0f, 1f, 0f);
            foreach (var bod in mracno2)
            {

                gl.Vertex(bod);

            }

            gl.End();

        }

        private static List<double[]> NactiPly(string nazev)
        {
            bool konechlavicky = false;
            bool pocet = false;
            List<double[]> body = new List<double[]>();

            using (StreamReader sr = new StreamReader(nazev))
            {
                int iterace = 0;
                int pocetbodu = 0;
                int pocetsten = 0;
                string radek;

                while (konechlavicky == false)
                {
                    radek = sr.ReadLine();
                    if (radek.Contains("element vertex"))
                    {
                        radek.Trim();
                        radek = radek.Substring(15);
                        pocet = Int32.TryParse(radek, out pocetbodu);
                    }
                    if (radek.Contains("element face"))
                    {
                        radek.Trim();
                        radek = radek.Substring(13);
                        pocet = Int32.TryParse(radek, out pocetsten);
                    }
                    if (radek.Contains("end_header"))
                    {
                        konechlavicky = true;
                    }
                }

                int j = 0;
                while (iterace < pocetbodu && (radek = sr.ReadLine()) != null)
                {

                    if (radek.Length > 0)
                    {
                        radek = radek.Replace(".", ",");
                        string[] novyradek = radek.Split(" ");
                        List<double> bod = new List<double>();
                        double[] souradnice = new double[3];
                        for (int i = 0; i < 3; i++)
                        {
                            bool b = double.TryParse(novyradek[i], out souradnice[i]);
                        }

                        if ((j % modulo) == 0)
                        {
                            body.Add(souradnice);
                        }
                        iterace++;
                    }
                    j++;
                }

            }

            return body;
        }


        private void openglControl1_Scroll(object sender, ScrollEventArgs e)
        {
            z = z + (e.NewValue - e.OldValue);
            openglControl1.DoRender();
        }


        private void openglControl1_MouseMove(object sender, MouseEventArgs e)
        {

            if (e.Button == MouseButtons.Right)
            {

                if (pozice.X > e.X)
                {
                    ry = ry - Math.Abs(e.X - pozice.X);
                }
                else
                {
                    ry = ry + Math.Abs(e.X - pozice.X);
                }

                if (pozice.Y > e.Y)
                {
                    rx = rx - Math.Abs(e.Y - pozice.Y);
                }
                else
                {
                    rx = rx + Math.Abs(e.Y - pozice.Y);
                }

            }
            if (e.Button == MouseButtons.Left)
            {
                if (pozice.X > e.X)
                {
                    x = x - Math.Abs(e.X - pozice.X) * 0.01f*(float)r;
                }
                else
                {
                    x = x + Math.Abs(e.X - pozice.X) * 0.01f *(float)r;
                }
                if (pozice.Y > e.Y)
                {
                    y = y + Math.Abs(e.Y - pozice.Y) * 0.01f * (float)r;
                }
                else
                {
                    y = y - Math.Abs(e.Y - pozice.Y) * 0.01f * (float)r;
                }
            }
            pozice = e.Location;

        }

        private void ICP_MouseClick(object sender, MouseEventArgs e)
        {
            if (mracno1load && mracno2load)
            {
                if (run == false)
                {
                    run = true;
                    spust = new Thread(new ThreadStart(delegate ()
                     {
                         ICP(mracno1, mracno2, Convert.ToDouble(textBox1.Text), Decimal.ToInt32(numericUpDown1.Value));
                     }));
                    spust.IsBackground = true;
                    spust.Start();

                }
            }
        }

        private void ICPN_MouseClick(object sender, MouseEventArgs e)
        {
            if (mracno1load && mracno2load)
            {
                if (run == false)
                {
                    run = true;
                    spust = new Thread(new ThreadStart(delegate ()
                    {
                        ICPN(mracno1, mracno2, Convert.ToDouble(textBox1.Text), Decimal.ToInt32(numericUpDown1.Value));

                    }));

                    spust.IsBackground = true;
                    spust.Start();

                }
            }
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) &&
        (e.KeyChar != ','))
            {
                e.Handled = true;
            }
        }

        private void openglControl1_Resized(object sender, EventArgs e)
        {
            OpenGL gl = openglControl1.OpenGL;
            gl.MatrixMode(OpenGL.GL_PROJECTION);
            gl.LoadIdentity();
            gl.Perspective(100.0f, (double)Width / (double)Height, 0.001, 1000.0);
            gl.MatrixMode(OpenGL.GL_MODELVIEW);
            openglControl1.DoRender();
        }



        private void reset_Click(object sender, EventArgs e)
        {
            if (mracno1load && mracno2load)
            {
                run = false;
                mracno2.Clear();
                mracno2 = NactiPly(cesta2);

                label1.Text = ""; ;
                label11.Text = "";
                label12.Text = "";
                label13.Text = "";
                label14.Text = "";
                label15.Text = "";
                label16.Text = "";
                openglControl1.DoRender();
            }

        }




        private void mračno1ToolStripMenuItem1_Click(object sender, EventArgs e)
        {

            openFileDialog1.Filter = "ply soubory (*.ply)|*.ply";
            openFileDialog1.InitialDirectory = "..\\::";

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                cesta1 = openFileDialog1.FileName;
                nazev1.Text = cesta1.Split("\\").Last();
                mracno1 = NactiPly(cesta1);               
            }
                       
            mracno1load = true;
            Camera();
        }

        private void Camera()
        {
            centr = NajdiTeziste(mracno1);
            r = PolomerMracna(mracno1);
            z =(float)-r*1.2f+(float)centr[2];
           
        }

        private void mračno2ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "ply soubory (*.ply)|*.ply";
            openFileDialog1.InitialDirectory = "::\\";

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {

                cesta2 = openFileDialog1.FileName;
                nazev2.Text = cesta2.Split("\\").Last();
                mracno2 = NactiPly(cesta2);

            }
            mracno2load = true;
        }
        

        public void ICP(List<double[]> mracno1, List<double[]> mracno2p, double E, int maxiterace)
        {
            List<double[]> mracno2 = new List<double[]>();

            foreach (var bod in mracno2p)
            {
                if (run)
                {
                    mracno2.Add(bod);
                }
                else return;
            }

            iterace = 0;
            List<double[,]> R = new List<double[,]>();
            List<double[,]> t = new List<double[,]>();//list všech rotací
            double[] t1 = NajdiTeziste(mracno1);
            double[] t2p = NajdiTeziste(mracno2);
            double[] t2 = t2p;

            List<double[]> nmracno = new List<double[]>();
            List<double[]> nmracno2 = new List<double[]>();

            for (int i = 0; i < mracno1.Count; i++)
            {
                if ((i % vmodulo) == 0)
                {
                    nmracno.Add(mracno1[i]);

                }
            }

            for (int i = 0; i < mracno2.Count; i++)
            {
                if ((i % vmodulo) == 0)
                {

                    nmracno2.Add(mracno2[i]);
                }
            }

            var cmracno1 = CentrMrac(nmracno, t1);

            List<List<double[]>> body = NajdiNejblBody(CentrMrac(nmracno2, t2), cmracno1);

            double[,] t_i = new double[3, 1];

            if (Chyba(body) > E)
            {
                while (Chyba(body) > E && iterace < maxiterace && run)
                {
                    if (iterace != 0)
                    {
                        body = NajdiNejblBody(CentrMrac(nmracno2, t2), cmracno1);
                    }


                    var C = Cmat(body);
                    SingularValueDecomposition svd = new SingularValueDecomposition(C);
                    var r = MatNas(svd.LeftSingularVectors, TranspMat(svd.RightSingularVectors));  //částečná rotace v každé iteraci
                    R.Add(r);
                    var td = MatNas(r, Transp(t2));
                    //částečné posunutí v každé iteraci
                    for (int i = 0; i < t_i.Length; i++)
                    {
                        t_i[i, 0] = t1[i] - td[i, 0];
                    }
                    t.Add(t_i);
                    if (run)
                    {
                        Invoke(new Action(() =>
                        {
                            Transformace(mracno2p, r, t_i);
                            openglControl1.DoRender();
                        }));

                    }
                    Transformace(mracno2, r, t_i);
                    Transformace(nmracno2, r, t_i);
                    t2 = NajdiTeziste(mracno2);

                    iterace++;

                } 

                if (run)
                {
                    MatRot = R[R.Count - 1];
                    transl = t[0];
                    for (int i = R.Count - 2; i >= 0; i--)
                    {
                        MatRot = MatNas(MatRot, R[i]);

                    }

                    for (int i = 1; i < t.Count; i++)
                    {
                        for (int j = 0; i < 3; i++)
                        {
                            transl[j, 0] += t[i][j, 0];
                        }
                    }
                    for (int i = 0; i < 3; i++)
                    {
                        transl[i, 0] -= t2p[i] + t1[i];
                    }
                    double alfa = 0, beta = 0, gama = 0;

                    alfa = Math.Atan2(MatRot[2, 1], MatRot[2, 2]);
                    beta = Math.Atan2(MatRot[2, 0], Math.Sqrt(Math.Pow(MatRot[2, 1], 2) + Math.Pow(MatRot[2, 2], 2)));
                    gama = Math.Atan2(MatRot[1, 0], MatRot[0, 0]);

                    Invoke(new Action(() =>
                    {
                        label1.Text = iterace.ToString();
                        label11.Text = (alfa * (180 / Math.PI)).ToString();
                        label12.Text = (beta * (180 / Math.PI)).ToString();
                        label13.Text = (gama * (180 / Math.PI)).ToString();
                        label14.Text = transl[0, 0].ToString();
                        label15.Text = transl[1, 0].ToString();
                        label16.Text = transl[2, 0].ToString();
                    }));

                }
            }
            else
            {
                Invoke(new Action(() =>
                {
                    label1.Text = "0";
                    label11.Text = "0";
                    label12.Text = "0";
                    label13.Text = "0";                
                   
                    double[] posun = new double[3];
                    double[,] r = new double[3, 3];

                    for (int i = 0; i < 3; i++)
                    {
                         posun[i] = t1[i] - t2[i];
                         r[i, i] = 1;
                    }
                    label14.Text = posun[0].ToString();
                    label15.Text = posun[1].ToString();
                    label16.Text = posun[2].ToString();


                    Transformace(mracno2p, r, Transp(posun));
                        openglControl1.DoRender();
                      
                    
                }));
            }
            run = false;

        }

        public void ICPN(List<double[]> mracno1, List<double[]> mracno2p, double E, int maxiterace)
        {

            List<double[]> mracno2 = new List<double[]>();

            foreach (var bod in mracno2p)
            {
                if (run)
                {
                    mracno2.Add(bod);
                }
                else return;
            }

            iterace = 0;
            double r = PolomerMracna(mracno1);
            double[] t1 = NajdiTeziste(mracno1);
            double[] t2p = NajdiTeziste(mracno2);



            List<double[]> nmracno = new List<double[]>();
            List<double[]> nmracno2 = new List<double[]>();
            for (int i = 0; i < mracno1.Count; i++)
            {
                if ((i % vmodulo) == 0)
                {
                    nmracno.Add(mracno1[i]);

                }
            }

            for (int i = 0; i < mracno2.Count(); i++)
            {
                if ((i % vmodulo) == 0)
                {

                    nmracno2.Add(mracno2[i]);
                }
            }

            List<List<double[]>> normaly = NajdiNormaly(nmracno, nmracno2, r);
            List<List<double[]>> body = NajdiNejblBody(normaly, nmracno2);
            List<double[,]> R = new List<double[,]>();
            List<double[,]> t = new List<double[,]>();
            List<double[,]> Y = new List<double[,]>();

            int n = body.Count;
            double[,] R_i;
            
            if (Chyba(body) > E)
            {

                while (Chyba(body) > E && iterace < maxiterace && run)
                {
                    double[,] b = new double[n, 1];
                    double[,] A = new double[n, 6];
                    if (iterace != 0)
                    {
                        body = NajdiNejblBody(normaly, nmracno2);
                    }
                    for (int i = 0; i < n; i++)
                    {
                        b[i, 0] = body[i][2][0] * body[i][0][0] + body[i][2][1] * body[i][0][1] + body[i][2][2] * body[i][0][2]
                            - body[i][2][0] * body[i][1][0] - body[i][2][1] * body[i][1][1] - body[i][2][2] * body[i][1][2];

                        A[i, 0] = body[i][2][2] * body[i][1][1] - body[i][2][1] * body[i][1][2];
                        A[i, 1] = body[i][2][0] * body[i][1][2] - body[i][2][2] * body[i][1][0];
                        A[i, 2] = body[i][2][1] * body[i][1][0] - body[i][2][0] * body[i][1][1];
                        for (int j = 3; j < 6; j++)
                        {
                            A[i, j] = body[i][2][j - 3];
                        }
                    }
                    SingularValueDecomposition svd = new SingularValueDecomposition(A);
                    double[,] sigma = svd.DiagonalMatrix;

                    for (int i = 0; i < sigma.GetLength(0); i++)
                    {
                        sigma[i, i] = 1 / sigma[i, i];
                    }
                    A = MatNas(MatNas(svd.RightSingularVectors, sigma), TranspMat(svd.LeftSingularVectors));


                    double[,] t_i = new double[3, 1];
                    double[,] y = MatNas(A, b);
                    Y.Add(y);
                    for (int i = 0; i < 3; i++)
                    {
                        t_i[i, 0] = y[i + 3, 0];
                    }
                    t.Add(t_i);
                    R_i = MatRotace(y[0, 0], y[1, 0], y[2, 0]);
                    R.Add(R_i);
                    Transformace(mracno2, R_i, t_i);
                    Transformace(nmracno2, R_i, t_i);
                    if (run)
                    {
                        Invoke(new Action(() =>
                        {
                            Transformace(mracno2p, R_i, t_i);
                            openglControl1.DoRender();
                        }));
                    }

                   iterace++;
                  
                } 

                if (run)
                {

                    double[] t3 = NajdiTeziste(mracno2);
                    double[] tez = new double[3];
                    MatRot = R[R.Count - 1];
                    double[] yps = { Y[0][0, 0], Y[0][1, 0], Y[0][2, 0] };
                    for (int i = 1; i < t.Count; i++)
                    {

                        for (int j = 0; j < 3; j++)
                        {
                            tez[j] = t3[j] - t2p[j];
                            yps[j] += Y[i][j, 0];
                        }

                    }


                    for (int i = R.Count - 2; i >= 0; i--)
                    {
                        MatRot = MatNas(MatRot, R[i]);
                    }

                    transl = t[t.Count - 1];
                    for (int i = t.Count - 2; i >= 0; i--)
                    {
                        double[,] Rt = R[R.Count - 1];
                        for (int j = t.Count - 2; j > i; j--)
                        {

                            Rt = MatNas(Rt, R[j]);

                        }
                        double[,] td = MatNas(Rt, t[i]);
                        for (int j = 0; j < 3; j++)
                        {
                            transl[j, 0] += td[j, 0];
                        }


                    }
                                        
                    Invoke(new Action(() =>
                    {
                        label1.Text = iterace.ToString();
                        label11.Text = (yps[0] * (180 / Math.PI)).ToString();
                        label12.Text = (yps[1] * (180 / Math.PI)).ToString();
                        label13.Text = (yps[2] * (180 / Math.PI)).ToString();
                        label14.Text = tez[0].ToString();
                        label15.Text = tez[1].ToString();
                        label16.Text = tez[2].ToString();
                    }));

                }

            }
            else
            {
                Invoke(new Action(() =>
                {
                    label1.Text = "0";
                    label11.Text = "0";
                    label12.Text = "0";
                    label13.Text = "0";
                    label14.Text = "0";
                    label15.Text = "0";
                    label16.Text = "0";
                }));
            }

            run = false;

        }
       
    }
}
