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

namespace RayTracer
{
    /// <summary>
    /// Wrapper providing support for transformed objects
    /// </summary>
    class Instance : RenderableObject
    {
        private Matrix inv_net_mat;
        private RenderableObject payload; //Serves as wrapper for another renderable object
        
        //Default constructor
        public Instance()
        {
            inv_net_mat = new Matrix();
            this.setMaterial(null);
        }
        //Constructor with handle
        public Instance(RenderableObject handle)
        {
            inv_net_mat = new Matrix();
            payload = handle;
            this.setMaterial(null);
        }

        public RenderableObject getHandle()
        {
            return payload;
        }
        public void setHandle(RenderableObject handle)
        {
            payload = handle;
        }

        /// <summary>
        /// Takes an inverse 4D transformation matrix and sets instance's
        /// </summary>
        /// <param name="inv_trans"></param>
        public void applyTransformation(Matrix inv_trans)
        {
            inv_net_mat = inv_trans;
        }

        public override bool hit(Ray r, ref double tmin, ref ShadeRec sr)
        {
            //Apply inverse transformation to incident ray and test for intersection
            Ray tfRay = new Ray(r);
            tfRay.origin = inv_net_mat * r.origin;
            tfRay.direction = inv_net_mat * r.direction;

            if(payload.hit(tfRay, ref tmin, ref sr))
            {
                //Transform the computed normal into worldspace
                sr.normal = inv_net_mat * sr.normal;
                sr.normal.normalize();
                sr.hit_point_local = r.origin + tmin * r.direction;
                return true;
            }
            else
            {
                return false;
            }
        }

        public override bool hit(Ray r, double tmin)
        {
            //Apply inverse transformation to incident ray and test for intersection
            Ray tfRay = new Ray(r);
            tfRay.origin = inv_net_mat * r.origin;
            tfRay.direction = inv_net_mat * r.direction;

            if (payload.hit(tfRay, tmin))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public override Material getMaterial()
        {
            if (getMaterial() == null)
            {
                return payload.getMaterial();
            }
            else
            {
                return getMaterial();
            }
        }
    }
}
