using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using RolePool.Game;
using UnityEngine;
using Random = UnityEngine.Random;

namespace RolePool.Player
{
    public class PlayerControl : MonoBehaviour
    {
        private Camera _mainCamera;
        [SerializeField] private GameObject player;

        [SerializeField] private int resolution = 100;

        // [SerializeField] private float speed = 1;
        // [SerializeField] private bool isCreateBezierCurveMove;
        [SerializeField] private Vector2 offset;

        [SerializeField] private GameObject shakeText;
        [SerializeField] private float speed;

        public float Speed => speed;

        private WaterFallControl _waterFallControl;
        private bool _isRun;

        private TweenerCore<float, float, FloatOptions> _task;

        private void Awake()
        {
            _mainCamera = Camera.main;
            _waterFallControl = FindObjectOfType<WaterFallControl>();
            Application.targetFrameRate = 90;
        }


        // private IEnumerator Start()
        // {
        //     while (true)
        //     {
        //         if (!isCreateBezierCurveMove)
        //         {
        //             player.transform.position = GetMousePosition();
        //             yield return null;
        //             continue;
        //         }
        //
        //         foreach (var nextPos in CreateBezierCurve(player.transform.position, GetMousePosition()))
        //         {
        //             while (Vector3.Distance(player.transform.position, nextPos) >= 0.001f)
        //             {
        //                 player.transform.position = Vector3.MoveTowards(player.transform.position, nextPos, Time.deltaTime * speed);
        //                 yield return null;
        //             }
        //         }
        //
        //         yield return null;
        //     }
        // }

        private void Update()
        {
            player.transform.position = Vector3.MoveTowards(player.transform.position, GetMousePosition(), Time.deltaTime * speed);

            if (Input.GetMouseButtonDown(0))
            {
                DoRotate();
            }

            if (Input.GetKeyDown(KeyCode.Tab))
            {
                shakeText.SetActive(!shakeText.activeSelf);
            }

            if (Input.GetKeyDown(KeyCode.Escape)) Application.Quit();
        }

        public void SetSpeed(float startValue, float endValue, float duration)
        {
            _task?.Kill();
            speed = startValue;
            _task = DOTween.To(() => speed, v => speed = v, endValue, duration);
        }

        private void DoRotate()
        {
            if (_isRun) return;
            _isRun = true;
            player.transform.DORotate(new Vector3(0, 0, 40), 0.25f).SetLoops(2, LoopType.Yoyo).OnComplete(() =>
            {
                _isRun = false;
                _waterFallControl.Fall(new Vector3(transform.position.x + offset.x, transform.position.y + offset.y, transform.position.z));
            });
        }


        private Vector3 GetMousePosition()
        {
            var pos = _mainCamera.ScreenToWorldPoint(Input.mousePosition);
            return new Vector3(pos.x, pos.y, player.transform.position.z);
        }

        private Vector3[] CreateBezierCurve(Vector3 start, Vector3 end)
        {
            var control = GenerateControlPoint(start, end);
            Vector3[] curvePoints = new Vector3[resolution];
            float t = 0.0f;
            float step = 1.0f / (resolution - 1);

            for (int i = 0; i < resolution; i++)
            {
                curvePoints[i] = CalculateBezierPoint(t, start, control, end);
                t += step;
            }

            return curvePoints;
        }

        // 使用二次贝塞尔曲线公式计算曲线上的一个点  
        private Vector3 CalculateBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2)
        {
            float u = 1 - t;
            float tt = t * t;
            float uu = u * u;

            Vector3 p = uu * p0; // 初始点影响  
            p += 2 * u * t * p1; // 控制点影响  
            p += tt * p2; // 结束点影响  

            return p;
        }

        // 生成二次贝塞尔曲线的控制点  
        private Vector3 GenerateControlPoint(Vector3 start, Vector3 end)
        {
            // 计算起始点和结束点的中点  
            Vector3 midPoint = (start + end) / 2f;

            // 确定控制点的随机偏移方向和大小  
            // 这里我们假设偏移量的大小是固定的，但方向是随机的  
            float offsetMagnitude = 5f; // 你可以根据需要调整这个值  
            Vector3 randomOffset = new Vector3(
                Random.Range(-offsetMagnitude, offsetMagnitude),
                Random.Range(-offsetMagnitude, offsetMagnitude),
                Random.Range(-offsetMagnitude, offsetMagnitude)
            );

            // 为了确保控制点不会在起始点和结束点的连线上，  
            // 我们可以将随机偏移量投影到该连线的垂直平面上。  
            // 首先，计算起始点到结束点的方向向量  
            Vector3 direction = (end - start).normalized;
            // 计算一个垂直于direction的向量（这里我们简单地选择z轴作为参考轴）  
            Vector3 perpendicular = Vector3.Cross(direction, Vector3.forward).normalized;
            // 如果direction和z轴几乎共线（即它们之间的夹角很小），我们选择x轴作为参考轴  
            if (Mathf.Abs(Vector3.Dot(direction, Vector3.forward)) > 0.99f)
            {
                perpendicular = Vector3.Cross(direction, Vector3.right).normalized;
            }

            // 现在我们将随机偏移量投影到垂直平面上  
            Vector3 projectedOffset = Vector3.ProjectOnPlane(randomOffset, direction);
            // 并且为了确保偏移量不是零（在极端情况下可能会发生），我们添加一个小的固定偏移量  
            projectedOffset += perpendicular * Mathf.Max(0.01f, offsetMagnitude / 10f);

            // 最后，我们将投影的偏移量添加到中点上，得到控制点  
            return midPoint + projectedOffset;
        }

        // ... 其他代码 ...  
    }
}