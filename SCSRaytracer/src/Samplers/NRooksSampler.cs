﻿//    
//    Copyright(C) 2015  Elanna Stephenson
//
//    This software is released under the MIT license, see LICENSE for details.
//    

using System;

namespace RayTracer
{
    sealed class NRooksSampler : Sampler
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
                    Point2D p = new Point2D((j + (float)randomgen.NextDouble()) / numsamples, (j + (float)randomgen.NextDouble()) / numsamples);
                    samples.Add(p);
                }
            }
            //shuffle_x_coords();
            //shuffle_y_coords();
        }

        public override Sampler clone()
        {
            return new NRooksSampler(this);
        }
        /*
        private void shuffle_x_coords()
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
        private void shuffle_y_coords()
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
        */
    }
}
