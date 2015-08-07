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
            if (mat == null)
            {
                return payload.getMaterial();
            }
            else
            {
                return mat;
            }
        }

        public static Instance LoadInstance(XmlElement def, World w)
        {
            Instance toReturn = new Instance();
            toReturn.id = def.GetAttribute("id");
            try
            {
                if (def.HasAttribute("mat"))
                {
                    toReturn.setMaterial(w.getMaterialById(def.GetAttribute("mat")));
                }

                if (def.HasAttribute("obj"))
                {
                    //Verify that object definition has been previously defined.
                    RenderableObject objRef = w.getObjectById(def.GetAttribute("obj"));
                    if (objRef != null)
                    {
                        toReturn.setHandle(objRef);
                    }
                    else
                    {
                        throw new XmlException("Error: No object definition with handle: " + def.GetAttribute("obj"));
                    }

                    Matrix baseTransformation = new Matrix();

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
                            baseTransformation = baseTransformation * Matrix.rotateDeg(rotations);
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
                            baseTransformation = baseTransformation * Matrix.scale(scaling);
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
                            baseTransformation = baseTransformation * Matrix.translate(translation);
                        }
                    }

                    //Apply accumulated transformations to instance
                    toReturn.applyTransformation(baseTransformation);
                }
                else
                {
                    throw new XmlException("Error: Cannot create instance without obj paired with an object id.");
                }
            }
            catch (XmlException e) { Console.WriteLine(e.ToString()); }

            return toReturn;
        }
    }
}
