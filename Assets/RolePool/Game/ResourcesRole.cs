using System.Collections.Generic;
using RolePool.Expand;
using UnityEngine;

namespace RolePool.Game
{
    public class ResourcesRole
    {
        public ResourcesRole(Transform node)
        {
            _node = node;
        }

        private readonly Transform _node;

        private readonly Dictionary<string, (GameObject prf, Sprite[])> _resource = new();

        public IRole GetRole(string roleName)
        {
            if (_resource.TryGetValue(roleName, out var res))
            {
                return Object.Instantiate(res.prf, _node).GetComponent<IRole>().Init(roleName, res.Item2);
            }

            var obj = Object.Instantiate(Resources.Load<GameObject>($"Role/{roleName}"), _node);
            if (obj == null)
            {
                Debug.LogError($"路径\"Role/{roleName}\"没有游戏对象!");
                return null;
            }

            var body = Resources.LoadAll<Sprite>(roleName);

            if (body == null || body.Length <= 0)
            {
                Debug.LogError($"路径\"roleName\"没有精灵图片!");
                return null;
            }

            res = (obj, body);
            _resource.Add(roleName, res);

            var roleObj = obj.GetComponent<IRole>();
            if (roleObj != null) return roleObj.Init(roleName, res.Item2);


            Debug.LogError($"游戏对象\"{obj.name}\"没有挂载继承自\"{nameof(IRole)}\"接口的组件!");
            return null;
        }
    }
}