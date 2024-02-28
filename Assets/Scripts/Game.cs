namespace KickblipsTwo
{
    using KickblipsTwo.Audio;
    using KickblipsTwo.Input;
    using KickblipsTwo.InputScroller;
    using KickblipsTwo.IO;
    using KickblipsTwo.UI;
    using MidiParser;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class Game : MonoBehaviour
    {
        internal const string ResolutionPlayerPrefsKey = "Game_ResolutionIndex";

        internal static Game Instance { get; private set; }

        // The time that needs to be retracted from the InputScroller's TransitionTime to correctly start the first beat.
        public const float InputScrollerCorrectionTime = 0.1406145f;

        public static MidiFile midiFile;

        [Header("References")]
        [SerializeField, Tooltip("The input scroller for pressing the right buttons.")]
        private InputScroller.InputScroller inputScroller;

        [SerializeField, Tooltip("The input manager.")]
        private InputManager inputManager;

        [field: SerializeField, Tooltip("The audio manager.")]
        internal AudioManager AudioManager { get; private set; }

        [field: SerializeField, Tooltip("The score counter.")]
        internal ScoreCounter ScoreCounter { get; private set; }

        [field: SerializeField, Tooltip("The combo counter.")]
        internal ComboCounter ComboCounter { get; private set; }

        [SerializeField, Tooltip("The pool with all the input combinations.")]
        private InputCombinationPool inputCombinationPool;

        [SerializeField, Tooltip("The screen manager which controls... well... all the screens.")]
        private ScreenManager screenManager;

        [field: SerializeField, Tooltip("The HP bar of the player.")]
        internal HPBar HPBar { get; private set; }

        [Header("UI settings")]
        [SerializeField, Tooltip("The starting screen.")]
        private UI.Screen.ScreenType screenType;

        [SerializeField, Tooltip("The delay after the final note until the results screen pops up.")]
        private int resultsScreenPopUpDelay = 2;

        [Header("Difficulty setting")]
        [SerializeField, Tooltip("The maximum distance the checked buttons can have from the target position. After that, 0 points.")]
        private int maxScoreDistance = 130;

        [SerializeField, Tooltip("The amount of seconds the system will listen to all the inputs coming from the device until it judges the entire input.")]
        private float inputListenDuration = 0.25f;

        private float listenToInputEndTime;
        private Coroutine inputListenCoroutine;

        [Header("Health settings")]
        [SerializeField, Tooltip("The amount of health that will be recovered whenever the player hits a good combination.")]
        private int hpRecoveryAmount = 10;

        [SerializeField, Tooltip("The amount of health that will be depleted whenever the player hits an incorrect combination.")]
        private int hpDepletionAmount = 5;

        /// <summary>
        /// The start time of the track within the game time.
        /// </summary>
        private float levelStartTime;

        /// <summary>
        /// Has the track started?
        /// </summary>
        private bool levelStarted;

        /// <summary>
        /// Gives a message to the game to check the input.
        /// </summary>
        private bool checkInput;

        private List<KickblipsTwo.MidiEvent> midiEvents = new List<KickblipsTwo.MidiEvent>();
        internal int TotalNotes => midiEvents.Count;
        internal int NotesHit { get; private set; }

        internal string CurrentDifficulty { get; private set; }

        private InputCombination upcomingInputCombination;
        private uint upcomingInputCombinationUID;
        private bool upcomingInputCombinationHit;


        /// <summary>
        /// Fetches a demo track and automatically starts the input scroller based on it.
        /// </summary>
        private void Awake()
        {
            // Setting up the singleton.
            if (Instance == null)
                Instance = this;
            else
            {
                Debug.LogError("[Game] There is more than 1 Game in the scene. Check out what's up!");
                Destroy(this);
            }

            inputManager.SwitchToUIActionMap();
            inputCombinationPool.OnReturnToPool += OnReturnToPool;
            UnityEngine.InputSystem.InputSystem.onActionChange += OnActionChange;
        }

        /// <summary>
        /// Prepares everything and then plays the song.
        /// </summary>
        /// <param name="songTitle">The title of the song according to the StreamingAssets folder</param>
        internal void PlaySong(string songTitle)
        {
            bool midiFileFetched = false;
            bool trackFileFetched = false;
            int bpm = 125;

            FileHandler.HighlightFolder(songTitle);
            FileHandler.FetchMidi((file) =>
            {
                midiFile = file;

                // Checking if the midi files can be checked.
                if (midiFile.TracksCount > 0)
                {
                    int ticksPerMinute = bpm * midiFile.TicksPerQuarterNote;
                    float ticksPerSecond = ticksPerMinute / 60;

                    // Browsing the entire midi data to fill in critical information.
                    for (int i = 0; i < midiFile.Tracks.Length; i++)
                        for (int j = 0; j < midiFile.Tracks[i].MidiEvents.Count; j++)
                        {
                            if (midiFile.Tracks[i].MidiEvents[j].MetaEventType == MetaEventType.Tempo)
                            {
                                bpm = midiFile.Tracks[i].MidiEvents[j].Note;
                                ticksPerMinute = bpm * midiFile.TicksPerQuarterNote;
                                ticksPerSecond = ticksPerMinute / 60;
                            }

                            // Filling the midi events array.
                            if (midiFile.Tracks[i].MidiEvents[j].MidiEventType == MidiEventType.NoteOn)
                                midiEvents.Add(new MidiEvent(ticksPerSecond, midiFile.Tracks[i].MidiEvents[j]));
                        }

                    midiFileFetched = true;
                    CheckForStart();
                }
                else
                    Debug.LogError("[Game] Couldn't play the track, because there are no Midi file tracks.");
            });

            FileHandler.FetchTrack((file) =>
            {
                AudioManager.SetMusicClip(file);
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
                        // Recover the HP.
                        HPBar.UpdateHPBarStatus(HPBar.defaultPlayerHealth);

                        // Reset all the other values.
                        levelStartTime = 0;
                        listenToInputEndTime = 0;
                        NotesHit = 0;
                        checkInput = default;
                        upcomingInputCombination = null;
                        upcomingInputCombinationUID = default;
                        upcomingInputCombinationHit = default;
                        ScoreCounter.UpdateScoreCounter(0);
                        ComboCounter.ResetEverything();

                        if (inputListenCoroutine != null)
                        {
                            StopCoroutine(inputListenCoroutine);
                            inputListenCoroutine = null;
                        }

                        levelStarted = true;

                        // The InputScrollerCorrectionTime is based on 60BPM. So it will be adjusted based on the different in the BPM.
                        yield return new WaitForSeconds(inputScroller.TransitionTime - InputScrollerCorrectionTime * (bpm / 60));

                        AudioManager.PlayMusic();
                        StartCoroutine(DoStopSongDelayed());

                        IEnumerator DoStopSongDelayed()
                        {
                            yield return new WaitForSeconds(AudioManager.GetMusicClipLength());
                            StopSong();
                        }
                    }
                }
            }

            for (int i = 0; i < inputManager.PossibleNoteInputs.Length; i++)
                inputManager.PossibleNoteInputs[i].InputActionReference.action.Enable();

            screenManager.ChangeScreen(UI.Screen.ScreenType.Game);
        }

        /// <summary>
        /// Checks if one of the possible inputs are being pressed and if they belong to the track input or not.
        /// </summary>
        /// <param name="arg1">The action being executed</param>
        /// <param name="arg2">The state of the action</param>
        private void OnActionChange(object arg1, UnityEngine.InputSystem.InputActionChange arg2)
        {
            if (arg2 == UnityEngine.InputSystem.InputActionChange.ActionStarted)
                for (int i = 0; i < inputManager.PossibleNoteInputs.Length; i++)
                    if (inputManager.PossibleNoteInputs[i].InputActionReference.action == arg1)
                        checkInput = true;
        }

        /// <summary>
        /// Will be called whenever the player has no health left.
        /// </summary>
        private void NoHealthLeft()
        {
            Debug.Log("[Game] No HP left!");
            StopSong(true);
        }

        /// <summary>
        /// Stops the song (and thus game) and shows the results screen instantly.
        /// </summary>
        /// <param name="immediateStop">Immediately stops everything and shows the result screen.</param>
        private void StopSong(bool immediateStop = false)
        {
            levelStarted = false;
            AudioManager.StopMusic();

            StartCoroutine(DoStopSong(immediateStop));
            IEnumerator DoStopSong(bool immediateStop = false)
            {
                if (!immediateStop)
                    yield return new WaitForSeconds(resultsScreenPopUpDelay);

                // Change the screen to the results screen.
                screenManager.ChangeScreen(UI.Screen.ScreenType.Results);

                // Disable all possible game input.
                for (int i = 0; i < inputManager.PossibleNoteInputs.Length; i++)
                    inputManager.PossibleNoteInputs[i].InputActionReference.action.Disable();

                // Return the visible input combinations.
                while (inputCombinationPool.VisibleInputCombinations.Count > 0)
                    inputCombinationPool.ReturnToPool(inputCombinationPool.VisibleInputCombinations[0]);
            }
        }

        /// <summary>
        /// Will be called whenever an input combination has been sent back to the pool.
        /// </summary>
        /// <param name="inputCombination">The input combination in question</param>
        private void OnReturnToPool(InputCombination inputCombination)
        {
            if (Equals(inputCombination.UID, upcomingInputCombinationUID) && !upcomingInputCombinationHit)
            {
                // If the upcoming input combination isn't null and it's already despawned, then punish the player.
                HPBar.UpdateHPBarStatus(Mathf.Clamp(HPBar.PlayerHealth - hpDepletionAmount, 0, 100));
                ComboCounter.ResetCombo();

                // Player health 0? Then you lose.
                if (HPBar.PlayerHealth <= 0)
                    NoHealthLeft();
            }

            // Resetting everything quickly.
            upcomingInputCombinationUID = 0;
            upcomingInputCombinationHit = false;
            StopListeningToInput();
        }

        /// <summary>
        /// Destroy event subscriptions.
        /// </summary>
        private void OnDestroy()
        {
            for (int i = 0; i < inputManager.PossibleNoteInputs.Length; i++)
                inputManager.PossibleNoteInputs[i].InputActionReference.action.Disable();

            UnityEngine.InputSystem.InputSystem.onActionChange -= OnActionChange;
            inputCombinationPool.OnReturnToPool -= OnReturnToPool;

            Instance = null;
        }

        /// <summary>
        /// Listens to the input for a certain amount of time.
        /// </summary>
        private void ListenToInput(Action<bool> doneListening)
        {
            if (inputListenCoroutine != null)
                return;

            listenToInputEndTime = Time.time + inputListenDuration;
            inputListenCoroutine = StartCoroutine(DoListenToInput(doneListening));

            IEnumerator DoListenToInput(Action<bool> doneListening)
            {
                // Are the first & second input OK?
                bool firstInputOK = false;
                bool secondInputOK = false;

                while (listenToInputEndTime > Time.time)
                {
                    // Only check if the first input is going to be OK within the time.
                    if (!firstInputOK)
                        firstInputOK = upcomingInputCombination.FirstInput.InputActionReference.action.triggered;

                    // Only check if the second input is going to be OK within the time.
                    if (!secondInputOK)
                        secondInputOK = upcomingInputCombination.SecondInput == null || (upcomingInputCombination.SecondInput != null && upcomingInputCombination.SecondInput.InputActionReference.action.triggered);

                    // Both correct? Then break it!
                    if (firstInputOK && secondInputOK)
                        break;

                    yield return null;
                }

                doneListening?.Invoke(firstInputOK && secondInputOK);
                StopListeningToInput();
            }
        }

        /// <summary>
        /// Stops listening to the input.
        /// </summary>
        private void StopListeningToInput()
        {
            if (inputListenCoroutine != null)
            {
                StopCoroutine(inputListenCoroutine);
                inputListenCoroutine = null;
            }
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

                        MidiEvent secondMidiEvent = null;
                        if (i + 1 < midiEvents.Count && midiEvents[i+1].Time == midiEvents[i].Time)
                        {
                            // Checking if there is a second input combination there. Is so, add it to the spawn input combination method.
                            secondMidiEvent = midiEvents[i+1];
                            secondMidiEvent.Spawn();
                        }

                        // Would the note like to be on the right side? Then swap the midi events!
                        if (secondMidiEvent != null && inputManager.NoteIsPreferablyOnRightSide(midiEvents[i].Note))
                        {
                            MidiEvent midiEvent = secondMidiEvent;
                            secondMidiEvent = midiEvents[i];
                            midiEvents[i] = midiEvent;
                        }

                        inputScroller.SpawnInputCombination(midiEvents[i], secondMidiEvent, inputManager);
                        StopListeningToInput();
                    }
                }

                // Checking if there's any input from the player.
                if (inputCombinationPool.VisibleInputCombinations.Count > 0)
                {
                    float smallestDistance = float.MaxValue;

                    for (int i = 0; i < inputCombinationPool.VisibleInputCombinations.Count; i++)
                    {
                        float targetDistance = inputCombinationPool.VisibleInputCombinations[i].transform.position.y - inputScroller.InputTargetPosition.position.y;
                        float targetDistanceAbsolute = Mathf.Abs(targetDistance);

                        // Checking if the distance is the smallest distance of them all.
                        if (smallestDistance > targetDistanceAbsolute && targetDistanceAbsolute < maxScoreDistance)
                        {
                            upcomingInputCombination = inputCombinationPool.VisibleInputCombinations[i];
                            upcomingInputCombinationUID = upcomingInputCombination.UID;

                            smallestDistance = targetDistanceAbsolute;
                        }
                    }

                    // Calculates the score and possibly give the player a score for this.
                    if (checkInput && upcomingInputCombination != null)
                    {
                        checkInput = false;

                        // Listen to input with a duration.
                        ListenToInput((inputIsOK) =>
                        {
                            upcomingInputCombinationHit = true;

                            // If both inputs are fine, then the distance may be calculated for the input score.
                            if (inputIsOK)
                            {
                                int scoreAdd = (int)(Mathf.Abs(Mathf.Abs(upcomingInputCombination.transform.position.y - inputScroller.InputTargetPosition.position.y) / maxScoreDistance - 1) * 100);

                                NotesHit++;

                                ScoreCounter.UpdateScoreCounter(ScoreCounter.Score + scoreAdd);
                                ComboCounter.IncreaseComboCount();
                                inputCombinationPool.ReturnToPool(upcomingInputCombination);
                                HPBar.UpdateHPBarStatus(Mathf.Clamp(HPBar.PlayerHealth + hpRecoveryAmount, 0, 100));

                                upcomingInputCombination = null;
                                upcomingInputCombinationUID = default;
                                upcomingInputCombinationHit = false;
                                StopListeningToInput();
                            }
                            else
                            {
                                HPBar.UpdateHPBarStatus(Mathf.Clamp(HPBar.PlayerHealth - hpDepletionAmount, 0, 100));
                                ComboCounter.ResetCombo();

                                inputCombinationPool.ReturnToPool(upcomingInputCombination);
                                upcomingInputCombination = null;
                                upcomingInputCombinationUID = default;
                                upcomingInputCombinationHit = false;

                                StopListeningToInput();

                                // Player health 0? Then you lose.
                                if (HPBar.PlayerHealth <= 0)
                                    NoHealthLeft();
                            }
                        });
                    }
                }
            }
        }
    }
}