namespace ClutterFeed
{
    static class TimerMan
    {
        public static bool Paused { get; set; }
        public static void Pause()
        {
            Program.UpdateTimer.Change(System.Threading.Timeout.Infinite, System.Threading.Timeout.Infinite);
            Paused = true;
        }
        public static void Resume()
        {
            Program.UpdateTimer.Change(0, 1000);
            Paused = false;
        }
    }
}
