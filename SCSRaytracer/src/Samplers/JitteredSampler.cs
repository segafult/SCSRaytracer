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

        public JitteredSampler(int numSamples) : base(numSamples)
        {
            randomgen = new Random();
        }
        public JitteredSampler(Sampler clone) : base(clone)
        {
            randomgen = new Random();
        }

        public override void GenerateSamples()
        {
            //Jittered sampler must be a perfect square
            int n = (int)Math.Sqrt(_numSamples);

            for(int s = 0; s < _numSets; s++)
            {
                for(int j = 0; j < n; j++)
                {
                    for(int k = 0; k < n; k++)
                    {
                        _samples.Add(new Point2D((k + (float)randomgen.NextDouble()) / n, (j + (float)randomgen.NextDouble()) / n));
                    }
                }
            }
        }

        public override Sampler Clone()
        {
            return new JitteredSampler(this);
        }
    }
}
