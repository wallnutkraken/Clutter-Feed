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
using System.IO;
using TweetSharp;
using System.Diagnostics;
using CursesSharp;

namespace ClutterFeed
{
    class User
    {
        public static TwitterService Account;

        public static List<Profile> profiles = new List<Profile>();
        public static OAuthAccessToken appKey = new OAuthAccessToken();
        private List<string> ConfigFile;
        public static Window CounterConsoleWin;
        private const string CONFIG = "clutterfeed.conf";

        public void GetConfigs()
        {
            ReadConfig();
            foreach (string line in ConfigFile)
            {
                if (line.ToLower().Contains("refresh") && line.StartsWith("#") == false)
                {
                    Settings.RefreshSeconds = int.Parse(line.Split('=')[1]);
                }
                if (line.ToLower().Contains("nosquash") && line.StartsWith("#") == false)
                {
                    if (line.Split('=')[1].ToLower().CompareTo("true") == 0)
                    {
                        Settings.NoSquash = true;
                    }
                }
                if (line.ToLower().Contains("noshortcuts") && line.StartsWith("#") == false)
                {
                    if (line.Split('=')[1].ToLower().CompareTo("true") == 0)
                    {
                        Settings.NoShortcuts = true;
                    }
                }
            }
            if (Settings.RefreshSeconds == 0)
            {
                Settings.RefreshSeconds = 300;
            }
        }

        /// <summary>
        /// Checks if all the files are in order
        /// </summary>
        private void ReadKeys()
        {
            if (!File.Exists("keys.conf"))
            {
                throw new FileNotFoundException("keys.conf does not exist.");
            }
            List<string> readAPIKeys = File.ReadAllLines("keys.conf").ToList();

            int tempIndex = 0;
            while (tempIndex < readAPIKeys.Count) /* Removes empty lines */
            {
                if (readAPIKeys[tempIndex].Trim(' ').CompareTo("") == 0)
                {
                    readAPIKeys.RemoveAt(tempIndex);
                }
                else
                {
                    tempIndex++;
                }
            }

            bool appTokenExists = false;
            bool appSecretExists = false;

            for (int index = 0; index < readAPIKeys.Count; index++)
            {
                string[] splitter = readAPIKeys[index].Split('=');
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
                string[] profileSplitter = readAPIKeys[index].Split(' ');
                Profile newProfile = new Profile();
                if (profileSplitter[0].InsensitiveCompare("default")) /* Finds the default user */
                {
                    newProfile.Default = true;
                    newProfile.Active = true;
                }
                if (profileSplitter[0].InsensitiveCompare("default") || profileSplitter[0].InsensitiveCompare("profile"))
                { /* Need to look for both so we can get both */
                    newProfile.Name = profileSplitter[1];
                    do
                    {
                        splitter = readAPIKeys[index].Split('=');

                        if (splitter[0].ToLower().CompareTo("usertoken") == 0)
                        {
                            newProfile.UserKey = splitter[1];
                        }
                        if (splitter[0].ToLower().CompareTo("usersecret") == 0)
                        {
                            newProfile.UserSecret = splitter[1];
                        }

                        index++;
                    } while (readAPIKeys[index].Trim(' ').CompareTo("}") != 0);

                    profiles.Add(newProfile);
                }
            }

            if (profiles.Count == 0)
            {
                CreateUser();
            }

            if ((appTokenExists == false) || (appSecretExists == false))
            {
                throw new FileLoadException("keys.conf is set up improperly.");
            }

        }
        private void ReadConfig()
        {
            if (ConfigFile == null)
            {
                ConfigFile = File.ReadAllLines(CONFIG).ToList();
            }
        }
        public static bool ConfigExists()
        {
            return File.Exists(CONFIG);
        }
        public bool ColorsDefined()
        {
            ReadConfig();
            foreach (string line in ConfigFile)
            {
                if (line.InsensitiveCompare("COLORS"))
                {
                    return true;
                }
            }
            return false;
        }
        public void FindColors()
        {
            ReadConfig();
            for (int index = 0; index < ConfigFile.Count; index++)
            {
                if (ConfigFile[index].InsensitiveCompare("colors"))
                {
                    index = index + 2;
                    while (ConfigFile[index].CompareTo("}") != 0)
                    {
                        string[] splitLine = ConfigFile[index].Split('=');
                        if (splitLine[0].InsensitiveCompare("identifiercolor") && ConfigFile[index].StartsWith("#") == false)
                        {
                            string[] colorSetting = splitLine[1].Split(',');
                            short red = short.Parse(colorSetting[0]);
                            short green = short.Parse(colorSetting[1]);
                            short blue = short.Parse(colorSetting[2]);
                            Color.IdentifierColor = SetScreenColor.CursifyColor(new Color(red, green, blue));
                        }
                        else if (splitLine[0].InsensitiveCompare("linkcolor") && ConfigFile[index].StartsWith("#") == false)
                        {
                            string[] colorSetting = splitLine[1].Split(',');
                            short red = short.Parse(colorSetting[0]);
                            short green = short.Parse(colorSetting[1]);
                            short blue = short.Parse(colorSetting[2]);
                            Color.LinkColor = SetScreenColor.CursifyColor(new Color(red, green, blue));
                        }
                        else if (splitLine[0].InsensitiveCompare("friendcolor") && ConfigFile[index].StartsWith("#") == false)
                        {
                            string[] colorSetting = splitLine[1].Split(',');
                            short red = short.Parse(colorSetting[0]);
                            short green = short.Parse(colorSetting[1]);
                            short blue = short.Parse(colorSetting[2]);
                            Color.FriendColor = SetScreenColor.CursifyColor(new Color(red, green, blue));
                        }
                        else if (splitLine[0].InsensitiveCompare("selfcolor") && ConfigFile[index].StartsWith("#") == false)
                        {
                            string[] colorSetting = splitLine[1].Split(',');
                            short red = short.Parse(colorSetting[0]);
                            short green = short.Parse(colorSetting[1]);
                            short blue = short.Parse(colorSetting[2]);
                            Color.SelfColor = SetScreenColor.CursifyColor(new Color(red, green, blue));
                        }
                        else if (splitLine[0].InsensitiveCompare("mentioncolor") && ConfigFile[index].StartsWith("#") == false)
                        {
                            string[] colorSetting = splitLine[1].Split(',');
                            short red = short.Parse(colorSetting[0]);
                            short green = short.Parse(colorSetting[1]);
                            short blue = short.Parse(colorSetting[2]);
                            Color.MentionColor = SetScreenColor.CursifyColor(new Color(red, green, blue));
                        }
                        index++;
                    }
                }
            }
            SetUnsetColorsToDefaults();
        }

