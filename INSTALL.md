# NuGet Packages
All you need to compile this is to get the TweetMoaSharp package from Nuget.

# FileNotFoundException: keys.conf does not exist.
This program uses a keys.conf file in the directory where the executable is in to read/store the API keys for the app and the user.

# What is the keys.conf file supposed to look like?
Note: the names are not case sensitive, but the tokens are.

```
appToken=apP-t0k3n
appSecret=aPp-T0k3N-s3kr37
userToken=
userSecret=
```

The app token and secret **_must_** be filled in manually, however initially the user keys should be left blank so you get to authorize with twitter.

# Cannot find notification.wav?
You have to put it in the same directory as the executable