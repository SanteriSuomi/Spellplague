using Spellplague.Utility;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Spellplague.Player
{
#pragma warning disable S3168 // Fire and forget method does not need to return task.
    [RequireComponent(typeof(AudioSource))]
    public class PlayerSoundController : MonoBehaviour
    {
        [SerializeField]
        private PlayerStateVariable playerState = default;
        [SerializeField]
        private AudioClip[] audioClips = default;
        private Dictionary<string, AudioClip> sounds;
        private AudioSource audioSource;
        [SerializeField]
        private string footStepsSoundClip = "FootSteps";
        [SerializeField]
        private string swordSoundClip = "Sword";
        [SerializeField]
        private string swordHitSoundClip = "SwordHit";
        [SerializeField]
        private float walkFootStepVolume = 0.45f;
        [SerializeField]
        private float runFootStepVolume = 0.65f;
        [SerializeField]
        private float crouchFootStepVolume = 0.25f;
        [SerializeField]
        private float walkSoundLengthMultiplier = 1.75f;
        [SerializeField]
        private float runSoundLengthMultiplier = 1.25f;
        [SerializeField]
        private float crouchSoundLengthMultiplier = 2.25f;
        [SerializeField]
        private float combatSwordVolume = 0.45f;
        private bool runTasks;

        private void Awake()
        {
            sounds = new Dictionary<string, AudioClip>();
            audioSource = GetComponent<AudioSource>();
            for (int i = 0; i < audioClips.Length; i++)
            {
                sounds.Add(audioClips[i].name, audioClips[i]);
            }
        }

        private void OnEnable()
        {
            runTasks = true;
            FootSteps();
            CombatSounds();
        }

        private async void FootSteps()
        {
            while (runTasks)
            {
                if (playerState.CurrentPlayerStance != PlayerStance.Jump)
                {
                    if (playerState.CurrentPlayerStance == PlayerStance.Crouch
                        && (playerState.CurrentPlayerMoveState == PlayerMove.Walk
                        || playerState.CurrentPlayerMoveState == PlayerMove.Sprint))
                    {
                        await PlayDynamicAsync(footStepsSoundClip, crouchFootStepVolume,
                            sounds[footStepsSoundClip].length, crouchSoundLengthMultiplier);
                    }
                    else if (playerState.CurrentPlayerMoveState == PlayerMove.Walk)
                    {
                        await PlayDynamicAsync(footStepsSoundClip, walkFootStepVolume,
                            sounds[footStepsSoundClip].length, walkSoundLengthMultiplier);
                    }
                    else if (playerState.CurrentPlayerMoveState == PlayerMove.Sprint)
                    {
                        await PlayDynamicAsync(footStepsSoundClip, runFootStepVolume,
                            sounds[footStepsSoundClip].length, runSoundLengthMultiplier);
                    }
                }

                await DelayDeltaTime();
            }
        }

        private async void CombatSounds()
        {
            while (runTasks)
            {
                if (playerState.CurrentPlayerCombatState == PlayerCombatState.Hit)
                {
                    Play(swordHitSoundClip, combatSwordVolume);
                }
                else if (playerState.CurrentPlayerCombatState == PlayerCombatState.Attacking)
                {
                    Play(swordSoundClip, combatSwordVolume);
                }

                await DelayDeltaTime();
            }
        }

        public Task PlayDynamicAsync(string clip, float volume, float length, float lengthMultiplier)
        {
            Play(clip, volume);
            return Task.Delay(TimeSpan.FromSeconds(length * lengthMultiplier));
        }

        public void Play(string clip, float volume)
        {
            if (!sounds.ContainsKey(clip))
            {
                Debug.LogError("No clip with this name.");
                return;
            }

            audioSource.PlayOneShot(sounds[clip], volume);
        }

        private Task DelayDeltaTime()
        {
            return Task.Delay(TimeSpan.FromMilliseconds(Time.deltaTime * 1000));
        }

        private void OnDisable() => runTasks = false;
    }
}