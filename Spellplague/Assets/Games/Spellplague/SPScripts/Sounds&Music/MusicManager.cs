using Spellplague.Utility;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Spellplague.Sounds
{
    #pragma warning disable S3168 // Fire and forget method does not need to return a task
    [RequireComponent(typeof(AudioSource))]
    public class MusicManager : Singleton<MusicManager>
    {
        private readonly Dictionary<string, AudioClip> audioDictionary = new Dictionary<string, AudioClip>();
        [SerializeField]
        private AudioClip[] audioClips = default;
        private AudioSource audioSource;
        [SerializeField]
        private string startingClipKey = "Safe Zone";
        [SerializeField]
        private float fadeMultiplier = 0.6f;
        private float maxVolume;
        private bool keepFading = true;

        protected override void Awake()
        {
            base.Awake();
            Initialize();
            AddStartingClip();
        }

        private void Initialize()
        {
            audioSource = GetComponent<AudioSource>();
            maxVolume = audioSource.volume;
            for (int i = 0; i < audioClips.Length; i++)
            {
                audioDictionary.Add(audioClips[i].name, audioClips[i]);
            }
        }

        private void AddStartingClip()
        {
            audioSource.clip = audioDictionary[startingClipKey];
            audioSource.Play();
        }

        public AudioSource GetSource() => audioSource;

        public async void ChangeClip(string audioClip)
        {
            if (!audioDictionary.ContainsKey(audioClip))
            {
                Debug.LogError($"{audioClip} does not exist!");
                return;
            }

            await FadeOut();
            audioSource.clip = audioDictionary[audioClip];
            if (!audioSource.isPlaying)
            {
                audioSource.Play();
            }
            await FadeIn();
        }

        private async Task FadeOut()
        {
            while (audioSource.volume > 0 && keepFading)
            {
                audioSource.volume -= Time.deltaTime * fadeMultiplier;
                await Task.Delay(TimeSpan.FromMilliseconds(Time.deltaTime * 1000));
            }
        }

        private async Task FadeIn()
        {
            while (audioSource.volume < maxVolume && keepFading)
            {
                audioSource.volume += Time.deltaTime * fadeMultiplier;
                await Task.Delay(TimeSpan.FromMilliseconds(Time.deltaTime * 1000));
            }
        }

        private void OnDisable() => keepFading = false;
    }
}