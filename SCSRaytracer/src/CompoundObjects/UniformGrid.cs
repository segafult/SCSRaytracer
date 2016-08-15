//    
//    Copyright(C) 2015  Elanna Stephenson
//
//    This software is released under the MIT license, see LICENSE for details.
//    

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Xml;

namespace SCSRaytracer
{
    class UniformGrid : CompoundRenderable
    {
        private RenderableObject[] cells;
        private BoundingBox _boundingBox;
        int nx, ny, nz;

        public override BoundingBox BoundingBox
        {
            get
            {
                return _boundingBox;
            }
        }

        //Constructors
        public UniformGrid() : base()
        {
            //cells = new List<RenderableObject>();
            _boundingBox = new BoundingBox();
        }

        /*
        public override BoundingBox get_bounding_box()
        {
            return bbox;
        }
        */

        /// <summary>
        /// Sets up the grid acceleration structure
        /// </summary>
        public void SetupCells()
        {
            //Construct bounding box
            Point3D pmin = MinimumCoordinates();
            Point3D pmax = max_coordinates();

            _boundingBox.corner0 = pmin.Coordinates;
            _boundingBox.corner1 = pmax.Coordinates;
            //bbox.x0 = pmin.coords.X; bbox.y0 = pmin.coords.Y; bbox.z0 = pmin.coords.Z;
            //bbox.x1 = pmax.coords.X; bbox.y1 = pmax.coords.Y; bbox.z1 = pmax.coords.Z;

            int numobjs = containedObjects.Count;
            //Find width of grid structure
            float wx = pmax.X - pmin.X;
            float wy = pmax.Y - pmin.Y;
            float wz = pmax.Z - pmin.Z;
            float multiplier = 2.0f; //For approximately 8x the number of cells as objects.

            //Calculate number of cells according to formulae given by Shirley (2000)
            //s = (wxwywz/n)^(1/3)
            //n = trunc(mw/s) + 1;
            float s = (float)Math.Pow((double)((wx * wy * wz) / numobjs), (1.0 / 3.0));
            nx = (int)(multiplier * wx / s + 1);
            ny = (int)(multiplier * wy / s + 1);
            nz = (int)(multiplier * wz / s + 1);

            //Get the computed number of cells
            int num_cells = nx * ny * nz;
            //Null initialize the set of cells
            cells = new RenderableObject[num_cells];
            for(int i = 0; i < num_cells; i++)
            {
                cells[i] = null;
            }

            //Temporary list to hold the number of objects in each cell
            int[] counts = new int[num_cells];
            for(int i = 0;i < num_cells;i++)
            {
                counts[i] = 0;
            }

            BoundingBox objectBoundingBox;
            int index;

            //Put objects in cells
            for(int i = 0; i < numobjs; i++)
            {
                objectBoundingBox = containedObjects[i].BoundingBox;

                //Find the corners of the bounding box in terms of cell indices of the grid
                //According to the mathematical relationship f(p) = (px - p0x)/(p1x-p0x) [0.0,1.0]
                //index = nf(p)
                int ixmin = (int)Clamp((objectBoundingBox.corner0.X - pmin.X) * nx / (pmax.X - pmin.X), 0, nx - 1);
                int iymin = (int)Clamp((objectBoundingBox.corner0.Y - pmin.Y) * ny / (pmax.Y - pmin.Y), 0, ny - 1);
                int izmin = (int)Clamp((objectBoundingBox.corner0.Z - pmin.Z) * nz / (pmax.Z - pmin.Z), 0, nz - 1);
                int ixmax = (int)Clamp((objectBoundingBox.corner1.X - pmin.X) * nx / (pmax.X - pmin.X), 0, nx - 1);
                int iymax = (int)Clamp((objectBoundingBox.corner1.Y - pmin.Y) * ny / (pmax.Y - pmin.Y), 0, ny - 1);
                int izmax = (int)Clamp((objectBoundingBox.corner1.Z - pmin.Z) * nz / (pmax.Z - pmin.Z), 0, nz - 1);

                //int ixmin = (int)clamp((object_bbox.x0 - pmin.coords.X) * nx / (pmax.coords.X - pmin.coords.X), 0, nx - 1);
                //int iymin = (int)clamp((object_bbox.y0 - pmin.coords.Y) * ny / (pmax.coords.Y - pmin.coords.Y), 0, ny - 1);
                //int izmin = (int)clamp((object_bbox.z0 - pmin.coords.Z) * nz / (pmax.coords.Z - pmin.coords.Z), 0, nz - 1);
                //int ixmax = (int)clamp((object_bbox.x1 - pmin.coords.X) * nx / (pmax.coords.X - pmin.coords.X), 0, nx - 1);
                //int iymax = (int)clamp((object_bbox.y1 - pmin.coords.Y) * ny / (pmax.coords.Y - pmin.coords.Y), 0, ny - 1);
                //int izmax = (int)clamp((object_bbox.z1 - pmin.coords.Z) * nz / (pmax.coords.Z - pmin.coords.Z), 0, nz - 1);

                //With the index information traverse all cells and add objects to cells that they are contained in.
                for(int iz = izmin; iz <= izmax; iz++)
                {
                    for(int iy = iymin; iy <= iymax; iy++)
                    {
                        for(int ix = ixmin; ix<= ixmax;ix++)
                        {
                            index = ix + (nx * iy) + (nx * ny * iz);

                            //Just add the object if no objects are stored in cell already
                            if(counts[index] == 0)
                            {
                                cells[index] = containedObjects[i];
                                counts[index]++;
                            }
                            //If object is already stored in cell, create a new compound object and store
                            //the two objects in it
                            else if(counts[index] == 1)
                            {
                                CompoundRenderable compoundObject = new CompoundRenderable();
                                compoundObject.AddObject(cells[index]);
                                compoundObject.AddObject(containedObjects[i]);

                                cells[index] = compoundObject;
                                counts[index]++;
                            }
                            //If more than one object is in the cell, just add it to the compound object already contained in the cell
                            else if(counts[index] > 1)
                            {
                                //Safe to assume that if this code is running, the object at cells[index] is a compoundrenderable
                                ((CompoundRenderable)cells[index]).AddObject(containedObjects[i]);
                                counts[index]++;
                            }
                        }
                        
                    }
                }
            }
        }
        
