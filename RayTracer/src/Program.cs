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
            w.camera.render_scene(w);
            w.drawPlan.RotateFlip(System.Drawing.RotateFlipType.RotateNoneFlipY);
            w.drawPlan.Save("E:\\test.bmp");
            //Console.ReadKey();
        }
    }
}
