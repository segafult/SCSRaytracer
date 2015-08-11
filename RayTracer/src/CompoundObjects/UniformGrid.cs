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
using System.Runtime.CompilerServices;

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
            bbox.x0 = pmin.xcoord; bbox.y0 = pmin.ycoord; bbox.z0 = pmin.zcoord;
            bbox.x1 = pmax.xcoord; bbox.y1 = pmax.ycoord; bbox.z1 = pmax.zcoord;

            int numobjs = objs.Count;
            //Find width of grid structure
            double wx = pmax.xcoord - pmin.xcoord;
            double wy = pmax.ycoord - pmin.ycoord;
            double wz = pmax.zcoord - pmin.zcoord;
            double multiplier = 2.0; //For approximately 8x the number of cells as objects.

            //Calculate number of cells according to formulae given by Shirley (2000)
            //s = (wxwywz/n)^(1/3)
            //n = trunc(mw/s) + 1;
            double s = Math.Pow((wx * wy * wz) / numobjs, (1.0 / 3.0));
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
                int ixmin = (int)clamp((object_bbox.x0 - pmin.xcoord) * nx / (pmax.xcoord - pmin.xcoord), 0, nx - 1);
                int iymin = (int)clamp((object_bbox.y0 - pmin.ycoord) * ny / (pmax.ycoord - pmin.ycoord), 0, ny - 1);
                int izmin = (int)clamp((object_bbox.z0 - pmin.zcoord) * nz / (pmax.zcoord - pmin.zcoord), 0, nz - 1);
                int ixmax = (int)clamp((object_bbox.x1 - pmin.xcoord) * nx / (pmax.xcoord - pmin.xcoord), 0, nx - 1);
                int iymax = (int)clamp((object_bbox.y1 - pmin.ycoord) * ny / (pmax.ycoord - pmin.ycoord), 0, ny - 1);
                int izmax = (int)clamp((object_bbox.z1 - pmin.zcoord) * nz / (pmax.zcoord - pmin.zcoord), 0, nz - 1);

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
        
        public override bool hit(Ray r, ref double tmin, ref ShadeRec sr)
        {
            double ox = r.origin.xcoord;
            double oy = r.origin.ycoord;
            double oz = r.origin.zcoord;
            double dx = r.direction.xcoord;
            double dy = r.direction.ycoord;
            double dz = r.direction.zcoord;

            double x0 = bbox.x0;
            double y0 = bbox.y0;
            double z0 = bbox.z0;
            double x1 = bbox.x1;
            double y1 = bbox.y1;
            double z1 = bbox.z1;

            double tx_min, ty_min, tz_min;
            double tx_max, ty_max, tz_max;

            double a = 1.0 / dx;
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

            double b = 1.0 / dy;
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

            double c = 1.0 / dz;
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
            double t0, t1;
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
                ix = (int)clamp((p.xcoord - x0) * nx / (x1 - x0), 0, nx - 1); //Get indices of ray hitpoint if origin outside
                iy = (int)clamp((p.ycoord - y0) * ny / (y1 - y0), 0, ny - 1);
                iz = (int)clamp((p.zcoord - z0) * nz / (z1 - z0), 0, nz - 1);
            }

            //Get increments for ray marching
            double dtx = (tx_max - tx_min) / nx;
            double dty = (ty_max - ty_min) / ny;
            double dtz = (tz_max - tz_min) / nz;

            double tx_next, ty_next, tz_next;
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
                        mat = object_ptr.getMaterial();
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
                            mat = object_ptr.getMaterial();
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
                            mat = object_ptr.getMaterial();
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

        public override bool hit(Ray r, double tmin)
        {
            double ox = r.origin.xcoord;
            double oy = r.origin.ycoord;
            double oz = r.origin.zcoord;
            double dx = r.direction.xcoord;
            double dy = r.direction.ycoord;
            double dz = r.direction.zcoord;

            double x0 = bbox.x0;
            double y0 = bbox.y0;
            double z0 = bbox.z0;
            double x1 = bbox.x1;
            double y1 = bbox.y1;
            double z1 = bbox.z1;

            double tx_min, ty_min, tz_min;
            double tx_max, ty_max, tz_max;

            double a = 1.0 / dx;
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

            double b = 1.0 / dy;
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

            double c = 1.0 / dz;
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
            double t0, t1;
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
                ix = (int)clamp((p.xcoord - x0) * nx / (x1 - x0), 0, nx - 1); //Get indices of ray hitpoint if origin outside
                iy = (int)clamp((p.ycoord - y0) * ny / (y1 - y0), 0, ny - 1);
                iz = (int)clamp((p.zcoord - z0) * nz / (z1 - z0), 0, nz - 1);
            }

            //Get increments for ray marching
            double dtx = (tx_max - tx_min) / nx;
            double dty = (ty_max - ty_min) / ny;
            double dtz = (tz_max - tz_min) / nz;

            double tx_next, ty_next, tz_next;
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
                if (bbox.x0 < pmin.xcoord) pmin.xcoord = bbox.x0;
                if (bbox.y0 < pmin.ycoord) pmin.ycoord = bbox.y0;
                if (bbox.z0 < pmin.zcoord) pmin.zcoord = bbox.z0;
            }

            pmin.xcoord -= GlobalVars.kEpsilon;
            pmin.ycoord -= GlobalVars.kEpsilon;
            pmin.zcoord -= GlobalVars.kEpsilon;

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
                if (bbox.x1 > pmax.xcoord) pmax.xcoord = bbox.x1;
                if (bbox.y1 > pmax.ycoord) pmax.ycoord = bbox.y1;
                if (bbox.z1 > pmax.zcoord) pmax.zcoord = bbox.z1;
            }

            pmax.xcoord += GlobalVars.kEpsilon;
            pmax.ycoord += GlobalVars.kEpsilon;
            pmax.zcoord += GlobalVars.kEpsilon;

            return pmax;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private double clamp(double x, double xmin, double xmax)
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
    }
}
