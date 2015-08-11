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

namespace RayTracer
{
    /// <summary>
    /// Abstract base class of all samplers for subpixel multisampling.
    /// </summary>
    abstract public class Sampler
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

        protected Sampler(int s)
        {
            count = 0;
            jump = 0;
            numsamples = s;
            numsets = 1;
            samples = new List<Point2D>();
            disk_samples = new List<Point2D>();
            sphere_samples = new List<Point2D>();
            shuffledIndices = new List<int>();
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
            bitmask = clone.getBitMask();
            jump = clone.getJump();
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

                double r;
                double phi;

                //Determine the quadrant for which to map according to the Shirley concentric map
                //Quadrant 1 and 2:
                if(samples[i].x > -samples[i].y)
                {
                    //Conditions for quadrant 1
                    if(samples[i].x > samples[i].y)
                    {
                        r = samples[i].x;
                        phi = (samples[i].y / samples[i].x) * (Math.PI / 4.0);
                    }
                    //Otherwise in quadrant 2
                    else
                    {
                        r = samples[i].y;
                        phi = (samples[i].y != 0.0) ? (2 - samples[i].x / samples[i].y) * (Math.PI / 4.0) : 0.0; //Don't divide by zero
                    }
                }
                //Quadrant 3 and 4
                else
                {
                    //Conditions for quadrant 3
                    if(samples[i].x < samples[i].y)
                    {
                        r = -samples[i].x;
                        phi = (samples[i].x != 0.0) ? (4 + samples[i].y / samples[i].x) * (Math.PI / 4.0) : 0.0; //Don't divide by zero
                    }
                    //Otherwise in quadrant 4
                    else
                    {
                        r = -samples[i].y;
                        phi = (samples[i].y != 0.0) ? (6 - samples[i].x / samples[i].y) * (Math.PI / 4.0) : 0.0; //Don't divide by zero
                    }
                }

                disk_samples[i].x = r * Math.Cos(phi);
                disk_samples[i].y = r * Math.Sin(phi);
            }
        }
        public virtual void setup_shuffled_indices()
        {
            //do nothing
        }
        public virtual void shuffle_samples()
        {
            //do nothing
        }
        public virtual Point2D sample_unit_square()
        {
            return (samples[(int)(count++ % (ulong)(numsamples * numsets))]);        
        }
        public virtual Point2D sample_disk()
        {
            return (disk_samples[(int)(count++ % (ulong)(numsamples * numsets))]);
        }
    }
}