        public override bool Hit(Ray ray, ref float tMin, ref ShadeRec sr)
        {
            //
            // Code from Suffern (2007)
            //
            float ox = ray.Origin.X;
            float oy = ray.Origin.Y;
            float oz = ray.Origin.Z;
            float dx = ray.Direction.X;
            float dy = ray.Direction.Y;
            float dz = ray.Direction.Z;

            float x0 = _boundingBox.corner0.X;
            float y0 = _boundingBox.corner0.Y;
            float z0 = _boundingBox.corner0.Z;
            float x1 = _boundingBox.corner1.X;
            float y1 = _boundingBox.corner1.Y;
            float z1 = _boundingBox.corner1.Z;

            float tx_min, ty_min, tz_min;
            float tx_max, ty_max, tz_max;

            float a = 1.0f / dx;
            if(a >= 0)
            {
                tx_min = (x0 - ox) * a;
                tx_max = (x1 - ox) * a;
            }
            else
            {
                tx_min = (x1 - ox) * a;
                tx_max = (x0 - ox) * a;
            }

            float b = 1.0f / dy;
            if(b >= 0)
            {
                ty_min = (y0 - oy) * b;
                ty_max = (y1 - oy) * b;
            }
            else
            {
                ty_min = (y1 - oy) * b;
                ty_max = (y0 - oy) * b;
            }

            float c = 1.0f / dz;
            if(c >= 0)
            {
                tz_min = (z0 - oz) * c;
                tz_max = (z1 - oz) * c;
            }
            else
            {
                tz_min = (z1 - oz) * c;
                tz_max = (z0 - oz) * c;
            }

            //Determine if volume was hit
            float t0, t1;
            t0 = (tx_min > ty_min) ? tx_min : ty_min;
            if (tz_min > t0) t0 = tz_min;
            t1 = (tx_max < ty_max) ? tx_max : ty_max;
            if (tz_max < t1) t1 = tz_max;

            if(t0 > t1)
            {
                return false;
            }

            //Initial cell coordinates
            int ix, iy, iz;
            if(_boundingBox.inside(ray.Origin))
            {
                ix = (int)Clamp((ox - x0) * nx / (x1 - x0), 0, nx - 1); //Get indices of ray origin if inside
                iy = (int)Clamp((oy - y0) * ny / (y1 - y0), 0, ny - 1);
                iz = (int)Clamp((oz - z0) * nz / (z1 - z0), 0, nz - 1);
            }
            else
            {
                Point3D p = ray.Origin + t0 * ray.Direction;
                ix = (int)Clamp((p.X - x0) * nx / (x1 - x0), 0, nx - 1); //Get indices of ray hitpoint if origin outside
                iy = (int)Clamp((p.Y - y0) * ny / (y1 - y0), 0, ny - 1);
                iz = (int)Clamp((p.Z - z0) * nz / (z1 - z0), 0, nz - 1);
            }

            //Get increments for ray marching
            float dtx = (tx_max - tx_min) / nx;
            float dty = (ty_max - ty_min) / ny;
            float dtz = (tz_max - tz_min) / nz;

            float tx_next, ty_next, tz_next;
            int ix_step, iy_step, iz_step;
            int ix_stop, iy_stop, iz_stop;

            if(dx > 0)
            {
                tx_next = tx_min + (ix + 1) * dtx;
                ix_step = 1;
                ix_stop = nx;
            }
            else
            {
                tx_next = tx_min + (nx - ix) * dtx;
                ix_step = -1;
                ix_stop = -1;
            }
            //Avoid divide by zero error
            if(dx == 0.0)
            {
                tx_next = GlobalVars.K_HUGE_VALUE;
                ix_step = -1;
                ix_stop = -1;
            }

            if(dy > 0)
            {
                ty_next = ty_min + (iy + 1) * dty;
                iy_step = 1;
                iy_stop = ny;
            }
            else
            {
                ty_next = ty_min + (ny - iy) * dty;
                iy_step = -1;
                iy_stop = -1;
            }
            //Avoid divide by zero error
            if(dy == 0.0)
            {
                ty_next = GlobalVars.K_HUGE_VALUE;
                iy_step = -1;
                iy_stop = -1;
            }

            if(dz > 0)
            {
                tz_next = tz_min + (iz + 1) * dtz;
                iz_step = +1;
                iz_stop = nz;
            }
            else
            {
                tz_next = tz_min + (nz - iz) * dtz;
                iz_step = -1;
                iz_stop = -1;
            }
            //Avoid divide by zero error
            if(dz == 0.0)
            {
                tz_next = GlobalVars.K_HUGE_VALUE;
                iz_step = -1;
                iz_stop = -1;
            }

            //Traverse grid
            while(true)
            {
                RenderableObject objectPointer = cells[ix + (nx * iy) + (nx * ny * iz)];

                //Is the next cell an increment in the x direction?
                if(tx_next < ty_next && tx_next < tz_next)
                {
                    //Hit an object in this cell, cull cells behind this object
                    if ((objectPointer != null) && objectPointer.Hit(ray, ref tMin, ref sr) && tMin < tx_next)
                    {
                        //mat = object_ptr.getMaterial();
                        return true;
                    }

                    //Walk the ray to the next cell
                    tx_next += dtx;
                    ix += ix_step;

                    //Havn't hit anything in the volume at all?
                    if(ix == ix_stop)
                    {
                        return false;
                    }
                }
                //Next cell is either an increment in y or z direction
                else
                {
                    //Next cell is in the y direction
                    if(ty_next < tz_next)
                    {
                        //Hit an object in this cell? Cull cells behind object
                        if (objectPointer != null && objectPointer.Hit(ray, ref tMin, ref sr) && tMin < ty_next) 
                        {
                            //mat = object_ptr.getMaterial();
                            return true;
                        }

                        //Walk the ray to the next cell
                        ty_next += dty;
                        iy += iy_step;

                        //Havn't hit anything in the volume?
                        if(iy == iy_stop)
                        {
                            return false;
                        }
                    }
                    //Next cell is in the z direction
                    else
                    {
                        //Hit an object in this cell? Cull cells behind object
                        if (objectPointer != null && objectPointer.Hit(ray, ref tMin, ref sr) && tMin < tz_next)
                        {
                            //mat = object_ptr.getMaterial();
                            return true;
                        }

                        //Walk the ray to the next cell
                        tz_next += dtz;
                        iz += iz_step;

                        //Havn't hit anything in the volume?
                        if(iz == iz_stop)
                        {
                            return false;
                        }
                    }
                }
            }      
        }

