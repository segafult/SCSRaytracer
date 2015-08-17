//    
//    Copyright(C) 2015  Elanna Stephenson
//
//    This software is released under the MIT license, see LICENSE for details.
//    

using System;
using System.IO;

namespace RayTracer
{
    sealed class Program
    {
        static void Main(string[] args)
        {
            
            bool multithread = false;
            int threads = 2;

            int a = 0;
            int numArgs = args.Length;

            if(numArgs == 0)
            {
                Console.WriteLine("Usage: scsraytracer -I \"Input XML path\" -O \"Output bmp path\"");
                Console.WriteLine("Additional options:\n-V: Verbose output, default off\n-T #: Number of threads (2, 4 or 8), default 2");
                return;
            }
            try {
                //Cycle through all arguments
                while (a < numArgs)
                {
                    string arg = args[a];
                    if (arg.Equals("-I") && GlobalVars.inFile == null)
                    {
                        if ((a + 1 < numArgs) && File.Exists(args[a+1]))
                        {
                            GlobalVars.inFile = args[a + 1];
                            a += 2;
                        }
                        else
                        {
                            throw new ArgumentException("Invalid input file location");
                        }
                    }
                    else if (arg.Equals("-O") && GlobalVars.outFile == null)
                    {
                        if ((a + 1 < numArgs))
                        {
                            GlobalVars.outFile = args[a + 1];
                            a += 2;
                        }
                        else
                        {
                            throw new ArgumentException("Invalid output file location");
                        }
                    }
                    else if (arg.Equals("-V"))
                    {
                        GlobalVars.verbose = true;
                        a++;
                    }
                    else if (arg.Equals("-T"))
                    {
                        multithread = true;
                        threads = Convert.ToInt32(args[a + 1]);
                        a += 2;
                    }
                }

                if(GlobalVars.outFile == null)
                {
                    throw new ArgumentException();
                }


                if(File.Exists(GlobalVars.outFile))
                {
                    GetUserInput:
                    Console.Write("File " + GlobalVars.outFile + " exists! Overwrite? (y/n): ");
                    switch(Console.ReadKey().KeyChar)
                    {
                        case 'y':
                            //do nothing
                            break;
                        case 'n':
                            Console.WriteLine("\nOk, exiting now.");
                            return;
                        default:
                            Console.Write("\n");
                            goto GetUserInput;
                    }
                }
                
            }
            catch (ArgumentException e)
            {
                Console.WriteLine(e.ToString());
                Console.WriteLine("Usage: scsraytracer -I \"Input XML path\" -O \"Output bmp path\"");
                Console.WriteLine("Additional options:\n-V: Verbose output, default off\n-T #: Number of threads (2, 4 or 8), default 2");
                return;
            }

            World w = new World();
            GlobalVars.worldref = w;

            w.build();
            w.open_window(w.vp.hres, w.vp.vres);
            //while (GlobalVars.frameno < 120)
            //{
            //    w.camera.setEye(new Point3D(200, 200, GlobalVars.cam_zcoord));
            //    w.camera.setLookat(new Point3D(0, 0, GlobalVars.lookat_zcoord));
            //    w.camera.compute_uvw();

                switch (multithread)
                {
                    case false:
                        w.camera.render_scene(w);
                        break;
                    case true:
                        w.camera.render_scene_multithreaded(w, threads);
                        break;
                }

                w.save_displayed_image(GlobalVars.outFile);

            //    GlobalVars.cam_zcoord -= 10;
            //    GlobalVars.lookat_zcoord -= 10;
            //    GlobalVars.frameno += 1;
            //}


			while(!GlobalVars.should_close)
            {
                w.poll_events();
            }
            
        }
    }
}
