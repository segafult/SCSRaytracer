//    
//    Copyright(C) 2015  Elanna Stephenson
//
//    This software is released under the MIT license, see LICENSE for details.
//    

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace RayTracer
{
    /// <summary>
    /// Abstract base class of all samplers for subpixel multisampling.
    /// </summary>
    abstract class Sampler
    {
        protected int numsamples; //Number of samples per pixel
        protected int numsets; //Number of stored "sample sets"
        protected List<Point2D> samples; //Sample points on unit square;
        protected List<Point2D> disk_samples; //Sample points on disk
        protected List<Point2D> sphere_samples; //Sample points on sphere
        protected List<int> shuffledIndices; //Shuffled samples array indices
        protected ulong count; //Current number of sample points
        protected ulong bitmask; //Bit mask 
        protected int jump; //Random index jump
        protected Random randomgen;

        protected Sampler(int s)
        {
            count = 0;
            jump = 0;
            numsamples = s;
            numsets = 25;
            samples = new List<Point2D>();
            disk_samples = new List<Point2D>();
            sphere_samples = new List<Point2D>();
            shuffledIndices = new List<int>();
            randomgen = new Random();
        }
        protected Sampler(Sampler clone)
        {
            count = 0;
            numsamples = clone.getNumSamples();
            numsets = clone.getNumSets();
            samples = clone.getSamples();
            disk_samples = clone.getDiskSamples();
            sphere_samples = clone.getSphereSamples();
            shuffledIndices = clone.getShuffledIndices();
            jump = clone.getJump();
            randomgen = new Random();
        }

        public int getNumSamples() { return numsamples; }
        public int getNumSets() { return numsets; }
        public List<Point2D> getSamples() { return samples; }
        public List<Point2D> getDiskSamples() { return disk_samples; }
        public List<Point2D> getSphereSamples() { return sphere_samples; }
        public List<int> getShuffledIndices() { return shuffledIndices; }
        public ulong getBitMask() { return bitmask; }
        public int getJump() { return jump; }

        public abstract Sampler clone();

        public virtual void generate_samples()
        {
            //do nothing
        }
        public virtual void map_samples_to_disk()
        {
            for(int i = 0; i < (numsamples*numsets); i++)
            {
                disk_samples.Add(new Point2D());

                float r;
                float phi;

                //Determine the quadrant for which to map according to the Shirley concentric map
                //Quadrant 1 and 2:
                if(samples[i].x > -samples[i].y)
                {
                    //Conditions for quadrant 1
                    if(samples[i].x > samples[i].y)
                    {
                        r = samples[i].x;
                        phi = (samples[i].y / samples[i].x) * ((float)Math.PI / 4.0f);
                    }
                    //Otherwise in quadrant 2
                    else
                    {
                        r = samples[i].y;
                        phi = (samples[i].y != 0.0f) ? (2 - samples[i].x / samples[i].y) * ((float)Math.PI / 4.0f) : 0.0f; //Don't divide by zero
                    }
                }
                //Quadrant 3 and 4
                else
                {
                    //Conditions for quadrant 3
                    if(samples[i].x < samples[i].y)
                    {
                        r = -samples[i].x;
                        phi = (samples[i].x != 0.0f) ? (4 + samples[i].y / samples[i].x) * ((float)Math.PI / 4.0f) : 0.0f; //Don't divide by zero
                    }
                    //Otherwise in quadrant 4
                    else
                    {
                        r = -samples[i].y;
                        phi = (samples[i].y != 0.0f) ? (6 - samples[i].x / samples[i].y) * ((float)Math.PI / 4.0f) : 0.0f; //Don't divide by zero
                    }
                }

                disk_samples[i].x = r * (float)Math.Cos(phi);
                disk_samples[i].y = r * (float)Math.Sin(phi);
            }
        }
        public virtual void setup_shuffled_indices()
        {
            List<int> indices = new List<int>();

            for (int i = 0; i < numsamples; i++)
                indices.Add(i);
            for(int i = 0; i < numsets; i++)
            {
                shuffle_samples(ref indices);

                for (int j = 0; j < numsamples; j++)
                {
                    shuffledIndices.Add(indices[j]);
                }
            }
        }
        public virtual void shuffle_samples(ref List<int> index)
        {
            //shuffle the index list using the Fisher yates shuffling algorithm
            for(int i = index.Count - 1; i > 1; i--)
            {
                int swapindex = randomgen.Next(0,i);
                int tmp = index[i];
                index[i] = index[swapindex];
                index[swapindex] = tmp;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual Point2D sample_unit_square()
        {
            if (count % (ulong)numsamples == 0)
                jump = (randomgen.Next() % numsets);
            return (samples[jump + shuffledIndices[(int)((ulong)jump + count++ % (ulong)(numsamples))]]);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual Point2D sample_disk()
        {
            if(count%(ulong)numsamples == 0)
                jump = (randomgen.Next() % numsets);
            return (disk_samples[jump + shuffledIndices[(int)((ulong)jump + count++ % (ulong)(numsamples))]]);
        }

        public static Sampler LoadSampler(string str_sampler)
        {
            Sampler toReturn;
            //Determine sampler type and load accordingly.
            if (str_sampler.Equals("regular"))
                toReturn = new RegularSampler(GlobalVars.num_samples);
            else if (str_sampler.Equals("random"))
                toReturn = new RandomSampler(GlobalVars.num_samples);
            else if (str_sampler.Equals("jittered"))
                toReturn = new JitteredSampler(GlobalVars.num_samples);
            else if (str_sampler.Equals("nrooks"))
                toReturn = new NRooksSampler(GlobalVars.num_samples);
            else if (str_sampler.Equals("multijittered"))
                toReturn = new MultiJitteredSampler(GlobalVars.num_samples);
            else
            {
                Console.WriteLine("Unknown sampler type: " + str_sampler);
                toReturn = new RegularSampler(GlobalVars.num_samples);
            }

            toReturn.generate_samples();
            toReturn.setup_shuffled_indices();
            return toReturn;
        }
    }
}
