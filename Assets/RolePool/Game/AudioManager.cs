using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Pool;
using UnityEngine.Serialization;

namespace RolePool.Game
{
    public class AudioManager : MonoBehaviour
    {
        private ObjectPool<AudioSource> _objectPool;
        private readonly Dictionary<AudioSource, AudioState> _audioState = new();

        [SerializeField] private AudioMixerGroup audioMixerGroup;

        private TweenerCore<float, float, FloatOptions> _task;

        private class AudioState
        {
            public AudioState(bool isPlay)
            {
                IsPlay = isPlay;
            }

            public bool IsPlay { get; set; }
        }


        public WaitForSeconds CheckSeconds { get; set; } = new(1);

        private int _count;

        private void Awake()
        {
            _objectPool = new ObjectPool<AudioSource>(CreateFunc);
        }

        private AudioSource CreateFunc()
        {
            var au = gameObject.AddComponent<AudioSource>();
            au.outputAudioMixerGroup = audioMixerGroup;
            _audioState.Add(au, new AudioState(false));
            return au;
        }


        public AudioSource Get()
        {
            var au = _objectPool.Get();
            _audioState[au].IsPlay = true;
            return au;
        }

        public void Play(AudioClip clip)
        {
            var au = Get();
            au.clip = clip;
            au.Play();
        }

        public void SetPitch(float startValue, float endValue, float duration)
        {
            _task?.Kill();
            SetValue(startValue);
            _task = DOTween.To(GetValue, SetValue, endValue, duration);
        }


        private void SetValue(float value) => audioMixerGroup.audioMixer.SetFloat("SoundPitch", value);

        public float GetValue()
        {
            audioMixerGroup.audioMixer.GetFloat("SoundPitch", out var value);
            return value;
        }

        private IEnumerator Start()
        {
            while (true)
            {
                foreach (var state in _audioState)
                {
                    if (state.Key.isPlaying || !state.Value.IsPlay) continue;

                    _objectPool.Release(state.Key);
                    state.Value.IsPlay = false;
                }

                yield return CheckSeconds;
            }
        }
    }
}