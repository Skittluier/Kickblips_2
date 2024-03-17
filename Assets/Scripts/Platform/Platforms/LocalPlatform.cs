namespace KickblipsTwo.Platform.Platforms
{
    using UnityEngine;

    public class LocalPlatform
    {
        private const string HIGHSCORE_PREFIX = "Highscore_";


        /// <summary>
        /// Sets the highscore for a specific song.
        /// </summary>
        /// <param name="songName">The name of the song</param>
        /// <param name="score">The score itself</param>
        internal void SetHighscore(string songName, int score)
        {
            PlayerPrefs.SetInt(HIGHSCORE_PREFIX + songName, score);
        }

        /// <summary>
        /// Fetches the highscore.
        /// </summary>
        /// <param name="songName">The name of the song</param>
        /// <returns>The score itself</returns>
        internal int GetHighscore(string songName)
        {
            return PlayerPrefs.GetInt(HIGHSCORE_PREFIX + songName, 0);
        }
    }
}