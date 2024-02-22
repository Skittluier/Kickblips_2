namespace KickblipsTwo.UI.Screens
{
    using KickblipsTwo.Audio;
    using TMPro;
    using UnityEngine;
    using UnityEngine.EventSystems;
    using UnityEngine.UI;

    public class OptionsScreen : UI.Screen
    {
        [Header("References")]
        [SerializeField, Tooltip("The quit button")]
        private Button quitButton;

        [SerializeField, Tooltip("The back button")]
        private Button backButton;

        [SerializeField, Tooltip("The music slider")]
        private Slider musicSlider;

        [SerializeField, Tooltip("The event trigger for the music slider")]
        private EventTrigger musicSliderEventTrigger;

        [SerializeField, Tooltip("The music slider volume text next to the slider.")]
        private TMP_Text musicSliderValueText;

        [SerializeField, Tooltip("The SFX slider")]
        private Slider sfxSlider;

        [SerializeField, Tooltip("The SFX slider volume text next to the slider.")]
        private TMP_Text sfxSliderValueText;

        [SerializeField, Tooltip("The dropdown menu to change the resolution of the game.")]
        private TMP_Dropdown resolutionDropDown;

        [SerializeField, Tooltip("The button to reset the data and thus removes the save.")]
        private Button resetDataButton;

        private bool onMusicSliderPointerDown;

        [Header("Settings")]
        [SerializeField, Tooltip("The duration of the volume fade for the music preview.")]
        private float musicPreviewVolumeFadeDuration = 0.5f;

        [SerializeField, Tooltip("The preview music clip.")]
        private AudioClip previewMusicClip;

        [SerializeField, Tooltip("The preview SFX audio clip.")]
        private AudioClip previewSFXClip;

        [SerializeField, Tooltip("The formatting of the music slider text.\n{0} = Value in decibels")]
        private string musicSliderValueFormat = "{0:0} dB";

        [SerializeField, Tooltip("The formatting of the SFX slider text.\n{0} = Value in decibels")]
        private string sfxSliderValueFormat = "{0:0} dB";

        [SerializeField, Tooltip("The title for the pop-up when changing the resolution.")]
        private string changeResolutionConfirmTitle = "Change Resolution";

        [SerializeField, Tooltip("The resolution change confirmation text.\n{0}: Resolution width\n{1}: Resolution height")]
        private string changeResolutionConfirmMessage = "Are you sure you want to change the resolution to:\n\n{0}x{1}";

        [SerializeField, Tooltip("The title for the pop-up when resetting the data.")]
        private string dataResetConfirmTitle = "COMPLETE Data Reset";

        [SerializeField, Tooltip("The data reset confirmation message.")]
        private string dataResetConfirmMessage = "Are you sure you want to reset ALL YOUR DATA?\n It will be FOREVER lost and can never be recovered anymore!!";

        private Coroutine musicFadeCoroutine;


        /// <summary>
        /// Subscribe to some events for UI handling.
        /// </summary>
        private void Awake()
        {
            quitButton.onClick.AddListener(Application.Quit);
            backButton.onClick.AddListener(ScreenManager.GoToPreviousScreen);

            musicSlider.onValueChanged.AddListener(OnMusicSliderValueChanged);
            sfxSlider.onValueChanged.AddListener(OnSFXSliderValueChanged);
            resolutionDropDown.onValueChanged.AddListener(OnResolutionValueChanged);
            resetDataButton.onClick.AddListener(OnResetDataPressed);

            OnMusicSliderValueChanged(PlayerPrefs.GetFloat(AudioManager.MusicVolumePlayerPrefsKey, 0));
            OnSFXSliderValueChanged(PlayerPrefs.GetFloat(AudioManager.SFXVolumePlayerPrefsKey, 0));

            // Setting up the resolution option.
            resolutionDropDown.ClearOptions();

            int currentResolutionIndex = 0;
            for (int i = 0; i < Screen.resolutions.Length; i++)
            {
                resolutionDropDown.options.Add(new TMP_Dropdown.OptionData() { text = string.Format("{0}x{1}", Screen.resolutions[i].width, Screen.resolutions[i].height) });

                if (Screen.resolutions[i].width == Screen.currentResolution.width && Screen.resolutions[i].height == Screen.currentResolution.height && !PlayerPrefs.HasKey(Game.ResolutionPlayerPrefsKey))
                    currentResolutionIndex = i;
            }

            if (PlayerPrefs.HasKey(Game.ResolutionPlayerPrefsKey))
            {
                currentResolutionIndex = PlayerPrefs.GetInt(Game.ResolutionPlayerPrefsKey);
                Resolution resolution = Screen.resolutions[currentResolutionIndex];
                Screen.SetResolution(resolution.width, resolution.height, true);
            }

            resolutionDropDown.SetValueWithoutNotify(currentResolutionIndex);
        }

        /// <summary>
        /// The event that will be called when the music slider has been pointed down.
        /// </summary>
        public void OnMusicSliderPointerDown()
        {
            // Make sure it doesn't execute twice.
            if (onMusicSliderPointerDown)
                return;

            onMusicSliderPointerDown = true;

            if (musicFadeCoroutine != null)
            {
                Game.Instance.AudioManager.StopCoroutine(musicFadeCoroutine);
                musicFadeCoroutine = null;
            }

            musicFadeCoroutine = Game.Instance.AudioManager.FadeInMusic(musicPreviewVolumeFadeDuration);
            Game.Instance.AudioManager.PlayMusic(previewMusicClip);
        }

        /// <summary>
        /// The event that will be called when the music slider has been pointed up.
        /// </summary>
        public void OnMusicSliderPointerUp()
        {
            // Make sure it doesn't execute twice.
            if (!onMusicSliderPointerDown)
                return;

            onMusicSliderPointerDown = false;

            if (musicFadeCoroutine != null)
            {
                Game.Instance.AudioManager.StopCoroutine(musicFadeCoroutine);
                musicFadeCoroutine = null;
            }

            musicFadeCoroutine = Game.Instance.AudioManager.FadeOutMusic(musicPreviewVolumeFadeDuration, Game.Instance.AudioManager.StopMusic);
        }

        /// <summary>
        /// Will be called whenever the music slider value changes.
        /// </summary>
        /// <param name="value">The value of the music slider</param>
        private void OnMusicSliderValueChanged(float value)
        {
            Game.Instance.AudioManager.SetMusicVolume(value);
            musicSliderValueText.text = string.Format(musicSliderValueFormat, value);

            PlayerPrefs.SetFloat(AudioManager.MusicVolumePlayerPrefsKey, value);
        }

        /// <summary>
        /// Will be called whenever the SFX slider value changes.
        /// </summary>
        /// <param name="value">The new value</param>
        private void OnSFXSliderValueChanged(float value)
        {
            Game.Instance.AudioManager.SetSFXVolume((int)value);
            Game.Instance.AudioManager.PlaySFX(previewSFXClip);
            sfxSliderValueText.text = string.Format(sfxSliderValueFormat, value);

            PlayerPrefs.SetFloat(AudioManager.SFXVolumePlayerPrefsKey, value);
        }

        /// <summary>
        /// Will be called whenever the resolution value has been changed.
        /// </summary>
        /// <param name="value">The new resolution</param>
        /// <param name="showPopUp">Should a pop-up been shown?</param>
        private void OnResolutionValueChanged(int value)
        {
            Resolution resolution = Screen.resolutions[value];
            Screen.SetResolution(resolution.width, resolution.height, true);

            string popUpDescription = string.Format(changeResolutionConfirmMessage, resolution.width, resolution.height);

            ScreenManager.ShowPopUp(new PopUpScreen.PopUpScreenSettings()
            {
                Title = changeResolutionConfirmTitle,
                Description = popUpDescription,
                LeftButtonSettings = new PopUpScreen.PopUpScreenButtonSettings()
                {
                    ButtonName = "No",
                    ButtonAction = () =>
                    {
                        Resolution oldResolution = Screen.resolutions[PlayerPrefs.GetInt(Game.ResolutionPlayerPrefsKey, 0)];
                        Screen.SetResolution(oldResolution.width, oldResolution.height, true);

                        ScreenManager.ClosePopUp();
                    }
                },
                RightButtonSettings = new PopUpScreen.PopUpScreenButtonSettings()
                {
                    ButtonName = "Yes",
                    ButtonAction = () =>
                    {
                        PlayerPrefs.SetInt(Game.ResolutionPlayerPrefsKey, value);

                        ScreenManager.ClosePopUp();
                    }
                }
            });
        }

        /// <summary>
        /// Resets the ENTIRE save data + all player prefs.
        /// </summary>
        private void OnResetDataPressed()
        {
            ScreenManager.ShowPopUp(new PopUpScreen.PopUpScreenSettings()
            {
                Title = dataResetConfirmTitle,
                Description = dataResetConfirmMessage,
                LeftButtonSettings = new PopUpScreen.PopUpScreenButtonSettings()
                {
                    ButtonName = "No",
                    ButtonAction = ScreenManager.ClosePopUp
                },
                RightButtonSettings = new PopUpScreen.PopUpScreenButtonSettings()
                {
                    ButtonName = "Yes",
                    ButtonAction = () =>
                    {
                        PlayerPrefs.DeleteAll();

                        OnMusicSliderValueChanged(PlayerPrefs.GetFloat(AudioManager.MusicVolumePlayerPrefsKey, 0));
                        OnSFXSliderValueChanged(PlayerPrefs.GetFloat(AudioManager.SFXVolumePlayerPrefsKey, 0));

                        ScreenManager.ClosePopUp();
                    }
                }
            });
        }
    }
}