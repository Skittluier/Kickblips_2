namespace KickblipsTwo.Platform
{
    using KickblipsTwo.Platform.Platforms;
    using System;

    public static class PlatformHandler
    {
        // The local one always works.
        private static LocalPlatform localPlatform = new LocalPlatform();


        /// <summary>
        /// Sets the highscore for a specific song.
        /// </summary>
        /// <param name="songName">The name of the song</param>
        /// <param name="isSuccess">Did setting the highscore go well?</param>
        internal static void SetHighscore(string songName, int score, Action<bool> isSuccess)
        {
            localPlatform.SetHighscore(songName, score);

            // For now it's always true.
            isSuccess?.Invoke(true);
        }

        /// <summary>
        /// Fetches the highscore for a specific song.
        /// </summary>
        /// <param name="songName">The name of the song</param>
        /// <returns>The score itself</returns>
        internal static int GetHighscore(string songName)
        {
            return localPlatform.GetHighscore(songName);
        }
    }
}