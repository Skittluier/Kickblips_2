namespace KickblipsTwo.IO
{
    using KickblipsTwo.Extensions;
    using KickblipsTwo.Managers;
    using MidiParser;
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using UnityEngine;
    using UnityEngine.Networking;

    internal static class FileHandler
    {
        /// <summary>
        /// The default music folder location for all the music we're looking for.
        /// </summary>
        private const string DefaultMusicFolder = "Music";

        /// <summary>
        /// The current highlighted folder.
        /// </summary>
        internal static string HighlightedFolder { get; private set; }


        /// <summary>
        /// Highlights a file for future functionality.
        /// It is required to have the folder in the StreamingAssets/Music folder.
        /// </summary>
        internal static void HighlightFolder(string folderName)
        {
            HighlightedFolder = folderName;
        }

        /// <summary>
        /// Fetches a track from the highlighted folder.
        /// </summary>
        /// <returns>The track file</returns>
        internal static async void FetchTrack(Action<AudioClip> onTrackFetched)
        {
            if (string.IsNullOrEmpty(HighlightedFolder))
            {
                Debug.LogError("[FileHandler] There is no highlighted folder. Please execute that method first.");
                return;
            }

            string[] files = Directory.GetFiles($"{Application.streamingAssetsPath}/{DefaultMusicFolder}/{HighlightedFolder}/", "*.wav");

            // No wav files? MP3 files it is.
            if (files.Length == 0)
                files = Directory.GetFiles($"{Application.streamingAssetsPath}/{DefaultMusicFolder}/{HighlightedFolder}/", "*.mp3");

            // Checking if there are any audio files at all.
            if (files.Length == 0)
            {
                Debug.LogError("[FileHandler] No audio files files were found.");
                return;
            }
            // Checking if there are more than 1 audio file.
            else if (files.Length > 1)
            {
                Debug.LogError("[FileHandler] Too many audio files files have been found.");
                return;
            }

            AudioType audioType;
            string[] splitFilesString = files[0].Split('.');
            if (Equals(splitFilesString[splitFilesString.Length - 1], "mp3"))
                audioType = AudioType.MPEG;
            else
                audioType = AudioType.WAV;

            UnityWebRequest unityWebRequestMultimedia = UnityWebRequestMultimedia.GetAudioClip(files[0], audioType);
            await unityWebRequestMultimedia.SendWebRequest();

            if (DownloadHandlerAudioClip.GetContent(unityWebRequestMultimedia) is AudioClip clip)
                onTrackFetched(clip);
        }

        /// <summary>
        /// Fetches a MIDI file from the highlighted folder.
        /// </summary>
        /// <returns>The midi file</returns>
        internal static async void FetchMidi(Action<MidiFile> onMidiFetched)
        {
            if (string.IsNullOrEmpty(HighlightedFolder))
            {
                Debug.LogError("[FileHandler] There is no highlighted folder. Please execute that method first.");
                return;
            }

            string[] files = Directory.GetFiles($"{Application.streamingAssetsPath}/{DefaultMusicFolder}/{HighlightedFolder}/", "*.mid");

            // Checking if there are any midi files at all.
            if (files.Length == 0)
            {
                Debug.LogError("[FileHandler] No midi files files were found.");
                return;
            }
            // Checking if there are more than 1 midi file.
            else if (files.Length > 1)
            {
                Debug.LogError("[FileHandler] Too many midi files files have been found.");
                return;
            }

            UnityWebRequest unityWebRequest = UnityWebRequest.Get(files[0]);

            await unityWebRequest.SendWebRequest();

            while (!unityWebRequest.isDone)
                await Task.Yield();

            if (unityWebRequest.result == UnityWebRequest.Result.Success)
                onMidiFetched(new MidiFile(unityWebRequest.downloadHandler.data));
            else
                Debug.LogError($"[FileHandler] Couldn't fetch midi file: {unityWebRequest.result}");
        }
    }
}