        /// <summary>
        /// Sets the colors that weren't set to defaults
        /// </summary>
        public static void SetUnsetColorsToDefaults()
        {
            if (Color.IdentifierColor == null)
            {
                Color.IdentifierColor = SetScreenColor.CursifyColor(new Color(0, 126, 199));
            }
            if (Color.LinkColor == null)
            {
                Color.LinkColor = SetScreenColor.CursifyColor(new Color(66, 140, 187));
            }
            if (Color.FriendColor == null)
            {
                Color.FriendColor = SetScreenColor.CursifyColor(new Color(249, 129, 245));
            }
            if (Color.SelfColor == null)
            {
                Color.SelfColor = SetScreenColor.CursifyColor(new Color(225, 165, 0));
            }
            if (Color.MentionColor == null)
            {
                Color.MentionColor = SetScreenColor.CursifyColor(new Color(236, 183, 9));
            }
        }

        /// <summary>
        /// Deactivates current active user and activates the selected user
        /// </summary>
        /// <param name="name">Profile name of the user</param>
        public void SelectUser(string name)
        {
            foreach (Profile user in profiles)
            {
                if (user.Active)
                {
                    user.Active = false;
                }
                if (user.Name.InsensitiveCompare(name))
                {
                    user.Active = true;
                }
            }
            GetUpdates.ReauthenticateTwitter();
        }
        /// <summary>
        /// Creates a user using OAuth
        /// </summary>
        private void CreateUser()
        {
            OAuthAccessToken userKey = new OAuthAccessToken();
            Profile defaultProfile = new Profile();
            TwitterService service = new TwitterService(appKey.Token, appKey.TokenSecret);
            OAuthRequestToken requestToken = service.GetRequestToken();
            Uri uri = service.GetAuthorizationUri(requestToken);
            Process.Start(uri.ToString());
            service.AuthenticateWith(appKey.Token, appKey.TokenSecret);

            Window auth = new Window(1, ScreenInfo.WindowWidth, 0, 0);
            Curses.Echo = true;
            auth.Add("Please input the authentication number: ");
            auth.Refresh();
            string verifier = auth.GetString(7);
            userKey = service.GetAccessToken(requestToken, verifier);
            defaultProfile.Active = true;
            defaultProfile.Default = true;
            defaultProfile.UserKey = userKey.Token;
            defaultProfile.UserSecret = userKey.TokenSecret;
            defaultProfile.Name = userKey.ScreenName;

            profiles.Add(defaultProfile);
            Curses.Echo = false;
            auth.Dispose();
            WriteFile();
        }

