using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using TweetSharp;
using System.Diagnostics;

namespace ClutterFeed
{
    class User
    {
        OAuthAccessToken userKey = new OAuthAccessToken();
        OAuthAccessToken appKey = new OAuthAccessToken();
        /// <summary>
        /// Checks if all the files are in order
        /// </summary>
        private void ReadKeys()
        {
            string userDir = Environment.CurrentDirectory;
            if (!File.Exists(userDir + "/keys.conf"))
            {
                Console.WriteLine("Error: keys.conf does not exist.");
                Environment.Exit(0);
            }
            List<string> readAPIKeys = File.ReadAllLines(userDir + "/keys.conf").ToList();

            //for (int index = 0; index < readAPIKeys.Count; index++) /* Removes empty lines */
            //{
            //    if (readAPIKeys[index] == "")
            //    {
            //        readAPIKeys.RemoveAt(index);
            //    }
            //}

            if (readAPIKeys.Count != 4)
            {
                Console.WriteLine("Bad keys.conf file.");
                Environment.Exit(0);
            }

            bool appTokenExists = false;
            bool appSecretExists = false;
            bool userTokenExists = false;
            bool userSecretExists = false;

            for (int index = 0; index < 4; index++)
            {
                string[] splitter = new string[1];
                splitter = readAPIKeys[index].Split('=');
                if (splitter[0].ToLower().CompareTo("apptoken") == 0)
                {
                    appTokenExists = true;
                    appKey.Token = splitter[1];
                }
                if (splitter[0].ToLower().CompareTo("appsecret") == 0)
                {
                    appSecretExists = true;
                    appKey.TokenSecret = splitter[1];
                }
                if (splitter[0].ToLower().CompareTo("usertoken") == 0)
                {
                    userTokenExists = true;
                    userKey.Token = splitter[1];
                }
                if (splitter[0].ToLower().CompareTo("usersecret") == 0)
                {
                    userSecretExists = true;
                    userKey.TokenSecret = splitter[1];
                }
            }

            if ((userKey.Token == "") || (userKey.TokenSecret == ""))
            {
                CreateUser();
            }

            if ((appTokenExists == false) || (appSecretExists == false) || (userTokenExists == false) || (userSecretExists == false))
            {
                Console.WriteLine("Error: keys.conf is set up improperly.");
                Environment.Exit(0);
            }

        }
        /// <summary>
        /// Creates a user using OAuth
        /// </summary>
        private void CreateUser()
        {
            TwitterService service = new TwitterService(appKey.Token, appKey.TokenSecret);
            OAuthRequestToken requestToken = service.GetRequestToken();
            Uri uri = service.GetAuthorizationUri(requestToken);
            Process.Start(uri.ToString());
            service.AuthenticateWith(appKey.Token, appKey.TokenSecret);

            Console.Write("Please input the PIN: ");
            string verifier = Console.ReadLine();
            userKey = service.GetAccessToken(requestToken, verifier);

            WriteFile();
        }
        private void WriteFile()
        {
            List<string> newFile = new List<string>();
            newFile.Add("appToken=" + appKey.Token);
            newFile.Add("appSecret=" + appKey.TokenSecret);
            newFile.Add("userToken=" + userKey.Token);
            newFile.Add("userSecret=" + userKey.TokenSecret);
            File.WriteAllLines(Environment.CurrentDirectory + "/keys.conf", newFile);
        }
        public void Run()
        {
            ReadKeys();
        }

        public OAuthAccessToken GetUser()
        {
            return userKey;
        }

        public OAuthAccessToken GetApp()
        {
            return appKey;
        }
    }
}
