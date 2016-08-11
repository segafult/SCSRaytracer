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

        public RandomSampler(int numSamples) : base(numSamples)
        {
            randomgen = new Random();
        }
        public RandomSampler(Sampler clone) : base(clone)
        {
            randomgen = new Random();
        }
        public override void GenerateSamples()
        {
            //Generate random samples
            for (int s = 0; s < _numSets; s++)
            {
                for (int i = 0; i < _numSamples; i++)
                {
                    _samples.Add(new Point2D((float)randomgen.NextDouble(), (float)randomgen.NextDouble()));
                }
            }
        }

        public override Sampler Clone()
        {
            return new RandomSampler(this);
        }
    }
}