        private bool DefaultExists()
        {
            foreach (Profile user in profiles)
            {
                if (user.Default == true)
                {
                    return true;
                }
            }
            return false;
        }

        public void AddProfile()
        {
            ScreenDraw.Tweets.Clear();
            ScreenDraw.Tweets.Refresh();

            OAuthAccessToken userKey = new OAuthAccessToken();
            OAuthRequestToken requestToken = User.Account.GetRequestToken();
            Window auth = new Window(1, ScreenInfo.WindowWidth, 3, 0);
            Uri uri = User.Account.GetAuthorizationUri(requestToken);
            Process.Start(uri.ToString());

            Curses.Echo = true;

            auth.Add("Please input the authentication number: ");
            auth.Refresh();
            string verifier = auth.GetString(7);

            userKey = User.Account.GetAccessToken(requestToken, verifier);
            Profile newProfile = new Profile();

            newProfile.Name = userKey.ScreenName;
            newProfile.UserKey = userKey.Token;
            newProfile.UserSecret = userKey.TokenSecret;

            if (ProfileExists(newProfile))
            {
                ScreenDraw.ShowMessage("User already exists in the list");
            }
            else
            {
                profiles.Add(newProfile);
                WriteFile();
                ScreenDraw.ShowMessage("User added");
            }
            Curses.Echo = false;
            auth.Dispose();

            ScreenDraw draw = new ScreenDraw();
            draw.ShowTimeline();
        }

        /// <summary>
        /// Removes a profile by name and saves changes to file
        /// </summary>
        /// <param name="name"></param>
        public void RemoveProfile(Profile name)
        {
            profiles.Remove(name);
            WriteFile();
        }

        public void SetDefault(Profile user)
        {
            foreach (Profile account in profiles)
            {
                if (account.Default)
                {
                    account.Default = false;
                    account.Active = false;
                }
            }

            foreach (Profile account in profiles)
            {
                if (account.Name.InsensitiveCompare(user.Name))
                {
                    account.Active = true;
                    account.Default = true;
                }
            }
            WriteFile();
        }

        /// <summary>
        /// Writes the profiles and app key info to file
        /// </summary>
        private void WriteFile()
        {
            List<string> newFile = new List<string>();
            newFile.Add("appToken=" + appKey.Token);
            newFile.Add("appSecret=" + appKey.TokenSecret);
            foreach (Profile user in profiles)
            {
                if (user.Default)
                {
                    newFile.Add("DEFAULT " + user.Name);
                }
                else
                {
                    newFile.Add("PROFILE " + user.Name);
                }
                newFile.Add("{");
                newFile.Add("userToken=" + user.UserKey);
                newFile.Add("userSecret=" + user.UserSecret);
                newFile.Add("}");
            }
            File.WriteAllLines(Environment.CurrentDirectory + "/keys.conf", newFile);
        }
        public void Run()
        {
            ReadKeys();
        }

        public OAuthAccessToken GetUser()
        {
            if (DefaultExists())
            {
                foreach (Profile user in profiles)
                {
                    if (user.Active)
                    {
                        OAuthAccessToken token = new OAuthAccessToken();
                        token.Token = user.UserKey;
                        token.TokenSecret = user.UserSecret;
                        token.ScreenName = user.Name;
                        WriteFile();

                        return token;
                    }
                }
                return null;
            }
            else
            {
                profiles[0].Default = true;
                profiles[0].Active = true;
                OAuthAccessToken token = new OAuthAccessToken();
                token.Token = profiles[0].UserKey;
                token.TokenSecret = profiles[0].UserSecret;
                token.ScreenName = profiles[0].Name;

                return token;
            }
        }

        public OAuthAccessToken GetApp()
        {
            return appKey;
        }

        public static bool IsMissingArgs(string command)
        {
            try
            {
                bool checker = command.Split(' ')[1].Length != 2;
            }
            catch (IndexOutOfRangeException)
            {
                Window argsMiss = new Window(1, ScreenInfo.WindowWidth, 0, 0);
                argsMiss.Add("      Error: input was not complete.\n");
                argsMiss.Add("      You probaby didn't use enough args");
                argsMiss.Refresh();
                argsMiss.GetChar();
                argsMiss.Dispose();
                return true;
            }
            return false;
        }

