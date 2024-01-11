namespace KickblipsTwo.Managers
{
    using KickblipsTwo.InputScroller;
    using KickblipsTwo.IO;
    using KickblipsTwo.UI;
    using MidiParser;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class Game : MonoBehaviour
    {
        // TODO: Do something about the InputScroller delay with the track start-up.
        public const float DelayBeforeTrackStart = 0.8593855f;

        public static MidiFile midiFile;

        [Header("References")]
        [SerializeField, Tooltip("The input scroller for pressing the right buttons.")]
        private InputScroller inputScroller;

        [SerializeField, Tooltip("The audio source being played for the song.")]
        private AudioSource musicAudioSource;

        [SerializeField, Tooltip("The input manager.")]
        private InputManager inputManager;

        [SerializeField, Tooltip("The score counter.")]
        private ScoreCounter scoreCounter;

        [SerializeField, Tooltip("The pool with all the input combinations.")]
        private InputCombinationPool inputCombinationPool;

        [Header("Difficulty setting")]
        [SerializeField, Tooltip("The maximum distance the checked buttons can have from the target position. After that, 0 points.")]
        private int maxScoreDistance = 130;

        [SerializeField, Tooltip("The maximum distance for input response.")]
        private int maxInteractionDistance = 300;

        /// <summary>
        /// The current score.
        /// </summary>
        private int score;

        /// <summary>
        /// The start time of the track within the game time.
        /// </summary>
        private float levelStartTime;

        /// <summary>
        /// Has the track started?
        /// </summary>
        private bool levelStarted;

        /// <summary>
        /// Checks if the input is correct.
        /// </summary>
        private bool checkInput;

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
                    int bpm = 125;
                    int ticksPerMinute = bpm * midiFile.TicksPerQuarterNote;
                    float ticksPerSecond = ticksPerMinute / 60;

                    // Browsing the entire midi data to fill in critical information.
                    for (int i = 0; i < midiFile.Tracks.Length; i++)
                        for (int j = 0; j < midiFile.Tracks[i].MidiEvents.Count; j++)
                        {
                            if (midiFile.Tracks[i].MidiEvents[j].MetaEventType == MetaEventType.Tempo)
                                bpm = midiFile.Tracks[i].MidiEvents[j].Note;

                            // Filling the midi events array.
                            if (midiFile.Tracks[i].MidiEvents[j].MidiEventType == MidiEventType.NoteOn)
                                midiEvents.Add(new KickblipsTwo.MidiEvent(ticksPerSecond, midiFile.Tracks[i].MidiEvents[j]));
                        }

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

            // Checks if the game can start.
            void CheckForStart()
            {
                if (midiFileFetched && trackFileFetched && !levelStarted)
                {
                    StartCoroutine(DoStart());

                    // Start the track with a delay.
                    IEnumerator DoStart()
                    {
                        levelStarted = true;

                        yield return new WaitForSeconds(DelayBeforeTrackStart);

                        musicAudioSource.Play();
                    }
                }
            }

            for (int i = 0; i < inputManager.PossibleInputs.Length; i++)
                inputManager.PossibleInputs[i].InputActionReference.action.Enable();

            UnityEngine.InputSystem.InputSystem.onActionChange += OnActionChange;
        }

        /// <summary>
        /// Checks if one of the possible inputs are being pressed and if they belong to the track input or not.
        /// </summary>
        /// <param name="arg1">The action being executed</param>
        /// <param name="arg2">The state of the action</param>
        private void OnActionChange(object arg1, UnityEngine.InputSystem.InputActionChange arg2)
        {
            if (arg2 == UnityEngine.InputSystem.InputActionChange.ActionStarted)
                for (int i = 0; i < inputManager.PossibleInputs.Length; i++)
                    if (inputManager.PossibleInputs[i].InputActionReference.action == arg1)
                        checkInput = true;
        }

        /// <summary>
        /// Destroy event subscriptions.
        /// </summary>
        private void OnDestroy()
        {
            for (int i = 0; i < inputManager.PossibleInputs.Length; i++)
                inputManager.PossibleInputs[i].InputActionReference.action.Disable();
        }

        /// <summary>
        /// Continuously checks the midi events whether an input combination should be spawned or not.
        /// If so, this will be communicated with the InputScroller.
        /// </summary>
        private void Update()
        {
            if (levelStarted)
            {
                // Make sure to set the level start time at the right moment.
                if (levelStartTime == 0)
                    levelStartTime = Time.time;

                for (int i = 0; i < midiEvents.Count; i++)
                {
                    if (!midiEvents[i].IsSpawned && !midiEvents[i].IsPassed && Time.time - levelStartTime > midiEvents[i].Time)
                    {
                        midiEvents[i].Spawn();

                        KickblipsTwo.MidiEvent secondMidiEvent = null;
                        if (i + 1 < midiEvents.Count && midiEvents[i+1].Time == midiEvents[i].Time)
                        {
                            // Checking if there is a second input combination there. Is so, add it to the spawn input combination method.
                            secondMidiEvent = midiEvents[i+1];
                            secondMidiEvent.Spawn();
                        }

                        inputScroller.SpawnInputCombination(midiEvents[i], secondMidiEvent, inputManager);
                    }
                }

                // Checking if there's any input from the player.
                if (inputCombinationPool.VisibleInputCombinations.Count > 0)
                {
                    InputCombination upcomingInputCombination = inputCombinationPool.VisibleInputCombinations[0];

                    float targetDistance = upcomingInputCombination.transform.position.y - inputScroller.InputTargetPosition.position.y;
                    float targetDistanceAbsolute = Mathf.Abs(targetDistance);

                    // Calculates the score and possibly give the player a score for this.
                    if (checkInput && targetDistanceAbsolute < maxInteractionDistance)
                    {
                        checkInput = false;

                        if (targetDistanceAbsolute < maxScoreDistance)
                        {
                            int scoreAdd = (int)(Mathf.Abs(Mathf.Abs(upcomingInputCombination.transform.position.y - inputScroller.InputTargetPosition.position.y) / maxScoreDistance - 1) * 100);

                            score += scoreAdd;
                            scoreCounter.UpdateScoreCounter(score);

                            inputCombinationPool.ReturnToPool(upcomingInputCombination);
                        }
                    }
                }
            }
        }
    }
}