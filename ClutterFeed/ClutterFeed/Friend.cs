using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace ClutterFeed
{
    class Friend
    {
        private const string FileName = "friends.list";
        public static List<string> FriendsList;
        public void ReadFriends()
        {
            if (File.Exists(FileName))
            {
                FriendsList = File.ReadAllLines(FileName).ToList();
            }
        }

        /// <summary>
        /// Checks if the friend exists, and then act accordingly
        /// </summary>
        /// <param name="screenName"></param>
        public void FriendToggle(string screenName)
        {
            if (FriendsList != null && FriendsList.Contains(screenName.ToLower()))
            {
                RemoveFriend(screenName);
            }
            else
            {
                AddFriend(screenName);
            }
        }

        private void RemoveFriend(string screenName)
        {
            FriendsList.Remove(screenName.ToLower());
        }
        private void AddFriend(string screenName)
        {
            screenName = screenName.Remove(0, 1);
            StreamWriter addFriend = new StreamWriter(FileName);
            addFriend.WriteLine(screenName.ToLower());
            addFriend.Close();
        }
    }
}