        /// <summary>
        /// Turns a number into the string I want
        /// </summary>
        private static string Numberate(int number)
        {
            if (number < 10)
                return "00" + number;
            else if (number < 100)
                return "0" + number;
            else
                return number + "";
        }
        private static Window DrawConsoleNum(Window cmdWindow, int charCount)
        {
            cmdWindow.Color = 11;
            cmdWindow.Add("[");
            if (charCount > 140)
            {
                cmdWindow.Color = Colors.RED;
            }
            else
            {
                cmdWindow.Color = Colors.WHITE;
            }
            cmdWindow.Add(Numberate(charCount));
            cmdWindow.Color = 11;
            cmdWindow.Add("] > ");
            cmdWindow.Color = Colors.WHITE;

            return cmdWindow;
        }

        public static string CounterConsole()
        {
            CounterConsoleWin = new Window(2, ScreenInfo.WindowWidth, ScreenInfo.WindowHeight - 2, 0);
            CounterConsoleWin.Keypad = true;

            int splitCount = 3;
            string command = "";
            string message = "";
            int charCount = 0;
            int buttonPress = 0;
            int bufferPosition = 0;
            char[] splitter = { ' ' };

            do
            {
                CounterConsoleWin.Clear();

                if (command.StartsWith("/"))
                {
                    try
                    {
                        message = command.Split(splitter, splitCount)[2];
                        CounterConsoleWin = DrawConsoleNum(CounterConsoleWin, message.Length);
                    }
                    catch (IndexOutOfRangeException)
                    {
                        CounterConsoleWin = DrawConsoleNum(CounterConsoleWin, message.Length);
                    }
                }
                else
                {
                    CounterConsoleWin = DrawConsoleNum(CounterConsoleWin, charCount);
                }

                CounterConsoleWin.Add(command);
                CounterConsoleWin.Refresh();

                buttonPress = CounterConsoleWin.GetChar();
                if (Settings.NoShortcuts == false)
                {
                    Actions.DealWithShortcuts(buttonPress);
                }

                if (buttonPress == 8) /* 8 is backspace */
                {
                    if (charCount != 0)
                    {
                        command = command.Remove(command.Length - 1, 1);
                        charCount--;
                    }
                    else
                    {
                        Curses.Beep();
                    }
                }
                else
                {
                    if (buttonPress == Keys.DOWN)
                    {
                        if (bufferPosition == 0) /* Nothing happens if you're already at the latest command possible */
                        {
                            Curses.Beep();
                        }
                        else
                        {
                            try
                            {
                                bufferPosition--;
                                command = CommandHistory.GetCommand(bufferPosition);
                                charCount = command.Length;
                            }
                            catch (ArgumentOutOfRangeException)
                            {
                                Curses.Beep();
                            }
                        }
                    }
                    else if (buttonPress == Keys.UP) /* Handles going to earlier points in the history */
                    {
                        if (bufferPosition == 0)
                        {
                            if (CommandHistory.MaxIndex() != bufferPosition || CommandHistory.MaxIndex() == 0)
                            {
                                try
                                {
                                    CommandHistory.Add(command);
                                    bufferPosition++;
                                    command = CommandHistory.GetCommand(bufferPosition);
                                    charCount = command.Length;
                                }
                                catch (ArgumentOutOfRangeException)
                                {
                                    Curses.Beep();
                                }
                            }
                            else
                            {
                                Curses.Beep();
                            }
                        }
                        else if (bufferPosition == CommandHistory.MaxIndex())
                        {
                            Curses.Beep();
                        }
                        else
                        {
                            try
                            {
                                bufferPosition++;
                                command = CommandHistory.GetCommand(bufferPosition);
                                charCount = command.Length;
                            }
                            catch (ArgumentOutOfRangeException)
                            {
                                Curses.Beep();
                            }
                        }
                    }
                    else if (buttonPress == Keys.RIGHT
                        || buttonPress == Keys.LEFT)
                    {
                        /* Ignores left and right arrow key currently */
                        /* but one day I hope you could move in the command */
                    }
                    else if (buttonPress == 10 || buttonPress == Keys.ENTER) /* 10 is return */
                    {
                        buttonPress = int.MinValue;
                    }
                    else if (charCount < 146)
                    {
                        if (buttonPress < 57344)
                        {
                            command = command + Convert.ToChar(buttonPress);
                            try
                            {
                                message = command.Split(splitter, splitCount)[2];
                            }
                            catch (IndexOutOfRangeException) { }
                            charCount++;
                        }
                    }
                }


            } while (buttonPress != int.MinValue);

            CommandHistory.Add(command);
            CommandHistory.RemoveEmpties();

            return command;
        }

        /// <summary>
        /// Checks if the profile exists in the list
        /// </summary>
        private bool ProfileExists(Profile user)
        {
            foreach (Profile item in profiles)
            {
                if (item.UserKey.CompareTo(user.UserKey) == 0)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
