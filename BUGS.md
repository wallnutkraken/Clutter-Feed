# **Known bugs**

#### Sometimes, twitter will cut off access until restart
 Also makes the remaining API hits and API hit limit -1. EUREKA! Might be caused by too many actions
happening at once (tweet & refresh). Possible fix is to no longer refresh when tweeting.

#### Mentions are just broken, need lots of work