namespace KickblipsTwo
{
    /// <summary>
    /// A score registration structure for statistic purposes.
    /// </summary>
    public class MidiEvent
    {
        /// <summary>
        /// The note of the midi event
        /// </summary>
        internal int Note { get; private set; }

        /// <summary>
        /// The time of the midi event
        /// </summary>
        internal float Time { get; private set; }

        /// <summary>
        /// Was the button combination successfully pressed?
        /// </summary>
        internal bool IsSuccessfull { get; private set; }

        /// <summary>
        /// Has the midi event already been spawned into the scene?
        /// </summary>
        internal bool IsSpawned { get; private set; }

        /// <summary>
        /// Did the midi event already pass?
        /// </summary>
        internal bool IsPassed { get; private set; }


        /// <summary>
        /// Initializes the midi event.
        /// </summary>
        /// <param name="midiEvent">The midi event from the midi file.</param>
        internal MidiEvent(float ticksPerSecond, MidiParser.MidiEvent midiEvent)
        {
            Note = midiEvent.Note;
            Time = midiEvent.Time / (float)ticksPerSecond;
            IsSuccessfull = false;
            IsPassed = false;
        }
        
        /// <summary>
        /// Registers the midi event as spawned.
        /// </summary>
        internal void Spawn()
        {
            IsSpawned = true;
        }

        /// <summary>
        /// Will tell the midi event it passed.
        /// </summary>
        /// <param name="isSuccessfull">Was the event successfull?</param>
        internal void EventPassed(bool isSuccessfull)
        {
            IsPassed = true;
            IsSuccessfull = isSuccessfull;
        }
    }
}
