using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Zobrazovani
{
    class NejblizsiBod
    { 
        public double[] Nejblizsibod {get { return nejblizsibod; } }      
        private double[] nejblizsibod = new double[3];

        public NejblizsiBod(double[] bod, List<double[]> mracno) 
        {
                        
            nejblizsibod = NejblBod(bod, mracno);

        }
        private double[] NejblBod(double[] refbod, List<double[]> mracno)
        {
            double[] nejblbod = null;
            double vzd;
            double minvzd = Double.MaxValue;
            
            Parallel.ForEach(mracno,bod =>
            {
                vzd = Vzdalenost(refbod, bod);
                
                if (vzd < minvzd)
                {
                    minvzd = vzd;
                    nejblbod = bod;
                }
            });

            return nejblbod;

        }

        private static double Vzdalenost(double[] bod1, double[] bod2)
        {
            double vzdalenost = 0;
            for (int i = 0; i < bod1.Length; i++)
            {
                vzdalenost += Math.Pow(bod1[i] - bod2[i], 2);
            }
            vzdalenost = Math.Sqrt(vzdalenost);
            return vzdalenost;
        }

    }
}
