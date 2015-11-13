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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TweetSharp;

namespace ClutterFeed
{
    class StatusCommunication
    {
        /// <summary>
        /// A method to post a tweet
        /// </summary>
        /// <param name="twitterAccess">Twitter Service API object to access the API</param>
        /// <param name="command">String to tweet</param>
        public void PostTweet(TwitterService twitterAccess, string command)
        {
            SendTweetOptions options = new SendTweetOptions();
            options.Status = command;
            twitterAccess.BeginSendTweet(options);
        }
        public void ShowUpdates(TwitterService twitterAccess, GetUpdates showUpdates, bool fullUpdate)
        {

            showUpdates.GetTweets(fullUpdate);
            ScreenDraw timeline = new ScreenDraw();
            timeline.ShowTimeline();
        }
    }
}
