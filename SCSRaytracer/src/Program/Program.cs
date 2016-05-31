//    
//    Copyright(C) 2015  Elanna Stephenson
//
//    This software is released under the MIT license, see LICENSE for details.
//    

using System;
using System.IO;
using System.Numerics;
using System.Diagnostics;


namespace SCSRaytracer
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
                Console.WriteLine("Additional options:\n-V: Verbose output, default off\n-T #: Number of threads");
                Console.ReadKey();
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
                Console.WriteLine("Additional options:\n-V: Verbose output, default off\n-T #: Number of threads");
                Console.ReadKey();
                return;
            }
            //Elevate process priority to high

            using (Process p = Process.GetCurrentProcess())
                p.PriorityClass = ProcessPriorityClass.High;
            World w = new World();
            GlobalVars.WORLD_REF = w;

            w.Build();
            w.OpenWindow(w.CurrentViewPlane.HorizontalResolution, w.CurrentViewPlane.VerticalResolution);
            //while (GlobalVars.frameno < 120)
            //{
                //w.camera.setEye(new Point3D(200, 200, GlobalVars.cam_zcoord));
            //    w.camera.setLookat(new Point3D(0, 0, GlobalVars.lookat_zcoord));
            //    w.camera.compute_uvw();

                switch (multithread)
                {
                    case false:
                        w.Camera.RenderSceneMultithreaded(w, 1);
                        break;
                    case true:
                        w.Camera.RenderSceneMultithreaded(w, threads);
                        break;
                }

                w.SaveDisplayedImage(GlobalVars.outFile);

                //    GlobalVars.cam_zcoord -= 10;
                //    GlobalVars.lookat_zcoord -= 10;
                //    GlobalVars.frameno += 1;
                //w.animate();
                //GlobalVars.frameno++;
            //}


			while(!GlobalVars.should_close)
            {
                w.PollEvents();
            }

            
        }
    }
}
