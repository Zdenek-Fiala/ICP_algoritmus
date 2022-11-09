using Accord.Math.Decompositions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using static Zobrazovani.Matice;
using static Zobrazovani.Mracna;

namespace Zobrazovani
{

    class Normala
    {
        double[] vektor;
        public double[] normala { get { return vektor; } }


        public Normala(double[] bod, List<double[]> mracno, double r)
        {
            vektor = NajdiNormalu(bod, mracno, r);
        }

        private double[] NajdiNormalu(double[] bod, List<double[]> mracno, double r)
        {

            double[,] C = new double[3, 3];

            List<double[]> OkolniBody = NajdiOkolniBody(bod, mracno, r);


            double[,] c = new double[3, 3];

            foreach (var obod in OkolniBody)
            {
                double[] cbod = new double[3];
                for (int i = 0; i < 3; i++)
                {
                    cbod[i] = obod[i] - bod[i];
                }
                c = MatNas(Transp(cbod), cbod);
                C = Secti(C, c);
            }
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    C[i, j] = C[i, j] / 4;
                }
            }
            SingularValueDecomposition svd = new SingularValueDecomposition(C);
            double[,] U = svd.LeftSingularVectors;
            double[] n = new double[4];

            for (int i = 0; i < 3; i++)
            {
                n[i] = U[i, 2];
            }
            return n;
        }


        private List<double[]> NajdiOkolniBody(double[] refbod, List<double[]> mracno, double r)
        {
            List<double[]> OkolniBody = new List<double[]>();
            double polom = r / 100;
           
            while (OkolniBody.Count < 4)
            {

                foreach (var bod in mracno)
                {          

                    if (!OkolniBody.Contains(bod) && refbod != bod && polom >= Vzdalenost(refbod, bod))
                    {
                        OkolniBody.Add(bod);

                    }
                }

                polom += r / 100;
            }
            if (OkolniBody.Count > 4)
            {
                while (OkolniBody.Count != 4)
                {
                    double maxvzdalenost = 0;
                    double[] nejvzdalenejsiBod = new double[3];
                    foreach (var bod in OkolniBody)
                    {

                        if (Vzdalenost(refbod, bod) > maxvzdalenost)
                        {

                            maxvzdalenost = Vzdalenost(refbod, bod);
                            nejvzdalenejsiBod = bod;
                        }
                    }
                    OkolniBody.Remove(nejvzdalenejsiBod);
                }
            }
            return OkolniBody;
        }

    }

}
