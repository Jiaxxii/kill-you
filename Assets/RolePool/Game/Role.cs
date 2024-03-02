using System;
using System.Collections.Generic;
using RolePool.Expand;
using UnityEngine;
using Random = System.Random;

namespace RolePool.Game
{
    public interface IRole
    {
        public Rigidbody2D Rb { get; }
        public SpriteRenderer SpriteRenderer { get; }
        public GameObject GameObject { get; }
        public Transform Transform { get; }
        public Sprite[] Body { get; }

        public event Action<IRole> OnReleaseEventHandler;
        public event Action<IRole, Collision2D> OnCollisionEnterEventHandler;
        public event Action<IRole, Collider2D> OnTriggerEnterEventHandler;

        public Animator Animator { get; }
        public string Name { get; }


        public void SetPosition(float y, Vector2 range, float t);
        public IRole Init(string roleName, Sprite[] body);

        public void OnGet();
    }

    public class Role : MonoBehaviour, IRole
    {
        public Rigidbody2D Rb { get; private set; }
        public SpriteRenderer SpriteRenderer { get; private set; }
        public GameObject GameObject { get; private set; }
        public Transform Transform { get; private set; }
        public Sprite[] Body { get; private set; }

        public event Action<IRole> OnReleaseEventHandler;

        public event Action<IRole, Collision2D> OnCollisionEnterEventHandler;
        public event Action<IRole, Collider2D> OnTriggerEnterEventHandler;

        public Animator Animator { get; private set; }
        public string Name { get; private set; }


        private void Awake()
        {
            Rb = GetComponent<Rigidbody2D>();
            SpriteRenderer = GetComponent<SpriteRenderer>();
            GameObject = gameObject;
            Transform = transform;
            Animator = GetComponent<Animator>();
        }

        public IRole Init(string roleName, Sprite[] body)
        {
            Name = roleName;
            Body = body;
            return this;
        }

        public void SetPosition(float y, Vector2 range, float t)
        {
            Transform.position = new Vector3(Mathf.Lerp(range.x, range.y, t), y, transform.position.z);
        }

        public void OnGet()
        {
            ResetRigidBody2D();
            SetRandomBody();
            GameObject.SetActive(true);

            Invoke(nameof(OnRelease), UnityEngine.Random.Range(10, 20));
        }

        public void ResetRigidBody2D()
        {
            Rb.velocity = Vector2.zero; // 重置速度  
            Rb.angularVelocity = 0f; // 重置角速度  
            Rb.rotation = 0f; // 假设你是以Z轴作为旋转轴
            Transform.eulerAngles = Vector3.zero;

            // 可能还需要其他属性的重置，具体根据你的游戏需求  
        }

        private void OnRelease()
        {
            OnReleaseEventHandler?.Invoke(this);
            GameObject.SetActive(false);
        }


        private void SetRandomBody()
        {
            if (Body.Length == 1) SpriteRenderer.sprite = Body[0];
            SpriteRenderer.sprite = Body[UnityEngine.Random.Range(0, Body.Length)];
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            OnCollisionEnterEventHandler?.Invoke(this, other);
            Hurt(other.gameObject);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            OnTriggerEnterEventHandler?.Invoke(this, other);
            Hurt(other.gameObject);
        }


        private void Hurt(GameObject obj)
        {
            if (!obj.CompareTag("Mouse Item")) return;
            Animator.Play("ai_hurt");
        }
    }
}