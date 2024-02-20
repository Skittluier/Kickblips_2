namespace KickblipsTwo.IO
{
    internal class SongInfo
    {
        /// <summary>
        /// All the midi tracks known.
        /// The Track contains:
        /// 1. Text data, which defines the difficulty.
        /// 2. Midi data, which contains the track itself.
        /// </summary>
        internal MidiParser.MidiTrack[] Tracks { get; private set; }


        /// <summary>
        /// Initializes the song information.
        /// </summary>
        /// <param name="tracks">The tracks defined incl. the metadata.</param>
        internal SongInfo(MidiParser.MidiTrack[] tracks)
        {
            Tracks = tracks;
        }


        /// <summary>
        /// Checks if the amount of tracks is valid and the appropriate metadata is available.
        /// </summary>
        /// <returns>Is the song valid for the game?</returns>
        internal bool ValidateSong()
        {
            // Going through all the tracks.
            for (int i = 0; i < Tracks.Length; i++)
            {
                // TODO: Validate that stuff.
            }

            return true;
        }
    }
}