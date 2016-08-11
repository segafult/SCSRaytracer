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

        public RegularSampler(int numSamples) : base(numSamples)
        {

        }
        public RegularSampler(Sampler clone) : base(clone)
        {

        }

        public override void GenerateSamples()
        {
            int squareSize = (int)Math.Sqrt(_numSamples);
            _bitMask = (ulong)_numSamples - 1;

            for(int setloop = 0; setloop<_numSets; setloop++)
            {
                for(int j = 0; j < squareSize; j++)
                {
                    for(int k = 0; k < squareSize; k++)
                    {
                        _samples.Add(new Point2D((float)k / (float)squareSize, (float)j / (float)squareSize));
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
