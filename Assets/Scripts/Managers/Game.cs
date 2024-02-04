namespace KickblipsTwo.Managers
{
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
        // The time that needs to be retracted from the InputScroller's TransitionTime to correctly start the first beat.
        public const float InputScrollerCorrectionTime = 0.1406145f;

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

        [SerializeField, Tooltip("The combo counter.")]
        private ComboCounter comboCounter;

        [SerializeField, Tooltip("The pool with all the input combinations.")]
        private InputCombinationPool inputCombinationPool;

        [SerializeField, Tooltip("The HP bar of the player.")]
        private HPBar hpBar;

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
        /// The current score.
        /// </summary>
        private int score;

        /// <summary>
        /// The health of the player. 0 = game over.
        /// </summary>
        private int playerHealth = 100;

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

        private InputCombination upcomingInputCombination;
        private uint upcomingInputCombinationUID;
        private bool upcomingInputCombinationHit;


        /// <summary>
        /// Fetches a demo track and automatically starts the input scroller based on it.
        /// </summary>
        private void Awake()
        {
            bool midiFileFetched = false;
            bool trackFileFetched = false;

            FileHandler.HighlightFolder("Calibration");
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

                        yield return new WaitForSeconds(inputScroller.TransitionTime - InputScrollerCorrectionTime);

                        musicAudioSource.Play();
                    }
                }
            }

            for (int i = 0; i < inputManager.PossibleInputs.Length; i++)
                inputManager.PossibleInputs[i].InputActionReference.action.Enable();

            UnityEngine.InputSystem.InputSystem.onActionChange += OnActionChange;
            inputCombinationPool.OnReturnToPool += OnReturnToPool;
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
        /// Will be called whenever the player has no health left.
        /// </summary>
        private void NoHealthLeft()
        {
            Debug.Log("[Game] No HP left!");
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
                playerHealth = Mathf.Clamp(playerHealth - hpDepletionAmount, 0, 100);
                hpBar.UpdateHPBarStatus(playerHealth);
                comboCounter.ResetCombo();

                // Player health 0? Then you lose, if 
                if (playerHealth <= 0)
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
            for (int i = 0; i < inputManager.PossibleInputs.Length; i++)
                inputManager.PossibleInputs[i].InputActionReference.action.Disable();

            inputCombinationPool.OnReturnToPool -= OnReturnToPool;
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

                        KickblipsTwo.MidiEvent secondMidiEvent = null;
                        if (i + 1 < midiEvents.Count && midiEvents[i+1].Time == midiEvents[i].Time)
                        {
                            // Checking if there is a second input combination there. Is so, add it to the spawn input combination method.
                            secondMidiEvent = midiEvents[i+1];
                            secondMidiEvent.Spawn();
                        }

                        inputScroller.SpawnInputCombination(midiEvents[i], secondMidiEvent, inputManager);

                        upcomingInputCombination = inputCombinationPool.VisibleInputCombinations[0];
                        upcomingInputCombinationUID = upcomingInputCombination.UID;
                        StopListeningToInput();
                    }
                }

                // Checking if there's any input from the player.
                if (inputCombinationPool.VisibleInputCombinations.Count > 0)
                {
                    float targetDistance = upcomingInputCombination.transform.position.y - inputScroller.InputTargetPosition.position.y;
                    float targetDistanceAbsolute = Mathf.Abs(targetDistance);

                    // Calculates the score and possibly give the player a score for this.
                    if (checkInput)
                    {
                        checkInput = false;

                        if (targetDistanceAbsolute < maxScoreDistance)
                        {
                            // Listen to input with a duration.
                            ListenToInput((inputIsOK) =>
                            {
                                upcomingInputCombinationHit = true;

                                // If both inputs are fine, then the distance may be calculated for the input score.
                                if (inputIsOK)
                                {
                                    int scoreAdd = (int)(Mathf.Abs(Mathf.Abs(upcomingInputCombination.transform.position.y - inputScroller.InputTargetPosition.position.y) / maxScoreDistance - 1) * 100);

                                    score += scoreAdd;
                                    scoreCounter.UpdateScoreCounter(score);
                                    comboCounter.IncreaseComboCount();

                                    inputCombinationPool.ReturnToPool(upcomingInputCombination);
                                    playerHealth = Mathf.Clamp(playerHealth + hpRecoveryAmount, 0, 100);

                                    upcomingInputCombination = inputCombinationPool.VisibleInputCombinations.Count > 0 ? inputCombinationPool.VisibleInputCombinations[0] : null;
                                    upcomingInputCombinationUID = upcomingInputCombination == null ? 0 : upcomingInputCombination.UID;
                                    upcomingInputCombinationHit = false;
                                    StopListeningToInput();
                                }
                                else
                                {
                                    playerHealth = Mathf.Clamp(playerHealth - hpDepletionAmount, 0, 100);
                                    comboCounter.ResetCombo();
                                    StopListeningToInput();
                                }

                                hpBar.UpdateHPBarStatus(playerHealth);
                            });
                        }
                    }
                }
            }
        }
    }
}