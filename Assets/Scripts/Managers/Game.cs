namespace KickblipsTwo.Managers
{
    using KickblipsTwo.InputScroller;
    using KickblipsTwo.IO;
    using MidiParser;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class Game : MonoBehaviour
    {
        // TODO: Do something about the InputScroller delay with the track start-up.
        public const float DelayBeforeTrackStart = 1;

        public static MidiFile midiFile;

        // The time multiplier is based on the midi file's TicksPerQuarterNote times 2.
        private float timeMultiplier;

        [SerializeField, Tooltip("The input scroller for pressing the right buttons.")]
        private InputScroller inputScroller;

        [SerializeField, Tooltip("The audio source being played for the song.")]
        private AudioSource musicAudioSource;

        [SerializeField, Tooltip("The input manager.")]
        private InputManager inputManager;

        private float trackStartTime = 0;
        private bool trackStarted;

        private List<KickblipsTwo.MidiEvent> midiEvents = new List<KickblipsTwo.MidiEvent>();


        /// <summary>
        /// Fetches a demo track and automatically starts the input scroller based on it.
        /// </summary>
        private void Awake()
        {
            bool midiFileFetched = false;
            bool trackFileFetched = false;

            FileHandler.HighlightFolder("Marissa Hapeman - T.N.T");
            FileHandler.FetchMidi((file) =>
            {
                midiFile = file;

                // Checking if the midi files can be checked.
                if (midiFile.TracksCount > 0)
                {
                    // Filling the midi events array.
                    for (int i = 0; i < midiFile.Tracks[0].MidiEvents.Count; i++)
                        if (midiFile.Tracks[0].MidiEvents[i].MidiEventType == MidiEventType.NoteOn)
                            midiEvents.Add(new KickblipsTwo.MidiEvent(midiFile.Tracks[0].MidiEvents[i]));


                    timeMultiplier = midiFile.TicksPerQuarterNote * 2;

                    midiFileFetched = true;
                    CheckForStart();
                }
                else
                    Debug.LogError("[Game] Couldn't play the track, because there are no Midi file tracks.");
            });
            FileHandler.FetchTrack((file) =>
            {
                musicAudioSource.clip = file;
                trackFileFetched = true;

                CheckForStart();
            });

            void CheckForStart()
            {
                if (midiFileFetched && trackFileFetched)
                {
                    StartCoroutine(DoStart());
                    IEnumerator DoStart()
                    {
                        trackStartTime = Time.time + DelayBeforeTrackStart;
                        trackStarted = true;

                        yield return new WaitForSeconds(DelayBeforeTrackStart);
                        musicAudioSource.Play();
                    }
                }
            }
        }

        /// <summary>
        /// Continuously checks the midi events whether an input combination should be spawned or not.
        /// If so, this will be communicated with the InputScroller.
        /// </summary>
        private void Update()
        {
            if (trackStarted)
                for (int i = 0; i < midiEvents.Count; i++)
                    if (!midiEvents[i].IsPassed && (Time.time * timeMultiplier) - trackStartTime > midiEvents[i].Time)
                        if (!midiEvents[i].IsSpawned)
                        {
                            midiEvents[i].Spawn();

                            // Checking if there is a second input combination there. Is so, add it to the spawn input combination method.
                            KickblipsTwo.MidiEvent secondMidiEvent = default;
                            if (i + 1 < midiEvents.Count && midiEvents[i+1].Time == midiEvents[i].Time)
                            {
                                secondMidiEvent = midiEvents[i+1];
                                secondMidiEvent.Spawn();
                            }

                            inputScroller.SpawnInputCombination(midiEvents[i], secondMidiEvent, inputManager);
                        }
        }
    }
}