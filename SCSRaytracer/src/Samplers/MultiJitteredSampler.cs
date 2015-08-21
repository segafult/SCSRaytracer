//    
//    Copyright(C) 2015  Elanna Stephenson
//
//    This software is released under the MIT license, see LICENSE for details.
//    

using System;

namespace RayTracer
{
    sealed class MultiJitteredSampler : Sampler
    {

        public MultiJitteredSampler(int s) : base(s)
        {
            randomgen = new Random();
        }
        public MultiJitteredSampler(Sampler clone) : base(clone)
        {
            randomgen = new Random();
        }

        public override void generate_samples()
        {
            //Jittered sampler must be a perfect square
            int n = (int)Math.Sqrt(numsamples);

            for (int s = 0; s < numsets; s++)
            {
                for (int j = 0; j < n; j++)
                {
                    for (int k = 0; k < n; k++)
                    {
                        samples.Add(new Point2D((k + (float)randomgen.NextDouble()) / n, (j + (float)randomgen.NextDouble()) / n));
                    }
                }
            }
            //shuffle_x_coords();
            //shuffle_y_coords();
        }

        /*
        private void shuffle_x_coords()
        {
            int n = (int)Math.Sqrt(numsamples);
            float xcoord1;
            //Perform the same swap for each column
            for (int ns = 0; ns < numsets; ns++)
            {
                for(int k = 0; k < n; k++)
                {
                    int offset = randomgen.Next(n);
                    for(int j = 0; j < n; j++)
                    {
                        xcoord1 = samples[(ns * numsamples) + (j * n) + (k)].x;
                        samples[(ns * numsamples) + (j * n) + (k)].x = samples[(ns * numsamples) + (((j + offset) % n) * n) + k].x;
                        samples[(ns * numsamples) + (((j + offset) % n) * n) + k].x = xcoord1;
                    }
                }
            }
        }
        private void shuffle_y_coords()
        {
            int n = (int)Math.Sqrt(numsamples);
            float ycoord1;
           
            //Perform the same swap for each row
            for (int ns = 0; ns < numsets; ns++)
            {
                for (int j = 0; j < n; j++)
                {
                    int offset = randomgen.Next(n);
                    for(int k = 0; k < n; k++)
                    {
                        ycoord1 = samples[(ns * numsamples) + (j * n) + k].y;
                        samples[(ns * numsamples) + (j * n) + k].y = samples[(ns * numsamples) + (j * n) + ((k + offset) % n)].y;
                        samples[(ns * numsamples) + (j * n) + ((k + offset) % n)].y = ycoord1;
                    }
                }
            }
        }
        */
        public override Sampler clone()
        {
            return new MultiJitteredSampler(this);
        }
    }
}
