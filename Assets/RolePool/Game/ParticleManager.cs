using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace RolePool.Game
{
    public class ParticleManager : MonoBehaviour
    {
        private ObjectPool<ParticleSystem> _objectPool;


        private readonly Dictionary<ParticleSystem, ParticleState> _audioState = new();

        private class ParticleState
        {
            public ParticleState(bool isPlay)
            {
                IsPlay = isPlay;
            }

            public bool IsPlay { get; set; }
        }


        public WaitForSeconds CheckSeconds { get; set; } = new(1);

        private int _count;

        private void Awake()
        {
            _objectPool = new ObjectPool<ParticleSystem>(CreateFunc);
        }

        private ParticleSystem CreateFunc()
        {
            var ps = Instantiate(Resources.Load<GameObject>("Smoke"), transform).GetComponent<ParticleSystem>();
            _audioState.Add(ps, new ParticleState(false));
            return ps;
        }


        public ParticleSystem Get()
        {
            var ps = _objectPool.Get();
            ps.gameObject.SetActive(true);
            _audioState[ps].IsPlay = true;
            return ps;
        }

        public void Play(Vector3 position)
        {
            var ps = Get();
            ps.gameObject.transform.position = position;
            ps.Play();
        }


        private IEnumerator Start()
        {
            while (true)
            {
                foreach (var state in _audioState)
                {
                    if (state.Key.isPlaying || !state.Value.IsPlay) continue;

                    state.Key.gameObject.SetActive(false);
                    _objectPool.Release(state.Key);
                    state.Value.IsPlay = false;
                }

                yield return CheckSeconds;
            }
        }
    }
}