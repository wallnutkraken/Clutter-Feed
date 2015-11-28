namespace ClutterFeed
{
    static class TimerMan
    {
        public static bool Paused { get; set; }
        public static void Pause()
        {
            Paused = true;
        }
        public static void Resume()
        {
            Paused = false;
        }
    }
}
