//    
//    Copyright(C) 2015  Elanna Stephenson
//
//    This software is released under the MIT license, see LICENSE for details.
//    

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace SCSRaytracer
{
    /// <summary>
    /// Abstract base class of all samplers for subpixel multisampling.
    /// </summary>
    abstract class Sampler
    {
        protected int _numSamples; //Number of samples per pixel
        protected int _numSets; //Number of stored "sample sets"
        protected List<Point2D> _samples; //Sample points on unit square;
        protected List<Point2D> _diskSamples; //Sample points on disk
        protected List<Point2D> _sphereSamples; //Sample points on sphere
        protected List<int> _shuffledIndices; //Shuffled samples array indices
        protected ulong count; //Current number of sample points
        protected ulong _bitMask; //Bit mask 
        protected int _jump; //Random index jump
        protected Random randomgen;

        // accessors
        public int NumSamples { get { return _numSamples; } }
        public int NumSets { get { return _numSets; } }
        public List<Point2D> Samples { get { return _samples; } }
        public List<Point2D> DiskSamples { get { return _diskSamples; } }
        public List<Point2D> SphereSamples { get { return _sphereSamples; } }
        public List<int> ShuffledIndices { get { return _shuffledIndices; } }
        public ulong BitMask { get { return _bitMask; } }
        public int Jump { get { return _jump; } }


        protected Sampler(int numSamples)
        {
            count = 0;
            _jump = 0;
            _numSamples = numSamples;
            _numSets = 25;
            _samples = new List<Point2D>();
            _diskSamples = new List<Point2D>();
            _sphereSamples = new List<Point2D>();
            _shuffledIndices = new List<int>();
            randomgen = new Random();
        }
        protected Sampler(Sampler clone)
        {
            count = 0;
            _numSamples = clone.NumSamples;
            _numSets = clone.NumSets;
            _samples = clone.Samples;
            _diskSamples = clone.DiskSamples;
            _sphereSamples = clone.SphereSamples;
            _shuffledIndices = clone.ShuffledIndices;
            _jump = clone.Jump;
            randomgen = new Random();
        }

        public abstract Sampler Clone();

        public virtual void GenerateSamples()
        {
            //do nothing
        }
        public virtual void MapSamplesToDisk()
        {
            for(int i = 0; i < (_numSamples*_numSets); i++)
            {
                _diskSamples.Add(new Point2D());

                float r;
                float phi;

                //Determine the quadrant for which to map according to the Shirley concentric map
                //Quadrant 1 and 2:
                if(_samples[i].coords.X > -_samples[i].coords.Y)
                {
                    //Conditions for quadrant 1
                    if(_samples[i].coords.X > _samples[i].coords.Y)
                    {
                        r = _samples[i].coords.X;
                        phi = (_samples[i].coords.Y / _samples[i].coords.X) * ((float)Math.PI / 4.0f);
                    }
                    //Otherwise in quadrant 2
                    else
                    {
                        r = _samples[i].coords.Y;
                        phi = (_samples[i].coords.Y != 0.0f) ? (2 - _samples[i].coords.X / _samples[i].coords.Y) * ((float)Math.PI / 4.0f) : 0.0f; //Don't divide by zero
                    }
                }
                //Quadrant 3 and 4
                else
                {
                    //Conditions for quadrant 3
                    if(_samples[i].coords.X < _samples[i].coords.Y)
                    {
                        r = -_samples[i].coords.X;
                        phi = (_samples[i].coords.X != 0.0f) ? (4 + _samples[i].coords.Y / _samples[i].coords.X) * ((float)Math.PI / 4.0f) : 0.0f; //Don't divide by zero
                    }
                    //Otherwise in quadrant 4
                    else
                    {
                        r = -_samples[i].coords.Y;
                        phi = (_samples[i].coords.Y != 0.0f) ? (6 - _samples[i].coords.X / _samples[i].coords.Y) * ((float)Math.PI / 4.0f) : 0.0f; //Don't divide by zero
                    }
                }

                _diskSamples[i] = new Point2D(r * (float)Math.Cos(phi), r * (float)Math.Sin(phi));
            }
        }
        public virtual void SetupShuffledIndices()
        {
            List<int> indices = new List<int>();

            for (int i = 0; i < _numSamples; i++)
                indices.Add(i);
            for(int i = 0; i < _numSets; i++)
            {
                ShuffleSamples(ref indices);

                for (int j = 0; j < _numSamples; j++)
                {
                    _shuffledIndices.Add(indices[j]);
                }
            }
        }
        public virtual void ShuffleSamples(ref List<int> index)
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
        public virtual Point2D SampleUnitSquare()
        {
            if (count % (ulong)_numSamples == 0)
                _jump = (randomgen.Next() % _numSets);
            return (_samples[_jump + _shuffledIndices[(int)((ulong)_jump + count++ % (ulong)(_numSamples))]]);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual Point2D SampleDisk()
        {
            if(count%(ulong)_numSamples == 0)
                _jump = (randomgen.Next() % _numSets);
            return (_diskSamples[_jump + _shuffledIndices[(int)((ulong)_jump + count++ % (ulong)(_numSamples))]]);
        }

        public static Sampler LoadSampler(string str_sampler)
        {
            Sampler toReturn;
            //Determine sampler type and load accordingly.
            if (str_sampler.Equals("regular"))
                toReturn = new RegularSampler(GlobalVars.NUM_SAMPLES);
            else if (str_sampler.Equals("random"))
                toReturn = new RandomSampler(GlobalVars.NUM_SAMPLES);
            else if (str_sampler.Equals("jittered"))
                toReturn = new JitteredSampler(GlobalVars.NUM_SAMPLES);
            else if (str_sampler.Equals("nrooks"))
                toReturn = new NRooksSampler(GlobalVars.NUM_SAMPLES);
            else if (str_sampler.Equals("multijittered"))
                toReturn = new MultiJitteredSampler(GlobalVars.NUM_SAMPLES);
            else
            {
                Console.WriteLine("Unknown sampler type: " + str_sampler);
                toReturn = new RegularSampler(GlobalVars.NUM_SAMPLES);
            }

            toReturn.GenerateSamples();
            toReturn.SetupShuffledIndices();
            return toReturn;
        }
    }
}
