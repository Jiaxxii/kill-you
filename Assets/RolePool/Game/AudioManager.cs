using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace RolePool.Game
{
    public class AudioManager : MonoBehaviour
    {
        private ObjectPool<AudioSource> _objectPool;
        private readonly Dictionary<AudioSource, AudioState> _audioState = new();

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