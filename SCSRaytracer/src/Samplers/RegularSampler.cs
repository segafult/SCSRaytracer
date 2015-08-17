//    
//    Copyright(C) 2015  Elanna Stephenson
//
//    This software is released under the MIT license, see LICENSE for details.
//    

using System;

namespace RayTracer
{
    /// <summary>
    /// Regularly spaced subpixel sampler for simple antialiasing.
    /// </summary>
    sealed class RegularSampler : Sampler
    {

        public RegularSampler(int s) : base(s)
        {

        }
        public RegularSampler(Sampler clone) : base(clone)
        {

        }

        public override void generate_samples()
        {
            int n = (int)Math.Sqrt(numsamples);
            bitmask = (ulong)numsamples - 1;

            for(int setloop = 0; setloop<numsets; setloop++)
            {
                for(int j = 0; j < n; j++)
                {
                    for(int k = 0; k < n; k++)
                    {
                        samples.Add(new Point2D((float)k / (float)n, (float)j / (float)n));
                    }
                }
            }
        }

        public override Sampler clone()
        {
            return new RegularSampler(this);
        }
    }
}
