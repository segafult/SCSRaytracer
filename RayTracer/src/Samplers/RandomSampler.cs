﻿//    
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
    class RandomSampler : Sampler
    {
        Random randomgen;

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
                    samples.Add(new Point2D(randomgen.NextDouble(), randomgen.NextDouble()));
                }
            }
        }

        public override Point2D sample_unit_square()
        {
            return samples[(int)(count++ % (ulong)numsamples)];
        }

        public override void shuffle_samples()
        {
            throw new NotImplementedException();
        }

        public override void setup_shuffled_indices()
        {
            throw new NotImplementedException();
        }

        public override Sampler clone()
        {
            return new RandomSampler(this);
        }
    }
}
