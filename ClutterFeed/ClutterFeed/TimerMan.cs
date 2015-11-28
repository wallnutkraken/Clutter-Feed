namespace ClutterFeed
{
    static class TimerMan
    {
        public static bool Paused { get; set; }
        public static void Pause()
        {
            Paused = true;
            ScreenDraw.UpdateHeader();
        }
        public static void Resume()
        {
            Paused = false;
            ScreenDraw.UpdateHeader();
        }
    }
}
