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

namespace RayTracer
{
    /// <summary>
    /// Regularly spaced subpixel sampler for simple antialiasing.
    /// </summary>
    public class RegularSampler : Sampler
    {
        public RegularSampler(int s)
        {
            count = 0;
            jump = 0;
            numsamples = s;
            numsets = 1;
            samples = new List<Point2D>();
            shuffledIndices = new List<int>();
        }

        public override void generate_samples()
        {
            int n = (int)Math.Sqrt(numsamples);

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

        public override void setup_shuffled_indices()
        {
            throw new NotImplementedException();
        }

        public override void shuffle_samples()
        {
            throw new NotImplementedException();
        }

        public override Point2D sample_unit_square()
        {
            return samples[(int)(count++ % Convert.ToUInt64(numsamples * numsets))];
        }
    }
}
