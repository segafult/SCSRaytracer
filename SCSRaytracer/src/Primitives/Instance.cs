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
        private Matrix4x4 inverseNetTransformationMatrix; //Inverse transformation matrix (for ray transformation)
        private Matrix4x4 netTransformationMatrix; //Transformation matrix (for bounding box generation for instancing inside grid or octree)

        private RenderableObject payload; //Serves as wrapper for another renderable object
        private BoundingBox boundingBox;

        // accessors
        public override Material Material
        {
            get
            {
                if (_material == null)
                {
                    return payload.Material;
                }
                else
                {
                    return _material;
                }
            }
            set
            {
                _material = value;
            }
        }
        public override BoundingBox BoundingBox
        {
            get
            {
                ComputeBoundingBox();
                return boundingBox;
            }
        }
        public RenderableObject Handle
        {
            get
            {
                return payload;
            }
            set
            {
                payload = value;
            }
        }

        //Default constructor
        public Instance()
        {
            inverseNetTransformationMatrix = Matrix4x4.Identity;
            netTransformationMatrix = Matrix4x4.Identity;
            this.Material = null;
        }
        //Constructor with handle
        public Instance(RenderableObject handle)
        {
            inverseNetTransformationMatrix = Matrix4x4.Identity;
            netTransformationMatrix = Matrix4x4.Identity;
            payload = handle;
            this.Material = null;
        }

        public override string ToString()
        {
            return "Instanced object:\n" +
                "  ID: " + id + "\n" +
                "  Obj: " + payload.id+ "\n"+
                "  Mat: " + (_material == null ? payload.Material.id : _material.id);
        }
        /*
        public RenderableObject getHandle()
        {
            return payload;
        }
        public void setHandle(RenderableObject handle)
        {
            payload = handle;
        }
        */
        public void Translate(Vect3D trans)
        {
            Matrix4x4 temp = Matrix4x4.CreateTranslation(trans.Coordinates);
            Matrix4x4 inv_temp;
            Matrix4x4.Invert(temp, out inv_temp);

            inverseNetTransformationMatrix = inv_temp * inverseNetTransformationMatrix; //Post multiply for inverse transformation matrix
            netTransformationMatrix = netTransformationMatrix * temp; //Pre multiply for transformation matrix.
        }
        public void Translate(float x, float y, float z)
        {
            Matrix4x4 temp = Matrix4x4.CreateTranslation(x, y, z);
            Matrix4x4 inv_temp;
            Matrix4x4.Invert(temp, out inv_temp);

            inverseNetTransformationMatrix = inv_temp * inverseNetTransformationMatrix; //Post multiply for inverse transformation matrix
            netTransformationMatrix = netTransformationMatrix * temp; //Pre multiply for transformation matrix.
        }
        public void Rotate(Vect3D rot)
        {
            Rotate(rot.X, rot.Y, rot.Z);
        }
        public void Rotate(float x, float y, float z)
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


            inverseNetTransformationMatrix = inv_temp * inverseNetTransformationMatrix; //Post multiply for inverse transformation matrix
            netTransformationMatrix = netTransformationMatrix * temp; //Pre multiply for transformation matrix.

        }
        public void Scale(Vect3D scale)
        {
            Matrix4x4 temp = Matrix4x4.CreateScale(scale.Coordinates);
            Matrix4x4 inv_temp;

            Matrix4x4.Invert(temp, out inv_temp);


            inverseNetTransformationMatrix = inv_temp * inverseNetTransformationMatrix; //Post multiply for inverse transformation matrix
            netTransformationMatrix = netTransformationMatrix * temp; //Pre multiply for transformation matrix.
        }
        public void Scale(float x, float y, float z)
        {
            Matrix4x4 temp = Matrix4x4.CreateScale(x, y, z);
            Matrix4x4 inv_temp;
            Matrix4x4.Invert(temp, out inv_temp);

            inverseNetTransformationMatrix = inv_temp * inverseNetTransformationMatrix; //Post multiply for inverse transformation matrix
            netTransformationMatrix = netTransformationMatrix * temp; //Pre multiply for transformation matrix.
        }
        public void ApplyTransformation(Matrix4x4 trans, Matrix4x4 inv_trans)
        {
            inverseNetTransformationMatrix = inv_trans;
            netTransformationMatrix = trans;
        }

        public override bool Hit(Ray r, ref float tmin, ref ShadeRec sr)
        {
            //Apply inverse transformation to incident ray and test for intersection
            Ray tfRay = new Ray(r);
            tfRay.Origin = inverseNetTransformationMatrix * r.Origin;
            tfRay.Direction = inverseNetTransformationMatrix * r.Direction;

            if(payload.Hit(tfRay, ref tmin, ref sr))
            {
                //Transform the computed normal into worldspace
                sr.Normal = inverseNetTransformationMatrix * sr.Normal;
                sr.Normal.Normalize();
                if(_material != null)
                {
                    sr.ObjectMaterial = _material;
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        public override bool Hit(Ray r, float tmin)
        {
            //Apply inverse transformation to incident ray and test for intersection
            Ray tfRay = new Ray(r);
            tfRay.Origin = inverseNetTransformationMatrix * r.Origin;
            tfRay.Direction = inverseNetTransformationMatrix * r.Direction;

            if (payload.Hit(tfRay, tmin))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /*
        public override Material getMaterial()
        {
            if (_material == null)
            {
                return payload.getMaterial();
            }
            else
            {
                return _material;
            }
        }
        
        public override BoundingBox get_bounding_box()
        {
            compute_bounding_box();
            return bbox;
        }
        */
        public void ComputeBoundingBox()
        {
            //Get the bounding box of the payload prior to transformation.
            BoundingBox preTransform = payload.BoundingBox;
            float x0 = preTransform.corner0.X;
            float x1 = preTransform.corner1.X;
            float y0 = preTransform.corner0.Y;
            float y1 = preTransform.corner1.Y;
            float z0 = preTransform.corner0.Z;
            float z1 = preTransform.corner1.Z;

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
                points[i] = netTransformationMatrix * points[i];
            }

            float xmin = GlobalVars.K_HUGE_VALUE;
            float xmax = -GlobalVars.K_HUGE_VALUE;
            float ymin = GlobalVars.K_HUGE_VALUE;
            float ymax = -GlobalVars.K_HUGE_VALUE;
            float zmin = GlobalVars.K_HUGE_VALUE;
            float zmax = -GlobalVars.K_HUGE_VALUE;

            //Find xmin, xmax, ymin, ymax, and zmin, zmax
            for(int i = 0; i < 8; i++)
            {
                if (points[i].X < xmin) xmin = points[i].X;
                if (points[i].X > xmax) xmax = points[i].X;
                if (points[i].Y < ymin) ymin = points[i].Y;
                if (points[i].Y > ymax) ymax = points[i].Y;
                if (points[i].Z < zmin) zmin = points[i].Z;
                if (points[i].Z > zmax) zmax = points[i].Z;
            }

            //Create bounding box based on transformed payload bounding box
            boundingBox = new BoundingBox(xmin, xmax, ymin, ymax, zmin, zmax);
        }

        public static Instance LoadInstance(XmlElement def)
        {
            Instance toReturn = new Instance();

            try
            {
                if (def.HasAttribute("mat"))
                {
                    toReturn.Material = GlobalVars.WORLD_REF.GetMaterialByID(def.GetAttribute("mat"));
                }

                if (def.HasAttribute("obj"))
                {
                    //Verify that object definition has been previously defined.
                    RenderableObject objRef = GlobalVars.WORLD_REF.GetObjectByID(def.GetAttribute("obj"));
                    if (objRef != null)
                    {
                        toReturn.Handle = objRef;
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
                        toReturn.Handle = RenderableObject.LoadRenderableObject((XmlElement)childobj);
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
                        toReturn.Rotate(rotations);
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
                        toReturn.Scale(scaling);
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
                        toReturn.Translate(translation);
                    //}
                }
            }
            catch (XmlException e) { Console.WriteLine(e.ToString()); }
            toReturn.ComputeBoundingBox();

            return toReturn;
        }
    }
}
