using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TaiPan.StatsLib
{
    public class StatsLib
    {
        //Box-Muller Transform of gaussian random variable
        public static double[] NormRand(double mean, double stdDev, int number)
        {
            Random rand = new Random();
            double[] ret = new double[number];

            for (int i = 0; i != number; ++i)
            {
                double u1 = rand.NextDouble();
                double u2 = rand.NextDouble();
                double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) *
                             Math.Sin(2.0 * Math.PI * u2); //random normal(0,1)
                double randNormal = mean + stdDev * randStdNormal; //random normal(mean,stdDev^2)
                ret[i] = randNormal;
            }

            return ret;
        }
    }
}
