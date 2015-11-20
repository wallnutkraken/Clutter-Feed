﻿namespace ClutterFeed
{
    static class TimerMan
    {
        public static void Pause()
        {
            Program.UpdateTimer.Change(System.Threading.Timeout.Infinite, System.Threading.Timeout.Infinite);
        }
        public static void Resume()
        {
            Program.UpdateTimer.Change(0, 1000);
        }
    }
}