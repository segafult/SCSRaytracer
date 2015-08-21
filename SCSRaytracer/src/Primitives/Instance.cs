//    
//    Copyright(C) 2015  Elanna Stephenson
//
//    This software is released under the MIT license, see LICENSE for details.
//    

using System;
using System.Xml;
using System.Numerics;

namespace SCSRaytracer
{
    /// <summary>
    /// Wrapper providing support for transformed objects
    /// </summary>
    sealed class Instance : RenderableObject
    {
        private Matrix4x4 inv_net_mat; //Inverse transformation matrix (for ray transformation)
        private Matrix4x4 net_mat; //Transformation matrix (for bounding box generation for instancing inside grid or octree)

        private RenderableObject payload; //Serves as wrapper for another renderable object
        private BoundingBox bbox;
        
        //Default constructor
        public Instance()
        {
            inv_net_mat = Matrix4x4.Identity;
            net_mat = Matrix4x4.Identity;
            this.setMaterial(null);
        }
        //Constructor with handle
        public Instance(RenderableObject handle)
        {
            inv_net_mat = Matrix4x4.Identity;
            net_mat = Matrix4x4.Identity;
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
            Matrix4x4 temp = Matrix4x4.CreateTranslation(trans.coords);
            Matrix4x4 inv_temp;
            Matrix4x4.Invert(temp, out inv_temp);

            inv_net_mat = inv_temp * inv_net_mat; //Post multiply for inverse transformation matrix
            net_mat = net_mat * temp; //Pre multiply for transformation matrix.
        }
        public void translate(float x, float y, float z)
        {
            Matrix4x4 temp = Matrix4x4.CreateTranslation(x, y, z);
            Matrix4x4 inv_temp;
            Matrix4x4.Invert(temp, out inv_temp);

            inv_net_mat = inv_temp * inv_net_mat; //Post multiply for inverse transformation matrix
            net_mat = net_mat * temp; //Pre multiply for transformation matrix.
        }
        public void rotate(Vect3D rot)
        {
            rotate(rot.coords.X, rot.coords.Y, rot.coords.Z);
        }
        public void rotate(float x, float y, float z)
        {
            Vect3D rot = new Vect3D(x, y, z);
            Matrix4x4 xmat = Matrix4x4.CreateRotationX(x * FastMath.THREESIXTYINVTWOPI);
            Matrix4x4 ymat = Matrix4x4.CreateRotationY(y * FastMath.THREESIXTYINVTWOPI);
            Matrix4x4 zmat = Matrix4x4.CreateRotationZ(z * FastMath.THREESIXTYINVTWOPI);
            Matrix4x4 xmatinv, ymatinv, zmatinv;
            Matrix4x4.Invert(xmat, out xmatinv);
            Matrix4x4.Invert(ymat, out ymatinv);
            Matrix4x4.Invert(zmat, out zmatinv);
            Matrix4x4 temp = xmat * ymat * zmat;
            Matrix4x4 inv_temp = zmatinv * ymatinv * xmatinv;


            inv_net_mat = inv_temp * inv_net_mat; //Post multiply for inverse transformation matrix
            net_mat = net_mat * temp; //Pre multiply for transformation matrix.

        }
        public void scale(Vect3D scale)
        {
            Matrix4x4 temp = Matrix4x4.CreateScale(scale.coords);
            Matrix4x4 inv_temp;

            Matrix4x4.Invert(temp, out inv_temp);


            inv_net_mat = inv_temp * inv_net_mat; //Post multiply for inverse transformation matrix
            net_mat = net_mat * temp; //Pre multiply for transformation matrix.
        }
        public void scale(float x, float y, float z)
        {
            Matrix4x4 temp = Matrix4x4.CreateScale(x, y, z);
            Matrix4x4 inv_temp;
            Matrix4x4.Invert(temp, out inv_temp);

            inv_net_mat = inv_temp * inv_net_mat; //Post multiply for inverse transformation matrix
            net_mat = net_mat * temp; //Pre multiply for transformation matrix.
        }
        public void applyTransformation(Matrix4x4 trans, Matrix4x4 inv_trans)
        {
            inv_net_mat = inv_trans;
            net_mat = trans;
        }

        public override bool hit(Ray r, ref float tmin, ref ShadeRec sr)
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
                if(mat != null)
                {
                    sr.obj_material = mat;
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        public override bool hit(Ray r, float tmin)
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
            float x0 = preTransform.c0.X;
            float x1 = preTransform.c1.X;
            float y0 = preTransform.c0.Y;
            float y1 = preTransform.c1.Y;
            float z0 = preTransform.c0.Z;
            float z1 = preTransform.c1.Z;

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

            float xmin = GlobalVars.kHugeValue;
            float xmax = -GlobalVars.kHugeValue;
            float ymin = GlobalVars.kHugeValue;
            float ymax = -GlobalVars.kHugeValue;
            float zmin = GlobalVars.kHugeValue;
            float zmax = -GlobalVars.kHugeValue;

            //Find xmin, xmax, ymin, ymax, and zmin, zmax
            for(int i = 0; i < 8; i++)
            {
                if (points[i].coords.X < xmin) xmin = points[i].coords.X;
                if (points[i].coords.X > xmax) xmax = points[i].coords.X;
                if (points[i].coords.Y < ymin) ymin = points[i].coords.Y;
                if (points[i].coords.Y > ymax) ymax = points[i].coords.Y;
                if (points[i].coords.Z < zmin) zmin = points[i].coords.Z;
                if (points[i].coords.Z > zmax) zmax = points[i].coords.Z;
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
                    //if (rotations != null)
                    //{
                        //Accumulate rotation on base transformation matrix
                        toReturn.rotate(rotations);
                    //}
                }

                //If defined, get scaling data
                XmlNode scaleNode = def.SelectSingleNode("scale");
                if (scaleNode != null)
                {
                    string scaleString = ((XmlText)scaleNode.FirstChild).Data;
                    Vect3D scaling = Vect3D.FromCsv(scaleString);
                    //Scaling format valid?
                    //if (scaling != null)
                    //{
                        //Accumulate scaling on base transformation matrix
                        toReturn.scale(scaling);
                    //}
                }

                //If defined, get translation data
                XmlNode transNode = def.SelectSingleNode("translate");
                if (transNode != null)
                {
                    string transString = ((XmlText)transNode.FirstChild).Data;
                    Vect3D translation = Vect3D.FromCsv(transString);
                    //Translation format valid?
                    //if (translation != null)
                    //{
                        //Accumulate translation on base transformation matrix
                        toReturn.translate(translation);
                    //}
                }
            }
            catch (XmlException e) { Console.WriteLine(e.ToString()); }
            toReturn.compute_bounding_box();

            return toReturn;
        }
    }
}
