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
        public static List<InteractiveTweet> updateTweets;
        public void ShowUpdates(TwitterService twitterAccess, GetUpdates showUpdates, bool fullUpdate)
        {
            if (fullUpdate)
            {
                updateTweets = showUpdates.GetTweets(twitterAccess);
                if (updateTweets == null)
                {
                    throw new NullReferenceException("No tweets gotten");
                }
            }
            else
            {
                updateTweets = showUpdates.UpdateTweets(twitterAccess);
                if(updateTweets == null)
                {
                    throw new NullReferenceException("No tweets gotten");
                }
            }
            Console.Clear();
            ScreenDraw.ShowTimeline(updateTweets);
        }
    }
}
