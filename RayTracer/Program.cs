using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;

namespace RayTracer
{
    class Program
    {
        static void Main(string[] args)
        {
            World w = new World();
            w.build();
            w.render_scene();

            //Console.ReadKey();
        }
    }
}
