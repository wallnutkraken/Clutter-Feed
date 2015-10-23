using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TweetSharp;
using System.Threading;

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
                if (existingTweets[0] == null) ;
            }
            catch(Exception)
            {
                return false;
            }

            for(int index = 0; index < existingTweets.Count; index++)
            {
                if(existingTweets[index].TweetIdentification == identity)
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
            var random = new Random();
            string randomIdentifier = "";
            do
            {
                randomIdentifier = new string(Enumerable.Repeat(chars, 2).Select(s => s[random.Next(s.Length)]).ToArray());
            } while (IdentificationExists(randomIdentifier) == true);
            return randomIdentifier;
        }

        public static SendTweetOptions GetTweetID(string identifier)
        {
            SendTweetOptions found = new SendTweetOptions();
            List<InteractiveTweet> tweetList = GetUpdates.localTweetList;
            for(int index = 0; index < tweetList.Count; index++)
            {
                if(tweetList[index].TweetIdentification.CompareTo(identifier) == 0)
                {
                    found.InReplyToStatusId = tweetList[index].ID;
                    string atName = tweetList[index].AuthorName;
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
        public static TwitterStatus FindTweet(long tweetID)
        {
            List<TwitterStatus> tweetList = GetUpdates.unformattedLocalTweetList;
            for(int index = 0; index < tweetList.Count; index++)
            {
                if(tweetList[index].Id == tweetID)
                {
                    return tweetList[index];
                }
            }
            throw new KeyNotFoundException("A tweet with such an ID does not exist.");
        }
    }
}
