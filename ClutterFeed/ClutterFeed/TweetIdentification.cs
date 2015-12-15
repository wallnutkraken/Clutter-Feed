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
using TweetSharp;

namespace ClutterFeed
{
    class TweetIdentification
    {
        private bool IdentificationExists(string identity)
        {
            List<InteractiveTweet> existingTweets = GetUpdates.localTweetList;
            bool foundMatch = false;
            try
            {
                bool tryTest = existingTweets[0] == null;
            }
            catch (Exception)
            {
                return false;
            }

            for (int index = 0; index < existingTweets.Count; index++)
            {
                if (existingTweets[index].TweetIdentification == identity)
                {
                    foundMatch = true;
                }
            }
            return foundMatch;
        }
        /// <summary>
        /// Generates a two character identification
        /// </summary>
        /// <returns></returns>
        public string GenerateIdentification()
        {
            const string chars = "abcdefghijklmnopqrstuvwxyz0123456789";
            string randomIdentifier = "";
            do
            {
                randomIdentifier = new string(Enumerable.Repeat(chars, 2).Select(s => s[RandomGenerator.GetRandomNumber(0, s.Length)]).ToArray());
            } while (IdentificationExists(randomIdentifier) == true);
            return randomIdentifier;
        }

        public static SendTweetOptions GetTweetID(string identifier)
        {
            SendTweetOptions found = new SendTweetOptions();
            List<InteractiveTweet> tweetList = GetUpdates.localTweetList;
            for (int index = 0; index < tweetList.Count; index++)
            {
                if (tweetList[index].TweetIdentification.CompareTo(identifier) == 0)
                {
                    found.InReplyToStatusId = tweetList[index].ID;
                    string atName = tweetList[index].AuthorScreenName;
                    found.Status = atName; /* @name */
                }
            }
            return found;
        }
        /// <summary>
        /// Finds the unformatted tweet by ID
        /// </summary>
        /// <param name="tweetID"></param>
        /// <returns></returns>
        public static InteractiveTweet FindTweet(long tweetID)
        {
            List<InteractiveTweet> tweetList = GetUpdates.localTweetList;
            for (int index = 0; index < tweetList.Count; index++)
            {
                if (tweetList[index].ID == tweetID)
                {
                    return tweetList[index];
                }
            }
            throw new KeyNotFoundException("A tweet with such an ID does not exist.");
        }

        public static int NewTweetStart(List<TwitterStatus> tweets)
        {
            int firstOldTweet = int.MinValue;
            List<InteractiveTweet> cachedTweets = GetUpdates.localTweetList;
            try
            {
                for (int index = 0; index < tweets.Count; index++)
                {
                    if (tweets[index].Id == cachedTweets[0].ID)
                    {
                        firstOldTweet = index;
                    }
                }
            }
            catch (NullReferenceException)
            {
                return 0;
            }

            if (firstOldTweet != int.MinValue)
            {
                return firstOldTweet;
            }
            return -1;
        }

    }
}
