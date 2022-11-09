using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using static Zobrazovani.Matice;

namespace Zobrazovani
{
    public class Mracna
    {

        public static int iterace;
        public static int pocet;
        public static double[] NajdiTeziste(List<double[]> mracno)
        {
            double tezx = 0, tezy = 0, tezz = 0;
            foreach (var bod in mracno)
            {
                tezx += bod[0];
                tezy += bod[1];
                tezz += bod[2];
            }

            double[] teziste = { tezx / mracno.Count, tezy / mracno.Count, tezz / mracno.Count };
            return teziste;

        }

        public static double Chyba(List<List<double[]>> body)
        {
            double vzdalenost = 0;
            foreach (var bod in body)
            {
                vzdalenost += Vzdalenost(bod[0], bod[1]);
            }
            
            return vzdalenost / body.Count;
        }

        private static double[] NejblBod(double[] refbod, List<double[]> mracno)
        {
            double[] nejblbod = null;
            double vzd;
            double minvzd = Double.MaxValue;
            foreach (double[] bod in mracno)
            {
                vzd = Vzdalenost(refbod, bod);
                if (vzd < minvzd)
                {
                    minvzd = vzd;
                    nejblbod = bod;
                }
            }
            return nejblbod;
        }

        public static double Vzdalenost(double[] bod1, double[] bod2)
        {
            return Math.Sqrt(Math.Pow(bod1[0] - bod2[0], 2) + Math.Pow(bod1[1] - bod2[1], 2) + Math.Pow(bod1[2] - bod2[2], 2));
        }

        public static List<double[]> CentrMrac(List<double[]> mracno, double[] teziste)
        {
            List<double[]> centr = new List<double[]>();

            foreach (var bod in mracno)
            {
                double[] bodd = new double[3];
                for (int i = 0; i < 3; i++)
                {
                    bodd[i] = bod[i] - teziste[i];
                }
                centr.Add(bodd);
            }

            return centr;
        }

        public static void Transformace(List<double[]> mracno, double[,] R, double[,] t)
        {
            double[,] matTrans = MatTransf(R, t);
            for (int i = 0; i < mracno.Count; i++)
            {
                double[,] b = { { mracno[i][0] }, { mracno[i][1] }, { mracno[i][2] }, { 1 } };
                double[] a = Transp(MatNas(matTrans, b));
                double[] aa = { a[0], a[1], a[2] };
                mracno[i] = aa;

            }


        }

        public static List<double[]> NTransformace(List<double[]> mracno, double[,] R, double[,] t)
        {

            double[,] matTrans = MatTransf(R, t);
            List<double[]> vmracno = new List<double[]>();
            for (int i = 0; i < mracno.Count; i++)
            {
                double[,] b = { { mracno[i][0] }, { mracno[i][1] }, { mracno[i][2] }, { 1 } };
                double[] a = Transp(MatNas(matTrans, b));
                double[] aa = { a[0], a[1], a[2] };
                vmracno.Add(aa);

            }

            return vmracno;
        }

        public static List<List<double[]>> NajdiNejblBody(List<List<double[]>> normaly, List<double[]> mracno2)
        {
            List<List<double[]>> Body = new List<List<double[]>>();
            Object lockMe = new Object();

            Parallel.ForEach(normaly, bod =>
            {
                List<double[]> body = new List<double[]>();
                NejblizsiBod nejbod = new NejblizsiBod(bod[0], mracno2);

                double[] nbod = new double[4];
                double[] nnejbod = new double[4];

                nbod[3] = 1;
                nnejbod[3] = 1;

                for (int i = 0; i < 3; i++)
                {
                    nbod[i] = bod[0][i];
                    nnejbod[i] = nejbod.Nejblizsibod[i];

                }
                body.Add(nbod);
                body.Add(nnejbod);
                body.Add(bod[1]);
                lock (lockMe)
                {
                    Body.Add(body);
                }

            }
            );

            return Body;
        }

        public static List<List<double[]>> NajdiNormaly(List<double[]> mracno1, List<double[]> mracno2, double r)
        {
            List<List<double[]>> Body = new List<List<double[]>>();
            Object lockMe = new Object();

            Parallel.ForEach(mracno1, bod =>
            {
                List<double[]> body = new List<double[]>();
                Normala normala = new Normala(bod, mracno1, r);
                
                body.Add(bod);
                body.Add(normala.normala);
                lock (lockMe)
                {
                    Body.Add(body);
                }

            }
            );

            return Body;
        }

        public static List<List<double[]>> NajdiNejblBody(List<double[]> mracno1, List<double[]> mracno2)
        {
            List<List<double[]>> Body = new List<List<double[]>>();
            Object lockMe = new Object();
            
            Parallel.ForEach(mracno1, bod =>
            {
                List<double[]> body = new List<double[]>();
                NejblizsiBod nejbod = new NejblizsiBod(bod, mracno2);
                body.Add(bod);
                body.Add(nejbod.Nejblizsibod);

                lock (lockMe)
                {
                    Body.Add(body);
                }

            }
            );

            return Body;
        }
        public static double PolomerMracna(List<double[]> mracno)
        {
            double[] t = NajdiTeziste(mracno);
            double maxVzdalenost = 0;
            foreach (var bod in mracno)

            {

                if (Vzdalenost(t, bod) > maxVzdalenost)
                {
                    maxVzdalenost = Vzdalenost(t, bod);
                }
            }

            return maxVzdalenost;

        }
    }
}
