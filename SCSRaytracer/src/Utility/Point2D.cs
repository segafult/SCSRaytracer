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

namespace RayTracer
{
    class Point2D
    {
        public float x;
        public float y;

        public Point2D()
        {
            x = 0.0f;
            y = 0.0f;
        }
        public Point2D(float xcoord, float ycoord)
        {
            x = xcoord;
            y = ycoord;
        }
        public Point2D(Point2D p)
        {
            x = p.x;
            y = p.y;
        }
    }
}
