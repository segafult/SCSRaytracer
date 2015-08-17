//    
//    Copyright(C) 2015  Elanna Stephenson
//
//    This software is released under the MIT license, see LICENSE for details.
//    

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Xml;

namespace RayTracer
{
    class UniformGrid : CompoundRenderable
    {
        private List<RenderableObject> cells;
        private BoundingBox bbox;
        int nx, ny, nz;

        //Constructors
        public UniformGrid() : base()
        {
            cells = new List<RenderableObject>();
            bbox = new BoundingBox();
        }

        //Gets
        public override BoundingBox get_bounding_box()
        {
            return bbox;
        }

        /// <summary>
        /// Sets up the grid acceleration structure
        /// </summary>
        public void setup_cells()
        {
            //Construct bounding box
            Point3D pmin = min_coordinates();
            Point3D pmax = max_coordinates();
            bbox.x0 = pmin.coords.X; bbox.y0 = pmin.coords.Y; bbox.z0 = pmin.coords.Z;
            bbox.x1 = pmax.coords.X; bbox.y1 = pmax.coords.Y; bbox.z1 = pmax.coords.Z;

            int numobjs = objs.Count;
            //Find width of grid structure
            float wx = pmax.coords.X - pmin.coords.X;
            float wy = pmax.coords.Y - pmin.coords.Y;
            float wz = pmax.coords.Z - pmin.coords.Z;
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
            cells = new List<RenderableObject>(num_cells);
            for(int i = 0; i < num_cells; i++)
            {
                cells.Add(null);
            }

            //Temporary list to hold the number of objects in each cell
            List<int> counts = new List<int>(num_cells);
            for(int i = 0;i < num_cells;i++)
            {
                counts.Add(0);
            }

            BoundingBox object_bbox;
            int index;

            //Put objects in cells
            for(int i = 0; i < numobjs; i++)
            {
                object_bbox = objs[i].get_bounding_box();

                //Find the corners of the bounding box in terms of cell indices of the grid
                //According to the mathematical relationship f(p) = (px - p0x)/(p1x-p0x) [0.0,1.0]
                //index = nf(p)
                int ixmin = (int)clamp((object_bbox.x0 - pmin.coords.X) * nx / (pmax.coords.X - pmin.coords.X), 0, nx - 1);
                int iymin = (int)clamp((object_bbox.y0 - pmin.coords.Y) * ny / (pmax.coords.Y - pmin.coords.Y), 0, ny - 1);
                int izmin = (int)clamp((object_bbox.z0 - pmin.coords.Z) * nz / (pmax.coords.Z - pmin.coords.Z), 0, nz - 1);
                int ixmax = (int)clamp((object_bbox.x1 - pmin.coords.X) * nx / (pmax.coords.X - pmin.coords.X), 0, nx - 1);
                int iymax = (int)clamp((object_bbox.y1 - pmin.coords.Y) * ny / (pmax.coords.Y - pmin.coords.Y), 0, ny - 1);
                int izmax = (int)clamp((object_bbox.z1 - pmin.coords.Z) * nz / (pmax.coords.Z - pmin.coords.Z), 0, nz - 1);

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
                                cells[index] = objs[i];
                                counts[index]++;
                            }
                            //If object is already stored in cell, create a new compound object and store
                            //the two objects in it
                            else if(counts[index] == 1)
                            {
                                CompoundRenderable compound_object = new CompoundRenderable();
                                compound_object.add_object(cells[index]);
                                compound_object.add_object(objs[i]);

                                cells[index] = compound_object;
                                counts[index]++;
                            }
                            //If more than one object is in the cell, just add it to the compound object already contained in the cell
                            else if(counts[index] > 1)
                            {
                                //Safe to assume that if this code is running, the object at cells[index] is a compoundrenderable
                                ((CompoundRenderable)cells[index]).add_object(objs[i]);
                                counts[index]++;
                            }
                        }
                        
                    }
                }
            }
        }
        
        public override bool hit(Ray r, ref float tmin, ref ShadeRec sr)
        {
            float ox = r.origin.coords.X;
            float oy = r.origin.coords.Y;
            float oz = r.origin.coords.Z;
            float dx = r.direction.coords.X;
            float dy = r.direction.coords.Y;
            float dz = r.direction.coords.Z;

            float x0 = bbox.x0;
            float y0 = bbox.y0;
            float z0 = bbox.z0;
            float x1 = bbox.x1;
            float y1 = bbox.y1;
            float z1 = bbox.z1;

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
            if(bbox.inside(r.origin))
            {
                ix = (int)clamp((ox - x0) * nx / (x1 - x0), 0, nx - 1); //Get indices of ray origin if inside
                iy = (int)clamp((oy - y0) * ny / (y1 - y0), 0, ny - 1);
                iz = (int)clamp((oz - z0) * nz / (z1 - z0), 0, nz - 1);
            }
            else
            {
                Point3D p = r.origin + t0 * r.direction;
                ix = (int)clamp((p.coords.X - x0) * nx / (x1 - x0), 0, nx - 1); //Get indices of ray hitpoint if origin outside
                iy = (int)clamp((p.coords.Y - y0) * ny / (y1 - y0), 0, ny - 1);
                iz = (int)clamp((p.coords.Z - z0) * nz / (z1 - z0), 0, nz - 1);
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
                tx_next = GlobalVars.kHugeValue;
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
                ty_next = GlobalVars.kHugeValue;
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
                tz_next = GlobalVars.kHugeValue;
                iz_step = -1;
                iz_stop = -1;
            }

            //Traverse grid
            while(true)
            {
                RenderableObject object_ptr = cells[ix + (nx * iy) + (nx * ny * iz)];

                //Is the next cell an increment in the x direction?
                if(tx_next < ty_next && tx_next < tz_next)
                {
                    //Hit an object in this cell, cull cells behind this object
                    if ((object_ptr != null) && object_ptr.hit(r, ref tmin, ref sr) && tmin < tx_next)
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
                        if (object_ptr != null && object_ptr.hit(r, ref tmin, ref sr) && tmin < ty_next) 
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
                        if (object_ptr != null && object_ptr.hit(r, ref tmin, ref sr) && tmin < tz_next)
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

        public override bool hit(Ray r, float tmin)
        {
            float ox = r.origin.coords.X;
            float oy = r.origin.coords.Y;
            float oz = r.origin.coords.Z;
            float dx = r.direction.coords.X;
            float dy = r.direction.coords.Y;
            float dz = r.direction.coords.Z;

            float x0 = bbox.x0;
            float y0 = bbox.y0;
            float z0 = bbox.z0;
            float x1 = bbox.x1;
            float y1 = bbox.y1;
            float z1 = bbox.z1;

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

            if (t0 > t1 && !bbox.inside(r.origin))
            {
                return false;
            }

            //Initial cell coordinates
            int ix, iy, iz;
            if (bbox.inside(r.origin))
            {
                ix = (int)clamp((ox - x0) * nx / (x1 - x0), 0, nx - 1); //Get indices of ray origin if inside
                iy = (int)clamp((oy - y0) * ny / (y1 - y0), 0, ny - 1);
                iz = (int)clamp((oz - z0) * nz / (z1 - z0), 0, nz - 1);
            }
            else
            {
                Point3D p = r.origin + t0 * r.direction;
                ix = (int)clamp((p.coords.X - x0) * nx / (x1 - x0), 0, nx - 1); //Get indices of ray hitpoint if origin outside
                iy = (int)clamp((p.coords.Y - y0) * ny / (y1 - y0), 0, ny - 1);
                iz = (int)clamp((p.coords.Z - z0) * nz / (z1 - z0), 0, nz - 1);
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
            if (dx == 0.0)
            {
                tx_next = GlobalVars.kHugeValue;
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
            if (dy == 0.0)
            {
                ty_next = GlobalVars.kHugeValue;
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
            if (dz == 0.0)
            {
                tz_next = GlobalVars.kHugeValue;
                iz_step = -1;
                iz_stop = -1;
            }

            //Traverse grid
            while (true)
            {
                RenderableObject object_ptr = cells[ix + (nx * iy) + (nx * ny * iz)];

                //Is the next cell an increment in the x direction?
                if (tx_next < ty_next && tx_next < tz_next)
                {
                    //Hit an object in this cell, cull cells behind this object
                    if ((object_ptr != null) && object_ptr.hit(r, tmin) && tmin < tx_next)
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
                        if (object_ptr != null && object_ptr.hit(r, tmin) && tmin < ty_next)
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
                        if (object_ptr != null && object_ptr.hit(r, tmin) && tmin < tz_next)
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
        private Point3D min_coordinates()
        {
            BoundingBox bbox;
            Point3D pmin = new Point3D(GlobalVars.kHugeValue, GlobalVars.kHugeValue, GlobalVars.kHugeValue);

            int numobjects = objs.Count;
            for(int i = 0; i < numobjects; i++)
            {
                bbox = objs[i].get_bounding_box();
                if (bbox.x0 < pmin.coords.X) pmin.coords.X = bbox.x0;
                if (bbox.y0 < pmin.coords.Y) pmin.coords.Y = bbox.y0;
                if (bbox.z0 < pmin.coords.Z) pmin.coords.Z = bbox.z0;
            }

            pmin.coords.X -= GlobalVars.kEpsilon;
            pmin.coords.Y -= GlobalVars.kEpsilon;
            pmin.coords.Z -= GlobalVars.kEpsilon;

            return pmin;
        }
        /// <summary>
        /// Maximum coordinates in contained objects
        /// </summary>
        /// <returns>A Point3D containing the maximum x, y and z coordinates of the contained objects.</returns>
        private Point3D max_coordinates()
        {
            BoundingBox bbox;
            Point3D pmax = new Point3D(-GlobalVars.kHugeValue, -GlobalVars.kHugeValue, -GlobalVars.kHugeValue);

            int numobjects = objs.Count;
            for(int i = 0; i < numobjects; i++)
            {
                bbox = objs[i].get_bounding_box();
                if (bbox.x1 > pmax.coords.X) pmax.coords.X = bbox.x1;
                if (bbox.y1 > pmax.coords.Y) pmax.coords.Y = bbox.y1;
                if (bbox.z1 > pmax.coords.Z) pmax.coords.Z = bbox.z1;
            }

            pmax.coords.X += GlobalVars.kEpsilon;
            pmax.coords.Y += GlobalVars.kEpsilon;
            pmax.coords.Z += GlobalVars.kEpsilon;

            return pmax;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private float clamp(float x, float xmin, float xmax)
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
                    toReturn.add_object(rend);
            }

            toReturn.setup_cells();
            return toReturn;
        }
    }
}
