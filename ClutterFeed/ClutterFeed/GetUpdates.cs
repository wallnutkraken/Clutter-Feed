using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TweetSharp;

namespace ClutterFeed
{
    class InteractiveTweet
    {
        public string Contents { get; set; }
        public string ID { get; set; }
    }
    class GetUpdates
    {
        public void First()
        {
            User files = new User();

            files.Run();

            OAuthAccessToken userToken = files.GetUser();
            OAuthAccessToken appToken = files.GetApp();

            TwitterService service = new TwitterService(appToken.Token, appToken.TokenSecret);
            service.AuthenticateWith(userToken.Token, userToken.TokenSecret);

            var tweets = service.ListTweetsOnHomeTimeline(new ListTweetsOnHomeTimelineOptions());
            var tweetList = tweets.ToList();
            

            for (int index = 0; index < tweetList.Count; index++)
            {
                
                Console.WriteLine(ConvertTweet(tweetList[index]).Contents);
            }
            Console.ReadKey(true);
        }
        /// <summary>
        /// Converts the TwitterStatus object into a more readily usable InteractiveTweet object
        /// </summary>
        /// <param name="tweet">TwitterStatus object</param>
        /// <returns></returns>
        public InteractiveTweet ConvertTweet(TwitterStatus tweet)
        {
            InteractiveTweet formedTweet = new InteractiveTweet();

            formedTweet.Contents = "@" + tweet.Author.ScreenName + ": " + tweet.Text;
            formedTweet.ID = tweet.IdStr;

            return formedTweet;
        }
    }
}
