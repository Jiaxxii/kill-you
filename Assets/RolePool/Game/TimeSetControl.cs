using System;
using DG.Tweening;
using RolePool.Player;
using TMPro;
using UnityEngine;

namespace RolePool.Game
{
    public class TimeSetControl : MonoBehaviour
    {
        [SerializeField] private float lowTimeValue = 0.5F;
        [SerializeField] private float duration;

        private bool _isLowTime;

        private TimeState _timeState;

        private AudioManager _audioManager;
        private PlayerControl _playerControl;

        private enum TimeState
        {
            One,
            Low,
            InLow,
            Up,
            InUp
        }

        private void Awake()
        {
            _audioManager = FindObjectOfType<AudioManager>();
            _playerControl = FindObjectOfType<PlayerControl>();
        }

        private void Update()
        {
            _isLowTime = Input.GetKey(KeyCode.Space);

            if (_isLowTime)
            {
                FallTime();
            }
            else if (Math.Abs(Time.timeScale - 1f) >= 0.01f)
            {
                SharpTime();
            }
        }


        private void FallTime()
        {
            if (_timeState is TimeState.InLow or TimeState.Low) return;
            _timeState = TimeState.InLow;

            _audioManager.SetPitch(1, lowTimeValue, duration);
            _playerControl.SetSpeed(_playerControl.Speed, _playerControl.Speed / 2, duration);
            DOTween.To(() => Time.timeScale, v => Time.timeScale = v, lowTimeValue, duration)
                .OnComplete(() => _timeState = TimeState.Low);
        }

        private void SharpTime()
        {
            if (_timeState is TimeState.InUp or TimeState.Up) return;
            _timeState = TimeState.InUp;

            _playerControl.SetSpeed(_playerControl.Speed / 2, _playerControl.Speed * 2, duration);
            _audioManager.SetPitch(lowTimeValue, 1, duration);
            DOTween.To(() => Time.timeScale, v => Time.timeScale = v, 1, duration)
                .OnComplete(() => _timeState = TimeState.Up);
        }
    }
}