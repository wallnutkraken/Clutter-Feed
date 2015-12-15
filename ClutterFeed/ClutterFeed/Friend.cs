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

using System.Collections.Generic;
using System.Linq;
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
            else
            {
                FriendsList = new List<string>();
            }
        }

        /// <summary>
        /// Checks if the friend exists, and then act accordingly
        /// </summary>
        /// <param name="screenName"></param>
        public void FriendToggle(string screenName)
        {
            string cleanName = screenName.ToLower().Remove(0, 1);
            if (FriendsList != null && FriendsList.Contains(cleanName))
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
            screenName = screenName.Remove(0, 1); /* Removes the @ */
            FriendsList.Remove(screenName.ToLower());
            if (FriendsList.Count == 0)
            {
                File.Delete(FileName); /* Deletes the file if the friends list becomes empty */
            }
            else
            {
                File.WriteAllLines(FileName, FriendsList);
            }
        }
        private void AddFriend(string screenName)
        {
            screenName = screenName.Remove(0, 1);
            FriendsList.Add(screenName.ToLower());
            File.WriteAllLines(FileName, FriendsList);

            ReadFriends(); /* Reads the file again */
        }
    }
}
