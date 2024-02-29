using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Pool;

namespace RolePool.Game
{
    public class WaterFallControl : MonoBehaviour
    {
        private ObjectPool<IWater> _objectPool;
        private BuildRoleControl _buildRoleControl;

        private void Awake()
        {
            _objectPool = new ObjectPool<IWater>(CreateFunc);
            _buildRoleControl = FindObjectOfType<BuildRoleControl>();
        }

        public void Fall(Vector3 position)
        {
            var water = _objectPool.Get();
            water.OnGet(position);
        }

        private IWater CreateFunc()
        {
            var water = Instantiate(Resources.Load<GameObject>("Water"), transform).GetComponent<IWater>();
            water.OnTriggerEnterEventHandle += OnTriggerEnterEvent;
            return water;
        }

        private void OnTriggerEnterEvent(IWater water, Collider2D other)
        {
            if (!other.gameObject.CompareTag("Role")) return;
            _buildRoleControl.Hurt(other.gameObject.GetComponent<IRole>());
        }
    }
}