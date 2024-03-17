namespace KickblipsTwo.Audio
{
    using System;
    using System.Collections;
    using UnityEngine;
    using UnityEngine.Audio;

    public class AudioManager : MonoBehaviour
    {
        internal const string MusicVolumePlayerPrefsKey = "Audio_MusicVolume";
        internal const string SFXVolumePlayerPrefsKey = "Audio_SFXVolume";

        [Header("References")]
        [SerializeField, Tooltip("The audio mixer we're using.")]
        private AudioMixer audioMixer;

        [SerializeField, Tooltip("The audio source belonging to SFXs.")]
        private AudioSource sfxAudioSource;

        [SerializeField, Tooltip("The audio source for music.")]
        private AudioSource musicAudioSource;

        [Header("Settings")]
        [SerializeField, Tooltip("The volume range. X = Minimum, Y = Maximum")]
        private Vector2 volumeRange = new Vector2(-80, 20);

        [SerializeField, Tooltip("The SFX group in the audio mixer.")]
        private AudioMixerGroup sfxMixGroup;

        [SerializeField, Tooltip("The music group in the audio mixer.")]
        private AudioMixerGroup musicMixGroup;
        private float setupMusicMixerValue;


        /// <summary>
        /// Sets the SFX volume.
        /// </summary>
        /// <param name="value">The value of the SFX volume.</param>
        internal void SetSFXVolume(int value)
        {
            audioMixer.SetFloat(sfxMixGroup.name, Mathf.Clamp(value, volumeRange.x, volumeRange.y));
        }

        /// <summary>
        /// Sets the music volume.
        /// </summary>
        /// <param name="value">The value of the music volume.</param>
        internal void SetMusicVolume(float value)
        {
            float setupValue = Mathf.Clamp(value, volumeRange.x, volumeRange.y);

            audioMixer.SetFloat(musicMixGroup.name, setupValue);
            setupMusicMixerValue = setupValue;
        }

        /// <summary>
        /// Fades in the music.
        /// </summary>
        /// <param name="fadeDuration">The fade in duration</param>
        internal Coroutine FadeInMusic(float fadeDuration)
        {
            return StartCoroutine(DoFadeInMusic(fadeDuration));
            IEnumerator DoFadeInMusic(float fadeDuration)
            {
                float currVal = 0;
                float fromVolume = volumeRange.x;
                float toVolume = setupMusicMixerValue;

                while (currVal < 1)
                {
                    currVal += Time.deltaTime / fadeDuration;

                    if (currVal >= 1)
                        audioMixer.SetFloat(musicMixGroup.name, toVolume);
                    else
                        audioMixer.SetFloat(musicMixGroup.name, Mathf.Lerp(fromVolume, toVolume, currVal));

                    yield return null;
                }
            }
        }

        /// <summary>
        /// Fades out the music.
        /// </summary>
        /// <param name="fadeDuration">The fade out duration</param>
        /// <param name="onFadeOutComplete">The action called after the fade out is complete</param>
        internal Coroutine FadeOutMusic(float fadeDuration, Action onFadeOutComplete)
        {
            return StartCoroutine(DoFadeOutMusic(fadeDuration, onFadeOutComplete));
            IEnumerator DoFadeOutMusic(float fadeDuration, Action onFadeOutComplete)
            {
                float currVal = 0;
                audioMixer.GetFloat(musicMixGroup.name, out float fromVolume);
                float toVolume = volumeRange.x;

                while (currVal < 1)
                {
                    currVal += Time.deltaTime / fadeDuration;

                    if (currVal >= 1)
                        audioMixer.SetFloat(musicMixGroup.name, toVolume);
                    else
                        audioMixer.SetFloat(musicMixGroup.name, Mathf.Lerp(fromVolume, toVolume, currVal));

                    yield return null;
                }

                onFadeOutComplete?.Invoke();
            }
        }

        /// <summary>
        /// Plays a one-shot audio clip for a SFX.
        /// </summary>
        /// <param name="clip">The audio clip</param>
        internal void PlaySFX(AudioClip clip)
        {
            sfxAudioSource.PlayOneShot(clip);
        }

        /// <summary>
        /// Sets the music clip reference.
        /// </summary>
        /// <param name="musicClip">The clip in question</param>
        internal void SetMusicClip(AudioClip musicClip)
        {
            musicAudioSource.clip = musicClip;
        }

        /// <summary>
        /// Plays a looping audio clip for music.
        /// </summary>
        /// <param name="musicClip">The music clip</param>
        internal void PlayMusic(AudioClip musicClip = null)
        {
            if (musicClip != null)
                SetMusicClip(musicClip);

            musicAudioSource.loop = true;
            musicAudioSource.Play();
        }

        /// <summary>
        /// Fetches the music clip from the audio source and returns its length.
        /// </summary>
        /// <returns>The music clip length</returns>
        internal float GetMusicClipLength()
        {
            return musicAudioSource.clip.length;
        }

        /// <summary>
        /// Stops the music.
        /// </summary>
        internal void StopMusic()
        {
            musicAudioSource.Stop();
        }
    }
}