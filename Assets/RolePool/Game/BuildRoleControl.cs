using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Pool;
using Random = UnityEngine.Random;

namespace RolePool.Game
{
    public class BuildRoleControl : MonoBehaviour
    {
        private ObjectPool<IRole> _objectPool;
        private ResourcesRole _resourcesRole;
        private AudioManager _audioManager;
        private ParticleManager _particleManager;

        // [SerializeField] private string[] resourcesRoleName;

        [SerializeField] private float fallY;
        [SerializeField] private Vector2 range;

        // [SerializeField] private AudioClip ai, sin, yuk;
        // [SerializeField] private AudioClip 上位男, 女客人, 心乃;

        [SerializeField] private Audio[] resourceAudio;

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


        private void Awake()
        {
            _resourcesRole = new ResourcesRole(transform);
            _objectPool = new ObjectPool<IRole>(CreateFunc, GetFunc);

            _audioManager = FindObjectOfType<AudioManager>();
            _particleManager = FindObjectOfType<ParticleManager>();
        }


        private static void GetFunc(IRole obj) => obj.OnGet();

        private IRole CreateFunc()
        {
            var role = _resourcesRole.GetRole(resourceAudio[Random.Range(0, resourceAudio.Length)].Name);
            role.OnReleaseEventHandler += () => _objectPool.Release(role);
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
            // var clip = role.Name switch
            // {
            //     "ai" => ai,
            //     "sin" => sin,
            //     "yuk" => yuk,
            //     "上位男" => 上位男,
            //     "女客人" => 女客人,
            //     "心乃" => 心乃,
            //     _ => null
            // };

            if (audioClip == null) return;

            _audioManager.Play(audioClip);

            _particleManager.Play( /*other.transform.position*/ role.Transform.position);
            OnHurtEventHandle?.Invoke(role);
        }


        private IEnumerator Start()
        {
            while (true)
            {
                var role = _objectPool.Get();
                role.SetPosition(fallY, range, Random.Range(0.05f, 0.95f));

                yield return new WaitForSeconds(Random.Range(0.1f, 1f));
            }
        }
    }
}