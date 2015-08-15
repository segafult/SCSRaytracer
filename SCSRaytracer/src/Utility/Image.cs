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
using SFML.Graphics;
using SFML.System;

namespace RayTracer
{
    class Image
    {
        int rows, cols;
        byte[] pixels;

        public Image()
        {

        }

        public void loadFromFile(string file)
        {
            SFML.Graphics.Image img = new SFML.Graphics.Image(file);
            SFML.System.Vector2u dimensions = img.Size;

            rows = (int)dimensions.Y;
            cols = (int)dimensions.X;

            pixels = new byte[3 * rows * cols];
            for(int r = 0;r < rows; r++)
            {
                for(int c = 0; c < cols; c++)
                {
                    SFML.Graphics.Color color = img.GetPixel((uint)(c), (uint)(rows-r-1));
                    pixels[(cols * r * 3) + (3*c)] = color.R;
                    pixels[(cols * r * 3) + (3*c) + 1] = color.G;
                    pixels[(cols * r * 3) + (3*c) + 2] = color.B;
                }
            }    
        }

        public RGBColor get_color_at_uv(double u, double v, bool interpolate)
        {
            //Convert provided UV coordinates to texel coordinates
            int row0 = (int)((rows-1) * v);
            int col0 = (int)((cols-1) * u);
            double inv255 = 1.0 / 255.0;
            if(row0 < 0 || row0 > rows || col0 < 0 || col0 > cols)
            {
                return new RGBColor(1, 0, 0);
            }
            if(!interpolate)
            {
                //Return the floored texture value
                double r = (double)pixels[(row0 * cols * 3) + (3 * col0)] * inv255;
                double g = (double)pixels[(row0 * cols * 3) + (3 * col0) + 1] *inv255;
                double b = (double)pixels[(row0 * cols * 3) + (3 * col0) + 2] * inv255;
                return new RGBColor(r, g, b);
            }
            else
            {
                //Apply bilinear texture interpolation
                //u
                return new RGBColor(1,1,1);
            }
        }
    }
}