        public override bool Hit(Ray ray, float tMin)
        {
            float ox = ray.Origin.X;
            float oy = ray.Origin.Y;
            float oz = ray.Origin.Z;
            float dx = ray.Direction.X;
            float dy = ray.Direction.Y;
            float dz = ray.Direction.Z;

            float x0 = _boundingBox.corner0.X;
            float y0 = _boundingBox.corner0.Y;
            float z0 = _boundingBox.corner0.Z;
            float x1 = _boundingBox.corner1.X;
            float y1 = _boundingBox.corner1.Y;
            float z1 = _boundingBox.corner1.Z;

            float tx_min, ty_min, tz_min;
            float tx_max, ty_max, tz_max;

            float a = 1.0f / dx;
            if (a >= 0)
            {
                tx_min = (x0 - ox) * a;
                tx_max = (x1 - ox) * a;
            }
            else
            {
                tx_min = (x1 - ox) * a;
                tx_max = (x0 - ox) * a;
            }

            float b = 1.0f / dy;
            if (b >= 0)
            {
                ty_min = (y0 - oy) * b;
                ty_max = (y1 - oy) * b;
            }
            else
            {
                ty_min = (y1 - oy) * b;
                ty_max = (y0 - oy) * b;
            }

            float c = 1.0f / dz;
            if (c >= 0)
            {
                tz_min = (z0 - oz) * c;
                tz_max = (z1 - oz) * c;
            }
            else
            {
                tz_min = (z1 - oz) * c;
                tz_max = (z0 - oz) * c;
            }

            //Determine if volume was hit
            float t0, t1;
            t0 = (tx_min > ty_min) ? tx_min : ty_min;
            if (tz_min > t0) t0 = tz_min;
            t1 = (tx_max < ty_max) ? tx_max : ty_max;
            if (tz_max < t1) t1 = tz_max;

            if (t0 > t1 && !_boundingBox.inside(ray.Origin))
            {
                return false;
            }

            //Initial cell coordinates
            int ix, iy, iz;
            if (_boundingBox.inside(ray.Origin))
            {
                ix = (int)Clamp((ox - x0) * nx / (x1 - x0), 0, nx - 1); //Get indices of ray origin if inside
                iy = (int)Clamp((oy - y0) * ny / (y1 - y0), 0, ny - 1);
                iz = (int)Clamp((oz - z0) * nz / (z1 - z0), 0, nz - 1);
            }
            else
            {
                Point3D p = ray.Origin + t0 * ray.Direction;
                ix = (int)Clamp((p.X - x0) * nx / (x1 - x0), 0, nx - 1); //Get indices of ray hitpoint if origin outside
                iy = (int)Clamp((p.Y - y0) * ny / (y1 - y0), 0, ny - 1);
                iz = (int)Clamp((p.Z - z0) * nz / (z1 - z0), 0, nz - 1);
            }

            //Get increments for ray marching
            float dtx = (tx_max - tx_min) / nx;
            float dty = (ty_max - ty_min) / ny;
            float dtz = (tz_max - tz_min) / nz;

            float tx_next, ty_next, tz_next;
            int ix_step, iy_step, iz_step;
            int ix_stop, iy_stop, iz_stop;

            if (dx > 0)
            {
                tx_next = tx_min + (ix + 1) * dtx;
                ix_step = 1;
                ix_stop = nx;
            }
            else
            {
                tx_next = tx_min + (nx - ix) * dtx;
                ix_step = -1;
                ix_stop = -1;
            }
            //Avoid divide by zero error
            if (dx == 0.0f)
            {
                tx_next = GlobalVars.K_HUGE_VALUE;
                ix_step = -1;
                ix_stop = -1;
            }

            if (dy > 0)
            {
                ty_next = ty_min + (iy + 1) * dty;
                iy_step = 1;
                iy_stop = ny;
            }
            else
            {
                ty_next = ty_min + (ny - iy) * dty;
                iy_step = -1;
                iy_stop = -1;
            }
            //Avoid divide by zero error
            if (dy == 0.0f)
            {
                ty_next = GlobalVars.K_HUGE_VALUE;
                iy_step = -1;
                iy_stop = -1;
            }

            if (dz > 0)
            {
                tz_next = tz_min + (iz + 1) * dtz;
                iz_step = +1;
                iz_stop = nz;
            }
            else
            {
                tz_next = tz_min + (nz - iz) * dtz;
                iz_step = -1;
                iz_stop = -1;
            }
            //Avoid divide by zero error
            if (dz == 0.0f)
            {
                tz_next = GlobalVars.K_HUGE_VALUE;
                iz_step = -1;
                iz_stop = -1;
            }

            //Traverse grid
            while (true)
            {
                RenderableObject objectPointer = cells[ix + (nx * iy) + (nx * ny * iz)];

                //Is the next cell an increment in the x direction?
                if (tx_next < ty_next && tx_next < tz_next)
                {
                    //Hit an object in this cell, cull cells behind this object
                    if ((objectPointer != null) && objectPointer.Hit(ray, tMin) && tMin < tx_next)
                    {
                        return true;
                    }

                    //Walk the ray to the next cell
                    tx_next += dtx;
                    ix += ix_step;

                    //Havn't hit anything in the volume at all?
                    if (ix == ix_stop)
                    {
                        return false;
                    }
                }
                //Next cell is either an increment in y or z direction
                else
                {
                    //Next cell is in the y direction
                    if (ty_next < tz_next)
                    {
                        //Hit an object in this cell? Cull cells behind object
                        if (objectPointer != null && objectPointer.Hit(ray, tMin) && tMin < ty_next)
                        {
                            return true;
                        }

                        //Walk the ray to the next cell
                        ty_next += dty;
                        iy += iy_step;

                        //Havn't hit anything in the volume?
                        if (iy == iy_stop)
                        {
                            return false;
                        }
                    }
                    //Next cell is in the z direction
                    else
                    {
                        //Hit an object in this cell? Cull cells behind object
                        if (objectPointer != null && objectPointer.Hit(ray, tMin) && tMin < tz_next)
                        {
                            return true;
                        }

                        //Walk the ray to the next cell
                        tz_next += dtz;
                        iz += iz_step;

                        //Havn't hit anything in the volume?
                        if (iz == iz_stop)
                        {
                            return false;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Minimum coordinates in contained objects;
        /// </summary>
        /// <returns>A point3d with the minimum x, y, and z coordinates of contained objects.</returns>
        private Point3D MinimumCoordinates()
        {
            BoundingBox bbox;
            Point3D pmin = new Point3D(GlobalVars.K_HUGE_VALUE, GlobalVars.K_HUGE_VALUE, GlobalVars.K_HUGE_VALUE);

            int numobjects = containedObjects.Count;
            for(int i = 0; i < numobjects; i++)
            {
                bbox = containedObjects[i].BoundingBox;
                if (bbox.corner0.X < pmin.X) pmin.X = bbox.corner0.X;
                if (bbox.corner0.Y < pmin.Y) pmin.Y = bbox.corner0.Y;
                if (bbox.corner0.Z < pmin.Z) pmin.Z = bbox.corner0.Z;
            }

            pmin.X -= GlobalVars.K_EPSILON;
            pmin.Y -= GlobalVars.K_EPSILON;
            pmin.Z -= GlobalVars.K_EPSILON;

            return pmin;
        }
        /// <summary>
        /// Maximum coordinates in contained objects
        /// </summary>
        /// <returns>A Point3D containing the maximum x, y and z coordinates of the contained objects.</returns>
        private Point3D max_coordinates()
        {
            BoundingBox bbox;
            Point3D pmax = new Point3D(-GlobalVars.K_HUGE_VALUE, -GlobalVars.K_HUGE_VALUE, -GlobalVars.K_HUGE_VALUE);

            int numobjects = containedObjects.Count;
            for(int i = 0; i < numobjects; i++)
            {
                bbox = containedObjects[i].BoundingBox;
                if (bbox.corner1.X > pmax.X) pmax.X = bbox.corner1.X;
                if (bbox.corner1.Y > pmax.Y) pmax.Y = bbox.corner1.Y;
                if (bbox.corner1.Z > pmax.Z) pmax.Z = bbox.corner1.Z;
            }

            pmax.X += GlobalVars.K_EPSILON;
            pmax.Y += GlobalVars.K_EPSILON;
            pmax.Z += GlobalVars.K_EPSILON;

            return pmax;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private float Clamp(float x, float xmin, float xmax)
        {
            if(x < xmin)
            {
                return xmin;
            }
            else if(x > xmax)
            {
                return xmax;
            }
            else { return x; }
        }

        public static UniformGrid LoadUniformGrid(XmlElement def)
        {
            UniformGrid toReturn = new UniformGrid();

            XmlNodeList children = def.SelectNodes("renderable");
            foreach(XmlElement child in children)
            {
                RenderableObject rend = RenderableObject.LoadRenderableObject(child);
                if (rend != null)
                    toReturn.AddObject(rend);
            }

            toReturn.SetupCells();
            return toReturn;
        }
    }
}
