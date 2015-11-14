# Windows
### 1. Dependencies
ClutterFeed depends on TweetMoaSharp and a very specific variant of CursesSharp that you will have to also compile yourself. But first, make sure you have the `TweetMoaSharp` package installed in the project via NuGet.

To compile CursesSharp, you will first need to get a fork of [pdcurses](http://www.projectpluto.com/win32a.htm). To compile it, open Visual Studio x86 Native Compile tools. Then go to the win32a directory in pdcurses, and run
``` nmake -f vcwin32.mak WIDE=Y ```

Get the source code for [CursesSharp](http://sourceforge.net/projects/curses-sharp/files/curses-sharp/0.8/curses-sharp-0.8.tar.gz/download). Make a pdcurses directory where you extracted CursesSharp, and put pdcurses.lib from win32a in there, as well as curses.h, panel.h and term.h from where you extracted pdcurses.

Then, open CursesSharp with Visual Studio and build CursesSharp.dll and CursesWrapper.dll

### 2. Building ClutterFeed
Make sure you have downloaded the TweetMoaSharp package and referenced the CursesSharp.dll in the project. Note: You only need to add CursesSharp.dll as a reference, but CursesWrapper.dll has to be in the same directory as ClutterFeed.exe

# FileNotFoundException: keys.conf does not exist.
This program uses a keys.conf file in the directory where the executable is in to read/store the API keys for the app and the user.

# What is the keys.conf file supposed to look like?
The part of the file to be created by the user is as simple as this:

```
appToken=apP-t0k3n
appSecret=aPp-T0k3N-s3kr37
```

And once you run ClutterFeed, it will automatically ask you to authenticate with twitter and once that is done, it will fill in the user keys

# Cannot find notification.wav?
You have to put it in the same directory as the executable