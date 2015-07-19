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
    /// Abstract base class of all samplers for subpixel multisampling.
    /// </summary>
    abstract public class Sampler
    {
        protected int numsamples; //Number of samples per pixel
        protected int numsets; //Number of stored "sample sets"
        protected List<Point2D> samples; //Sample points on unit square;
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
            shuffledIndices = new List<int>();
        }
        protected Sampler(Sampler clone)
        {
            count = 0;
            numsamples = clone.getNumSamples();
            numsets = clone.getNumSets();
            samples = clone.getSamples();
            shuffledIndices = clone.getShuffledIndices();
            bitmask = clone.getBitMask();
            jump = clone.getJump();
        }

        public int getNumSamples() { return numsamples; }
        public int getNumSets() { return numsets; }
        public List<Point2D> getSamples() { return samples; }
        public List<int> getShuffledIndices() { return shuffledIndices; }
        public ulong getBitMask() { return bitmask; }
        public int getJump() { return jump; }

        public abstract Sampler clone();

        public abstract void generate_samples();
        public abstract void setup_shuffled_indices();
        public abstract void shuffle_samples();
        public abstract Point2D sample_unit_square(); //Return next sample point
    }
}
