//    
//    Copyright(C) 2015  Elanna Stephenson
//
//    This software is released under the MIT license, see LICENSE for details.
//    

using System;

namespace SCSRaytracer
{
    sealed class JitteredSampler : Sampler
    {

        public JitteredSampler(int s) : base(s)
        {
            randomgen = new Random();
        }
        public JitteredSampler(Sampler clone) : base(clone)
        {
            randomgen = new Random();
        }

        public override void generate_samples()
        {
            //Jittered sampler must be a perfect square
            int n = (int)Math.Sqrt(numsamples);

            for(int s = 0; s < numsets; s++)
            {
                for(int j = 0; j < n; j++)
                {
                    for(int k = 0; k < n; k++)
                    {
                        samples.Add(new Point2D((k + (float)randomgen.NextDouble()) / n, (j + (float)randomgen.NextDouble()) / n));
                    }
                }
            }
        }

        public override Sampler clone()
        {
            return new JitteredSampler(this);
        }
    }
}
