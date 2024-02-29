using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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

        [SerializeField] private string[] resourcesRoleName;

        [SerializeField] private float fallY;
        [SerializeField] private Vector2 range;

        [SerializeField] private AudioClip ai, sin, yuk;


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
            var role = _resourcesRole.GetRole(resourcesRoleName[Random.Range(0, resourcesRoleName.Length)]);
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
            var clip = role.Name switch
            {
                "ai" => ai,
                "sin" => sin,
                "yuk" => yuk,
                _ => null
            };

            if (clip == null) return;

            _audioManager.Play(clip);

            _particleManager.Play( /*other.transform.position*/ role.Transform.position);
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