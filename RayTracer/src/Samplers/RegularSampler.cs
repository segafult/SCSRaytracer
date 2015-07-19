//    
//    Copyright(C) 2015  Elanna Stephenson
//
//    This program is free software: you can redistribute it and/or modify
//    it under the terms of the GNU General Public License as published by
//    the Free Software Foundation, either version 3 of the License, or
//    (at your option) any later version.
//
//    This program is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
//    GNU General Public License for more details.
//
//    You should have received a copy of the GNU General Public License
//    along with this program.If not, see<http://www.gnu.org/licenses/>.
//

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;

namespace RayTracer
{
    /// <summary>
    /// Regularly spaced subpixel sampler for simple antialiasing.
    /// </summary>
    public class RegularSampler : Sampler
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
                        samples.Add(new Point2D((double)k / (double)n, (double)j / (double)n));
                    }
                }
            }
        }

        public override void setup_shuffled_indices()
        {
            throw new NotImplementedException();
        }

        public override void shuffle_samples()
        {
            throw new NotImplementedException();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override Point2D sample_unit_square()
        {
            //Bit masked fast modulus, special case for simple antialiasing
            return samples[(int)(count++ & bitmask)];
        }

        public override Sampler clone()
        {
            return new RegularSampler(this);
        }
    }
}
