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
using System.Xml;

namespace RayTracer
{
    /// <summary>
    /// Wrapper providing support for transformed objects
    /// </summary>
    class Instance : RenderableObject
    {
        private Matrix inv_net_mat; //Inverse transformation matrix (for ray transformation)
        private Matrix net_mat; //Transformation matrix (for bounding box generation for instancing inside grid or octree)

        private RenderableObject payload; //Serves as wrapper for another renderable object
        private BoundingBox bbox;
        
        //Default constructor
        public Instance()
        {
            inv_net_mat = new Matrix();
            net_mat = new Matrix();
            this.setMaterial(null);
        }
        //Constructor with handle
        public Instance(RenderableObject handle)
        {
            inv_net_mat = new Matrix();
            net_mat = new Matrix();
            payload = handle;
            this.setMaterial(null);
        }

        public override string ToString()
        {
            return "Instanced object:\n" +
                "  ID: " + id + "\n" +
                "  Obj: " + payload.id+ "\n"+
                "  Mat: " + (mat == null ? payload.getMaterial().id : mat.id);
        }
        public RenderableObject getHandle()
        {
            return payload;
        }
        public void setHandle(RenderableObject handle)
        {
            payload = handle;
        }

        public void translate(Vect3D trans)
        {
            Matrix inv_temp = Matrix.inv_translate(trans);
            Matrix temp = Matrix.translate(trans);

            inv_net_mat = inv_net_mat * inv_temp; //Post multiply for inverse transformation matrix
            net_mat = temp * net_mat; //Pre multiply for transformation matrix.
        }
        public void translate(double x, double y, double z)
        {
            Matrix inv_temp = Matrix.inv_translate(x, y, z);
            Matrix temp = Matrix.translate(x, y, z);

            inv_net_mat = inv_net_mat * inv_temp; //Post multiply for inverse transformation matrix
            net_mat = temp * net_mat; //Pre multiply for transformation matrix.
        }
        public void rotate(Vect3D rot)
        {
            Matrix inv_temp = Matrix.inv_rotateDeg(rot);
            Matrix temp = Matrix.rotateDeg(rot);

            inv_net_mat = inv_net_mat * inv_temp; //Post multiply for inverse transformation matrix
            net_mat = temp * net_mat; //Pre multiply for transformation matrix.
        }
        public void rotate(double x, double y, double z)
        {
            Vect3D rot = new Vect3D(x, y, z);
            Matrix inv_temp = Matrix.inv_rotateDeg(rot);
            Matrix temp = Matrix.rotateDeg(rot);

            inv_net_mat = inv_net_mat * inv_temp; //Post multiply for inverse transformation matrix
            net_mat = temp * net_mat; //Pre multiply for transformation matrix.

        }
        public void scale(Vect3D scale)
        {
            Matrix inv_temp = Matrix.inv_scale(scale);
            Matrix temp = Matrix.scale(scale);

            inv_net_mat = inv_net_mat * inv_temp; //Post multiply for inverse transformation matrix
            net_mat = temp * net_mat; //Pre multiply for transformation matrix.
        }
        public void scale(double x, double y, double z)
        {
            Vect3D scale = new Vect3D(x, y, z);
            Matrix inv_temp = Matrix.inv_scale(scale);
            Matrix temp = Matrix.scale(scale);

            inv_net_mat = inv_net_mat * inv_temp; //Post multiply for inverse transformation matrix
            net_mat = temp * net_mat; //Pre multiply for transformation matrix.
        }
        public void applyTransformation(Matrix trans, Matrix inv_trans)
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
            if (mat == null)
            {
                return payload.getMaterial();
            }
            else
            {
                return mat;
            }
        }
        public override BoundingBox get_bounding_box()
        {
            compute_bounding_box();
            return bbox;
        }
        public void compute_bounding_box()
        {
            //Get the bounding box of the payload prior to transformation.
            BoundingBox preTransform = payload.get_bounding_box();
            double x0 = preTransform.x0;
            double x1 = preTransform.x1;
            double y0 = preTransform.y0;
            double y1 = preTransform.y1;
            double z0 = preTransform.z0;
            double z1 = preTransform.z1;

            //Get points representing all 8 corners of the bounding box
            Point3D[] points = new Point3D[8];
            points[0] = new Point3D(x0, y0, z0);
            points[1] = new Point3D(x0, y1, z0);
            points[2] = new Point3D(x0, y0, z1);
            points[3] = new Point3D(x0, y1, z1);
            points[4] = new Point3D(x1, y0, z0);
            points[5] = new Point3D(x1, y1, z0);
            points[6] = new Point3D(x1, y0, z1);
            points[7] = new Point3D(x1, y1, z1);

            //Transform all corner points
            for(int i = 0; i < 8; i++)
            {
                points[i] = net_mat * points[i];
            }

            double xmin = GlobalVars.kHugeValue;
            double xmax = -GlobalVars.kHugeValue;
            double ymin = GlobalVars.kHugeValue;
            double ymax = -GlobalVars.kHugeValue;
            double zmin = GlobalVars.kHugeValue;
            double zmax = -GlobalVars.kHugeValue;

            //Find xmin, xmax, ymin, ymax, and zmin, zmax
            for(int i = 0; i < 8; i++)
            {
                if (points[i].xcoord < xmin) xmin = points[i].xcoord;
                if (points[i].xcoord > xmax) xmax = points[i].xcoord;
                if (points[i].ycoord < ymin) ymin = points[i].ycoord;
                if (points[i].ycoord > ymax) ymax = points[i].ycoord;
                if (points[i].zcoord < zmin) zmin = points[i].zcoord;
                if (points[i].zcoord > zmax) zmax = points[i].zcoord;
            }

            //Create bounding box based on transformed payload bounding box
            bbox = new BoundingBox(xmin, xmax, ymin, ymax, zmin, zmax);
        }

