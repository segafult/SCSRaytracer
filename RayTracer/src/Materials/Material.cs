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
    public abstract class Material
    {
        public string id;
        public abstract RGBColor shade(ShadeRec sr);

        public static Material getMaterialById(World w, string idarg)
        {
            //idarg will be null if no node is returned by the XmlProcessor
            if(idarg == null)
            {
                return new MatteShader();
            }
            else
            {
                int numMats = w.materialList.Count;
                bool foundMat = false;

                int matIndex=0;
                for(int i = 0;i<numMats;i++)
                {
                    if(w.materialList[i].id.Equals(idarg))
                    {
                        foundMat = true;
                        matIndex = i;
                        break;
                    }
                }

                if(foundMat)
                {
                    return w.materialList[matIndex];
                }
                else
                {
                    return new MatteShader();
                }
            }
        }
    }
}
