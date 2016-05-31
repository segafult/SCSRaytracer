//    
//    Copyright(C) 2015  Elanna Stephenson
//
//    This software is released under the MIT license, see LICENSE for details.
//    

using System;

namespace SCSRaytracer
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

        public override void GenerateSamples()
        {
            int n = (int)Math.Sqrt(_numSamples);
            _bitMask = (ulong)_numSamples - 1;

            for(int setloop = 0; setloop<_numSets; setloop++)
            {
                for(int j = 0; j < n; j++)
                {
                    for(int k = 0; k < n; k++)
                    {
                        _samples.Add(new Point2D((float)k / (float)n, (float)j / (float)n));
                    }
                }
            }
        }

        public override Sampler Clone()
        {
            return new RegularSampler(this);
        }
    }
}