        public static Instance LoadInstance(XmlElement def)
        {
            Instance toReturn = new Instance();

            try
            {
                if (def.HasAttribute("mat"))
                {
                    toReturn.setMaterial(GlobalVars.worldref.getMaterialById(def.GetAttribute("mat")));
                }

                if (def.HasAttribute("obj"))
                {
                    //Verify that object definition has been previously defined.
                    RenderableObject objRef = GlobalVars.worldref.getObjectById(def.GetAttribute("obj"));
                    if (objRef != null)
                    {
                        toReturn.setHandle(objRef);
                    }
                    else
                    {
                        throw new XmlException("Error: No object definition with handle: " + def.GetAttribute("obj"));
                    }  
                }
                else
                {
                    //See if there's a nested object
                    XmlNode childobj = def.SelectSingleNode("renderable");
                    if (childobj != null)
                        toReturn.setHandle(RenderableObject.LoadRenderableObject((XmlElement)childobj));
                    else
                        throw new XmlException("Error: Cannot create instance without obj paired with an object id.");
                }

                //If defined, get rotation data
                XmlNode rotNode = def.SelectSingleNode("rotate");
                if (rotNode != null)
                {
                    string rotString = ((XmlText)rotNode.FirstChild).Data;
                    Vect3D rotations = Vect3D.FromCsv(rotString);
                    //Rotation format valid?
                    if (rotations != null)
                    {
                        //Accumulate rotation on base transformation matrix
                        toReturn.rotate(rotations);
                    }
                }

                //If defined, get scaling data
                XmlNode scaleNode = def.SelectSingleNode("scale");
                if (scaleNode != null)
                {
                    string scaleString = ((XmlText)scaleNode.FirstChild).Data;
                    Vect3D scaling = Vect3D.FromCsv(scaleString);
                    //Scaling format valid?
                    if (scaling != null)
                    {
                        //Accumulate scaling on base transformation matrix
                        toReturn.scale(scaling);
                    }
                }

                //If defined, get translation data
                XmlNode transNode = def.SelectSingleNode("translate");
                if (transNode != null)
                {
                    string transString = ((XmlText)transNode.FirstChild).Data;
                    Vect3D translation = Vect3D.FromCsv(transString);
                    //Translation format valid?
                    if (translation != null)
                    {
                        //Accumulate translation on base transformation matrix
                        toReturn.translate(translation);
                    }
                }
            }
            catch (XmlException e) { Console.WriteLine(e.ToString()); }
            toReturn.compute_bounding_box();

            return toReturn;
        }
    }
}
