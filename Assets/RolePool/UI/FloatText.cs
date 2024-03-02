using System;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace RolePool.UI
{
    public interface IFloatText
    {
        public void Run(string content, Vector2 position, Vector2 offset, float duration, Ease ease = Ease.Linear);
        public IFloatText CreateFunc(Action<IFloatText> onRelease);
    }

    public class FloatText : MonoBehaviour, IFloatText
    {
        [SerializeField] private TextMeshProUGUI text;
        private Action<IFloatText> _onReleaseEventHandel;

        public void Run(string content, Vector2 position, Vector2 offset, float duration, Ease ease = Ease.OutQuad)
        {
            text.text = content;
            gameObject.SetActive(true);
            text.alpha = 1;

            text.rectTransform.anchoredPosition = position;
            var currentPos = text.rectTransform.anchoredPosition;
            var target = new Vector2(currentPos.x + offset.x, currentPos.y + offset.y);

            text.DOFade(0, duration).SetEase(ease);
            text.rectTransform.DOAnchorPos(target, duration).SetEase(ease).OnComplete(Release);
        }

        public IFloatText CreateFunc(Action<IFloatText> onRelease)
        {
            _onReleaseEventHandel = onRelease;
            return this;
        }

        private void Release()
        {
            gameObject.SetActive(false);
            _onReleaseEventHandel?.Invoke(this);
        }
    }
}