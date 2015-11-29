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
    /* I know the TwitterUpdate class exists. However, the point */
    /* of this class is that it contains an identifier string, */
    /* something unique to ClutterFeed that twitter's API does not */
    /* offer by itself, and that makes sense. This class is not redundant. */

    class InteractiveTweet
    {
        public string AuthorScreenName { get; set; } /* @Name */
        public string AuthorDisplayName { get; set; } /* Name */
        public string Contents { get; set; } /* The message of the tweet */
        public long ID { get; set; } /* Tweet ID, backend use only */
        public string TweetIdentification { get; set; } /* The identifier for the end user */
        public bool IsFavorited { get; set; } /* Has the user favorited this? */
        public string LinkToTweet { get; set; }
        public bool IsMention { get; set; } = false;
        public int FavoriteCount { get; set; }
        public int RetweetCount { get; set; }
        public DateTime TimePosted { get; set; }
        public bool IsDirectMessage { get; set; }
    }

}
