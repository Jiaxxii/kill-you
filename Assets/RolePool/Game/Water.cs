using System;
using UnityEngine;

namespace RolePool.Game
{
    public interface IWater
    {
        public void OnGet(Vector3 startPosition);

        public event Action<IWater> OnReleaseEventHandle;
        public event Action<IWater, Collider2D> OnTriggerEnterEventHandle;
    }

    public class Water : MonoBehaviour, IWater
    {
        [SerializeField] private Rigidbody2D rb;
        [SerializeField] private float force;

        public void OnGet(Vector3 startPosition)
        {
            transform.position = startPosition;
            gameObject.SetActive(true);
            rb.AddForce(Vector2.down * force);
        }

        private void OnRelease()
        {
            gameObject.SetActive(false);
            OnReleaseEventHandle?.Invoke(this);
        }

        public event Action<IWater> OnReleaseEventHandle;
        public event Action<IWater, Collider2D> OnTriggerEnterEventHandle;


        private void OnTriggerEnter2D(Collider2D other)
        {
            OnTriggerEnterEventHandle?.Invoke(this, other);

            if (!other.gameObject.CompareTag("Exit")) return;
            OnRelease();
        }
    }
}