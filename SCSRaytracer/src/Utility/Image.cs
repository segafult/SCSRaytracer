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
        private readonly float INVTWOFITTYFI = 1.0f / 255.0f;


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

        public RGBColor get_color_at_uv(float u, float v, bool interpolate)
        {
            //Convert provided UV coordinates to texel coordinates
            float rv0 = v * (rows - 1) - 0.5f;
            float cu0 = u * (cols - 1) - 0.5f;
            int row0 = (int)(rv0);
            int col0 = (int)(cu0);

            float r0 = (float)pixels[(row0 * cols * 3) + (3 * col0)] * INVTWOFITTYFI;
            float g0 = (float)pixels[(row0 * cols * 3) + (3 * col0) + 1] * INVTWOFITTYFI;
            float b0 = (float)pixels[(row0 * cols * 3) + (3 * col0) + 2] * INVTWOFITTYFI;

            if(!interpolate)
            {
                //Return the floored texture value
                return new RGBColor(r0, g0, b0);
            }
            else
            {
                //Apply bilinear texture filtering with wraparound
                int row1 = ((row0 + 1) == rows) ? 0 : row0 + 1;
                int col1 = ((col0 + 1) == cols) ? 0 : col0 + 1;

                //Legend:
                //c0 = texture[col0][row0] 
                //c1 = texture[col1][row0]
                //c2 = texture[col0][row1]
                //c3 = texture[col1][row1]
                float r1 = (float)pixels[(row0 * cols * 3) + (3 * col1)] * INVTWOFITTYFI;
                float g1 = (float)pixels[(row0 * cols * 3) + (3 * col1) + 1] * INVTWOFITTYFI;
                float b1 = (float)pixels[(row0 * cols * 3) + (3 * col1) + 2] * INVTWOFITTYFI;
                float r2 = (float)pixels[(row1 * cols * 3) + (3 * col0)] * INVTWOFITTYFI;
                float g2 = (float)pixels[(row1 * cols * 3) + (3 * col0) + 1] * INVTWOFITTYFI;
                float b2 = (float)pixels[(row1 * cols * 3) + (3 * col0) + 2] * INVTWOFITTYFI;
                float r3 = (float)pixels[(row1 * cols * 3) + (3 * col1)] * INVTWOFITTYFI;
                float g3 = (float)pixels[(row1 * cols * 3) + (3 * col1) + 1] * INVTWOFITTYFI;
                float b3 = (float)pixels[(row1 * cols * 3) + (3 * col1) + 2] * INVTWOFITTYFI;

                //Pixel color ratios:
                float ratio_col_high = cu0 - col0;
                float ratio_row_high = rv0 - row0;
                float ratio_col_low = 1.0f - ratio_col_high;
                float ratio_row_low = 1.0f - ratio_row_high;

                //Compute ratios for each color
                float rf = (r0 * ratio_col_low + r1 * ratio_col_high) * ratio_row_low +
                    (r2 * ratio_col_low + r3 * ratio_col_high) * ratio_row_high;
                float gf = (g0 * ratio_col_low + g1 * ratio_col_high) * ratio_row_low +
                    (g2 * ratio_col_low + g3 * ratio_col_high) * ratio_row_high;
                float bf = (b0 * ratio_col_low + b1 * ratio_col_high) * ratio_row_low +
                    (b2 * ratio_col_low + b3 * ratio_col_high) * ratio_row_high;

                return new RGBColor(rf, gf, bf);
            }
        }
    }
}
