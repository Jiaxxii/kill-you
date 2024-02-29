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
    public class Water : MonoBehaviour , IWater
    {
        
        public void OnGet(Vector3 startPosition)
        {
            transform.position = startPosition;
            gameObject.SetActive(true);
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
            OnTriggerEnterEventHandle?.Invoke(this,other);
            
            if (!other.gameObject.CompareTag("Exit")) return;
            OnRelease();
        }


    }
}