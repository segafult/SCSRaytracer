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
using System.IO;

namespace RayTracer
{
    class Program
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
            w.build();

            switch(multithread)
            {
                case false:
                    w.camera.render_scene(w);
                    break;
                case true:
                    w.camera.render_scene_multithreaded(w, threads);
                    break;
            }
            
            w.drawPlan.RotateFlip(System.Drawing.RotateFlipType.RotateNoneFlipY);
            w.drawPlan.Save(GlobalVars.outFile);
            
        }
    }
}
