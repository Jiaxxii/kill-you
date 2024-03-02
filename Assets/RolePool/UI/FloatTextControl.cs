using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Pool;

namespace RolePool.UI
{
    public class FloatTextControl : MonoBehaviour
    {
        private ObjectPool<IFloatText> _objectPool;

        private void Awake()
        {
            _objectPool = new ObjectPool<IFloatText>(CreateFunc);
        }

        private IFloatText CreateFunc()
        {
            return Instantiate(Resources.Load<GameObject>("Float Text"), transform).GetComponent<IFloatText>().CreateFunc(OnRelease);
        }

        private void OnRelease(IFloatText text)
        {
            _objectPool.Release(text);
        }


        public void GetText(string content, Vector2 position, Vector2 offset, float duration, Ease ease = Ease.Linear)
        {
            var obj = _objectPool.Get();
            obj.Run(content, position, offset, duration, ease);
        }
    }
}