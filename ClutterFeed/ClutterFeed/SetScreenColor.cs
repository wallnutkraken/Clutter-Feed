// Thanks to Alex Shvedov for providing this file.
// The original file is in the public domain
// See <https://bitbucket.org/benskolnick/color-console/src>

/*   This file is part of ClutterFeed.
 *
 *    ClutterFeed is free software: you can redistribute it and/or modify
 *    it under the terms of the GNU General Public License as published by
 *    the Free Software Foundation, either version 3 of the License, or
 *    (at your option) any later version.
 *
 *    ClutterFeed is distributed in the hope that it will be useful,
 *    but WITHOUT ANY WARRANTY; without even the implied warranty of
 *    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *    GNU General Public License for more details.
 *
 *    You should have received a copy of the GNU General Public License
 *    along with ClutterFeed. If not, see <http://www.gnu.org/licenses/>.
 */

using System;

namespace ClutterFeed
{
    static class SetScreenColor
    {
        public const double MULTIP_VAL = 3.92156;
        public static Color CursifyColor(Color color)
        {
            color.Blue = Convert.ToInt16(color.Blue * MULTIP_VAL);
            color.Red = Convert.ToInt16(color.Red * MULTIP_VAL);
            color.Green = Convert.ToInt16(color.Green * MULTIP_VAL);
            return color;
        }
    }
}
