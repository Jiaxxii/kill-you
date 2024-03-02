using System;
using System.Collections;
using System.Collections.Generic;
using RolePool.Expand;
using RolePool.UI;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace RolePool.Game
{
    public class BuildRoleControl : MonoBehaviour
    {
        private ObjectPool<IRole> _objectPool;
        private ResourcesRole _resourcesRole;
        private AudioManager _audioManager;
        private ParticleManager _particleManager;
        private FloatTextControl _floatTextControl;

        // [SerializeField] private string[] resourcesRoleName;

        [SerializeField] private float fallY;
        [SerializeField] private Vector2 range;

        // [SerializeField] private AudioClip ai, sin, yuk;
        // [SerializeField] private AudioClip 上位男, 女客人, 心乃;

        [SerializeField] private Audio[] resourceAudio;

        // private readonly Dictionary<IRole, RoleState> _hurtMap = new();

        //  private readonly List<(IRole role, RoleState roleState)> _roles = new();
        public event Action<IRole> OnRolStateChangeEventHandle;

        public int RoleCount { get; private set; }
        public int HurtCount { get; private set; }
        public int SafeCount => RoleCount - HurtCount;


        public event Action<IRole> OnHurtEventHandle;

        [Serializable]
        private class Audio
        {
            [SerializeField] private string name;
            [SerializeField] private AudioClip[] clips;

            public string Name => name;
            public bool IsCompare(string auName) => name == auName;

            public AudioClip GetClip(int index = -1)
            {
                if (index == -1) index = Random.Range(0, clips.Length);
                return clips[index % clips.Length];
            }
        }

        // public enum State
        // {
        //     Create,
        //     Ready,
        //     Injury
        // }
        //
        // public class RoleState
        // {
        //     public RoleState(State state, int injuryCount)
        //     {
        //         State = state;
        //         InjuryCount = injuryCount;
        //     }
        //
        //     public State State { get; set; }
        //     public int InjuryCount { get; set; }
        //
        //     public void ReSet()
        //     {
        //         State = State.Ready;
        //         InjuryCount = 0;
        //     }
        // }


        private void Awake()
        {
            _resourcesRole = new ResourcesRole(transform);
            _objectPool = new ObjectPool<IRole>(CreateFunc, GetFunc);

            _audioManager = FindObjectOfType<AudioManager>();
            _particleManager = FindObjectOfType<ParticleManager>();
            _floatTextControl = FindObjectOfType<FloatTextControl>();
        }


        private void GetFunc(IRole role)
        {
            role.OnGet();
            RoleCount++;
            UpDataUi(role);
            // _hurtMap[obj].ReSet();
        }

        private IRole CreateFunc()
        {
            var role = _resourcesRole.GetRole(resourceAudio[Random.Range(0, resourceAudio.Length)].Name);

            // _hurtMap.Add(role, new RoleState(State.Create, 0));
            role.OnReleaseEventHandler += OnReleaseEvent;
            role.OnCollisionEnterEventHandler += OnCollisionEnterEvent;
            return role;
        }

        private void OnCollisionEnterEvent(IRole role, Collision2D other)
        {
            if (!other.gameObject.CompareTag("Mouse Item")) return;
            Hurt(role);
        }

        public void Hurt(IRole role)
        {
            AudioClip audioClip = null;
            foreach (var au in resourceAudio)
            {
                if (!au.IsCompare(role.Name)) continue;
                audioClip = au.GetClip();
                break;
            }

            if (audioClip == null) return;

            _audioManager.Play(audioClip);

            _particleManager.Play( /*other.transform.position*/ role.Transform.position);
            // _hurtMap[role].State = State.Injury;
            // _hurtMap[role].InjuryCount++;
            OnHurtEventHandle?.Invoke(role);
            HurtCount++;
            OnRolStateChangeEventHandle?.Invoke(role);
            UpDataUi(role);

            SetText(role.Transform.position);
            // Debug.Log(other.position);
            // Debug.Break();
        }

        private void SetText(Vector2 position)
        {
            _floatTextControl.GetText(new[] { "啊", "好烫", "烫烫烫烫" }.GetRandomItem(), position, Vector2.one * Random.Range(3, 10), Random.Range(0.5f, 3f));
        }

        private void OnReleaseEvent(IRole role)
        {
            _objectPool.Release(role);
            UpDataUi(role);
        }

        private void UpDataUi(IRole role)
        {
            OnRolStateChangeEventHandle?.Invoke(role);
        }

        private IEnumerator Start()
        {
            while (true)
            {
                var role = _objectPool.Get();
                role.SetPosition(fallY, range, Random.Range(0.05f, 0.95f));

                yield return new WaitForSeconds(Random.Range(0.25f, 1.5f));
            }
        }
    }
}