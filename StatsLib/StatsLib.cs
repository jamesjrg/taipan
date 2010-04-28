using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TaiPan.StatsLib
{
    public class StatsLib
    {
        //static member, we want to reuse it to get better random numbers (and if we didn't, we'd have to reseed
        //it manually with each library call)
        Random rand = new Random();

        //Box-Muller Transform of gaussian random variable
        public double[] NormStdRand(int number)
        {
            double[] ret = new double[number];

            //note indices
            for (int i = 0; i < number; i += 2)
            {
                double u1 = rand.NextDouble();
                double u2 = rand.NextDouble();
                double randStdNormal1 = Math.Sqrt(-2.0 * Math.Log(u1)) *
                        Math.Sin(2.0 * Math.PI * u2); //random normal(0,1)
                ret[i] = randStdNormal1;

                if (i + 1 < number)
                {
                    double randStdNormal2 = Math.Sqrt(-2.0 * Math.Log(u1)) *
                            Math.Cos(2.0 * Math.PI * u2); //random normal(0,1)                
                    ret[i + 1] = randStdNormal2;
                }
            }

            return ret;
        }

        public double[] NormRand(double mean, double stdDev, int number)
        {
            double[]  values = NormStdRand(number);

            for (int i = 0; i != number; ++i)
                values[i] = mean + stdDev * values[i];

            return values;
        }

        //Geometric Brownian Motion
        //won't allow values to go beneath 0
        //http://en.wikipedia.org/wiki/Geometric_Brownian_motion
        //http://en.wikipedia.org/wiki/Wiener_process	
        //http://www.scipy.org/Cookbook/BrownianMotion
        public decimal[] GBMSequence(decimal currentVal, decimal tickVolatility, int number)
        {
            decimal[] ret = new decimal[number];
            double[] sample = NormStdRand(number);

            double lastSample = 0;

            for (int i = 0; i != number; ++i)
            {
                decimal changeInWiener = (decimal)sample[i] - (decimal)lastSample;

                decimal priceChange = tickVolatility * currentVal * changeInWiener;
                decimal newVal = currentVal + priceChange;
                if (newVal > 0)
                    ret[i] = newVal;
                else
                    ret[i] = 0;

                lastSample = sample[i];
            }

            return ret;
        }
    }
}
