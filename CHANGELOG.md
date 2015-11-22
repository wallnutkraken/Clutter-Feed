# **Release change log**

### Changes in Version 1.5.1:
- Timer now pauses when you're not on the timeline
- Raised the limit of how many tweets are initially shown on the timeline
- Added keyboard shortcuts:
	- Ctrl - C Closes ClutterFeed
	- Ctrl - U Updates
	- Ctrl - D Goes to mentions
- Changed the design of profiles and other popups
- More links now shown as intended
- Added an /afk command to pause/resume the timer along with a config file line to enable AFK mode on startup
- Fixed the mention and self colors being pretty much the same
-  Fixed some html special characters not escaping properly
- Fixed tweets getting cut off mid word


### Changes in Version 1.5:
- Added a configuration file
- Added support for custom colors with the config file
- Made the author's @name in a tweet be bold for readability
- Readded /follow ad /block
- Added twitter error messages 
- Fixed a bug that caused ClutterFeed to crash if you start it while API limited
- Fixed a bug where > and < were not escaped properly
- Fixed a bug with lines being split weirdly

### Changes in Version 1.4:
- Added support for multiple twitter accounts
- Added a timed update, to update tweets every 5 minutes
- Ported over the entire project to a curses library:
	- No longer limited by cmd
	- Better color support
	- Changed the look of ClutterFeed in some areas
- Fixed a bug that wouldn't remove profiles correctly
- Fixed a bug that would make ClutterFeed unable to start if you deleted the default profile from within ClutterFeed
- Changed the beeps to not be spammable

### Changes in Version 1.3:
- Added command history feature for the command console (use up and down keys to use)
- Added a /friend command which lets you set users as friends, and their tweet identifier would show up in a different color than regular users' identifiers
- Added a /tweet command which shows the selected tweet and more information about it
- Added an /open command which opens the selected tweet in the browser (only used in the /tweet command)
- Added an official sound for notifications
- Added a reply clean command which only includes the @name of the user you're replying to
- Changed the link and Tweets/Following/Followers colors on /profile
- Fixed a bug that would cause a crash when looking up the profile of an non-existant user
- Fixed a bug where the /rn command would only tweet the first word

### Changes in Version 1.2:
- You can now reply, favorite and retweet in the mention screen
- Tweets are now cached until a full update is made
- Added a /link command to give you the link to a tweet
- Added a character counter to the command line
- The reply (/r) option now replies to everyone in the mentioned tweet as well as the author
- Disabled unretweeting tweets for now
- Lots of bugfixes

### Version 1.1:
- Fixed bug that tweeted the entire command when trying to reply
- Changed the way ClutterFeed stores Twitter updates
- @notifications will now only notify you via sound when there is a new notification or a full update is made
- Fixed a bug that would make the command line sometimes be misplaces in mentions
- Added support for a custom notification sound
- Fixed a bug that would cause ClutterFeed to not unretweet tweets, despite saying it did
- Reworked the way commands are handled
- Removed the /new and /n commands (to tweet now you just type in your tweet)

### Version 1.0
Initial public release.
These features were included:

- Getting the timeline.
- Showing which tweets are favorited/retweeted by you.
- Retweeting and undoing retweets.
- API limit information showing
- Tweeting.
- Replying with or without using @mentions.
- Showing @mentions.
- Searching for profiles.
- Looking at profiles for every tweet.
-A help prompt.