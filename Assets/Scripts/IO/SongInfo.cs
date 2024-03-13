namespace KickblipsTwo.IO
{
    using MidiParser;

    internal class SongInfo
    {
        internal enum ErrorCode
        {
            Success,
            TrackIncorrectLength,
            TrackNoMidiEvents,
            NoTempoDefined
        }

        /// <summary>
        /// The name of the song.
        /// </summary>
        internal string Name { get; private set; }

        /// <summary>
        /// The name of the original artist.
        /// </summary>
        internal string OriginalArtistName { get; private set; }

        /// <summary>
        /// All the midi tracks known.
        /// The Track contains:
        /// 1. Text data, which defines the difficulty.
        /// 2. Midi data, which contains the track itself.
        /// </summary>
        internal MidiParser.MidiTrack[] Tracks { get; private set; }

        /// <summary>
        /// The highscore on this song.
        /// </summary>
        internal int Highscore { get; set; }


        /// <summary>
        /// Initializes the song information.
        /// </summary>
        /// <param name="trackName">The name of the track.</param>
        /// <param name="tracks">The tracks defined incl. the metadata.</param>
        /// <param name="originalArtistName">The name of the original artist.</param>
        internal SongInfo(string trackName, MidiTrack[] tracks, string originalArtistName, int highscore)
        {
            Name = trackName;
            Tracks = tracks;
            OriginalArtistName = originalArtistName;
            Highscore = highscore;
        }

        /// <summary>
        /// Checks if the amount of tracks is valid and the appropriate metadata is available.
        /// In the entire process we're skipping the first track, because of a FL Studio issue.
        /// </summary>
        /// <param name="originalArtistName">The name of the original artist who made this song</param>
        /// <returns>The error code returning after validating the song.</returns>
        internal ErrorCode ValidateSong(out string originalArtistName)
        {
            originalArtistName = null;
            int? bpm = null;

            // Checking if there is at least one track.
            if (Tracks.Length < 2)
                return ErrorCode.TrackIncorrectLength;

            for (int i = 1; i < Tracks.Length; i++)
                if (Tracks[i].MidiEvents.Count == 0)
                    return ErrorCode.TrackNoMidiEvents;

            // Looking for an artist name.
            for (int i = 0; i < Tracks[1].TextEvents.Count; i++)
                if (Tracks[1].TextEvents[i].TextEventType == TextEventType.TrackName)
                    originalArtistName = Tracks[1].TextEvents[i].Value;

            // Looking for the tempo.
            for (int j = 0; j < Tracks[0].MidiEvents.Count; j++)
                if (Tracks[0].MidiEvents[j].MetaEventType == MetaEventType.Tempo)
                    bpm = Tracks[0].MidiEvents[j].Note;

            // No BPM? Then the tempo isn't defined.
            if (!bpm.HasValue)
                return ErrorCode.NoTempoDefined;

            return ErrorCode.Success;
        }
    }
}