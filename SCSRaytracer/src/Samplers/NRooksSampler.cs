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
    class NRooksSampler : Sampler
    {

        public NRooksSampler(int s) : base(s)
        {
            randomgen = new Random();
            numsets = 16;
        }
        public NRooksSampler(Sampler clone) : base(clone)
        {
            randomgen = new Random();
        }

        public override void generate_samples()
        {
            //Generate samples along diagonal and then shuffle coordinates
            for(int n = 0; n < numsets; n++)
            {
                for(int j = 0; j < numsamples; j++)
                {
                    Point2D p = new Point2D();
                    p.x = (j + (float)randomgen.NextDouble()) / numsamples;
                    p.y = (j + (float)randomgen.NextDouble()) / numsamples;
                    samples.Add(p);
                }
            }
            shuffle_x_coords();
            shuffle_y_coords();
        }

        public override Sampler clone()
        {
            return new NRooksSampler(this);
        }
        protected virtual void shuffle_x_coords()
        {
            float xcoord1;
            float xcoord2;
            int randindex;
            for(int n = 0; n < numsets; n++)
            {
                for(int j = 0; j < numsamples; j++)
                {
                    xcoord1 = samples[(n * numsamples) + j].x;
                    //Generate random index
                    randindex = randomgen.Next(numsamples * numsets);
                    xcoord2 = samples[randindex].x;
                    //Swap the values
                    samples[(n * numsamples) + j].x = xcoord2;
                    samples[randindex].x = xcoord1;
                }
            }
        }
        protected virtual void shuffle_y_coords()
        {
            float ycoord1;
            float ycoord2;
            int randindex;
            for(int n = 0; n < numsets; n++)
            {
                for(int j = 0; j < numsamples; j++)
                {
                    ycoord1 = samples[(n * numsamples) + j].y;
                    //Generate random index
                    randindex = randomgen.Next(numsamples * numsets);
                    ycoord2 = samples[randindex].y;
                    //Swap values
                    samples[(n * numsamples) + j].y = ycoord2;
                    samples[randindex].y = ycoord1;
                }
            }
        }
    }
}
