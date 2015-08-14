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
    class MultiJitteredSampler : Sampler
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
                        samples.Add(new Point2D((k + randomgen.NextDouble()) / n, (j + randomgen.NextDouble()) / n));
                    }
                }
            }
            shuffle_x_coords();
            shuffle_y_coords();
        }

        protected virtual void shuffle_x_coords()
        {
            int n = (int)Math.Sqrt(numsamples);
            double xcoord1;
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
        protected virtual void shuffle_y_coords()
        {
            int n = (int)Math.Sqrt(numsamples);
            double ycoord1;
           
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
        public override Sampler clone()
        {
            return new MultiJitteredSampler(this);
        }
    }
}
