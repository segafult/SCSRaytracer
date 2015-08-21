//    
//    Copyright(C) 2015  Elanna Stephenson
//
//    This software is released under the MIT license, see LICENSE for details.
//    

using System;

namespace SCSRaytracer
{
    sealed class RandomSampler : Sampler
    {

        public RandomSampler(int s) : base(s)
        {
            randomgen = new Random();
        }
        public RandomSampler(Sampler clone) : base(clone)
        {
            randomgen = new Random();
        }
        public override void generate_samples()
        {
            //Generate random samples
            for (int s = 0; s < numsets; s++)
            {
                for (int i = 0; i < numsamples; i++)
                {
                    samples.Add(new Point2D((float)randomgen.NextDouble(), (float)randomgen.NextDouble()));
                }
            }
        }

        public override Sampler clone()
        {
            return new RandomSampler(this);
        }
    }
}
