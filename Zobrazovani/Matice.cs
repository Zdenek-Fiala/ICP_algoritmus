using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Accord.Math.Decompositions;
using SharpGL;

namespace Zobrazovani
{
    public class Matice
    {    
   
        public static double[,] MatRotace(double a, double b, double g)
        {
            double[,] matTrans = new double[3, 3];

            matTrans[0, 0] = Math.Cos(g) * Math.Cos(b);
            matTrans[0, 1] = -Math.Sin(g) * Math.Cos(a) + Math.Cos(g) * Math.Sin(b) * Math.Sin(a);
            matTrans[0, 2] = Math.Sin(g) * Math.Sin(a) + Math.Cos(g) * Math.Sin(b) * Math.Cos(a);
            matTrans[1, 0] = Math.Sin(g) * Math.Cos(b);
            matTrans[1, 1] = Math.Cos(g) * Math.Cos(a) + Math.Sin(g) * Math.Sin(b) * Math.Sin(a);
            matTrans[1, 2] = -Math.Cos(g) * Math.Sin(a) + Math.Sin(g) * Math.Sin(b) * Math.Cos(a);
            matTrans[2, 0] = -Math.Sin(b);
            matTrans[2, 1] = Math.Cos(b) * Math.Sin(a);
            matTrans[2, 2] = Math.Cos(b) * Math.Cos(a);
            return matTrans;
        }

        public static double[,] MatNas(double[,] A, double[,] B)
        {
            if (A.GetLength(1) == B.GetLength(0))
            {
                double[,] C = new double[A.GetLength(0), B.GetLength(1)];
                for (int i = 0; i < A.GetLength(0); i++)
                {
                    for (int j = 0; j < B.GetLength(1); j++)
                    {

                        for (int k = 0; k < B.GetLength(0); k++)
                        {
                            C[i, j] += A[i, k] * B[k, j];
                        }

                    }
                }

                return C;
            }
            else
            {
                Console.WriteLine("Nelze nasobit");
                return null;
            }

        }

        public static double[,] MatNas(double[,] A, double[] B)
        {
            double[,] B1 = new double[1, B.Length];
            for (int i = 0; i < B.Length; i++)
            {
                B1[0, i] = B[i];
            }
            var C = MatNas(A, B1);
            return C;

        }
      
        public static double[,] Secti(double[,] A, double[,] B)
        {
            for (int i = 0; i < A.GetLength(0); i++)
            {
                for (int j = 0; j < A.GetLength(1); j++)
                {
                    A[i, j] += B[i, j];
                }
            }
            return A;
        }
       
        public static double[,] Cmat(List<List<double[]>> nejbody)
        {

            double[,] C = new double[3, 3];

            foreach (var body in nejbody)
            {
                C = Secti(C, MatNas(Transp(body[1]), body[0]));
            }
            for (int j = 0; j < C.GetLength(0); j++)
            {
                for (int k = 0; k < C.GetLength(1); k++)
                {
                    C[j, k] = C[j, k] / nejbody.Count;
                }
            }
            return C;
        }

        public static double[,] TranspMat(double[,] matice)
        {

            double[,] nmatice = new double[matice.GetLength(1), matice.GetLength(0)];
            for (int i = 0; i < matice.GetLength(1); i++)
            {
                for (int j = 0; j < matice.GetLength(0); j++)
                {
                    nmatice[i, j] = matice[j, i];
                }
            }
            return nmatice;

        }
        public static double[,] Transp(double[] matice)
        {

            double[,] nmatice = new double[matice.GetLength(0), 1];
            for (int i = 0; i < matice.GetLength(0); i++)
            {
                nmatice[i, 0] = matice[i];
            }
            return nmatice;
        }

        public static double[] Transp(double[,] matice)
        {

            double[] nmatice = new double[matice.GetLength(0)];
            for (int i = 0; i < matice.GetLength(0); i++)
            {
                nmatice[i] = matice[i, 0];
            }
            return nmatice;
        }
        public static double[,] MatTransf(double[,] rotace, double[,] translace)
        {
            double[,] matice = new double[4, 4];
            matice[3, 3] = 1;
            for (int i = 0; i < 3; i++)
            {
                matice[i, 3] = translace[i, 0];
                for (int j = 0; j < 3; j++)
                {
                    matice[i, j] = rotace[i, j];
                }
            }
            return matice;
        }

    }


}


