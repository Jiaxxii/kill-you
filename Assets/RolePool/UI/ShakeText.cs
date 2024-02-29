using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

namespace RolePool.UI
{
    public class ShakeText : MonoBehaviour
    {
        private readonly List<TextMeshProUGUI> _textMeshPros = new();

        [Header("文字")] [SerializeField] private string content;
        [SerializeField] private float fontSize;
        [Header("文字 随机颜色")] [SerializeField] private bool randomColor;
        [SerializeField] private Color fontColor = Color.green;
        [SerializeField] private Vector2 alphaRange = Vector2.up;

        [Header("文字表现")] [SerializeField] private float y;
        [SerializeField] private float duration;
        [SerializeField] private float strength = 3F;

        private void Start()
        {
            CreateShakeText(content);
            Run();
        }

        [SerializeField] private float space;

        private void CreateShakeText(string message)
        {
            var prf = Resources.Load<GameObject>("Text (TMP)");
            var len = 0f;
            foreach (var str in message)
            {
                var text = Instantiate(prf, transform).GetComponent<TextMeshProUGUI>();
                _textMeshPros.Add(text);
                text.fontSize = fontSize;
                text.text = str.ToString();
                text.rectTransform.sizeDelta = new Vector2(text.preferredWidth, text.preferredHeight);
                len += text.preferredWidth;
            }


            var startPos = -((len + (_textMeshPros.Count - 1) * space) / 2);

            foreach (var text in _textMeshPros)
            {
                text.rectTransform.anchoredPosition = new Vector2(startPos, y);
                startPos += text.preferredWidth + space;
            }
        }

        private void Run()
        {
            foreach (var text in _textMeshPros)
            {
                text.color = fontColor;
                DOTween.Shake(() => text.rectTransform.anchoredPosition, v => text.rectTransform.anchoredPosition = v, duration, strength).SetLoops(-1);

                if (!randomColor) return;
                var color = Random.ColorHSV();
                color.a = Random.Range(alphaRange.x, alphaRange.y);
                text.DOColor(color, duration).SetLoops(-1, LoopType.Yoyo);
            }
        }
        
        
    }
}