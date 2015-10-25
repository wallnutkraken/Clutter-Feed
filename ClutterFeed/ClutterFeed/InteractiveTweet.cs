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

namespace ClutterFeed
{
    class InteractiveTweet
    {
        public string AuthorName { get; set; }
        public string Contents { get; set; }
        public long ID { get; set; }
        public string TweetIdentification { get; set; }
        public bool IsFavorited { get; set; }
        public bool IsRetweeted { get; set; }
    }
    
}
