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
        protected int jump; //Random index jump

        public abstract void generate_samples();
        public abstract void setup_shuffled_indices();
        public abstract void shuffle_samples();
        public abstract Point2D sample_unit_square(); //Return next sample point
    }
}
