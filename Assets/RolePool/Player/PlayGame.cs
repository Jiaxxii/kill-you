using System;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RolePool.Game;
using UnityEngine;
using Object = UnityEngine.Object;

namespace RolePool.Player
{
    public static class PlayGame
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void SetGameTime()
        {
            var file = Path.Combine(Application.persistentDataPath, "run game.json");

            if (!File.Exists(file)) File.Create(file).Close();

            Application.quitting += () =>
            {
                var content = File.ReadAllText(file);
                var info = Object.FindObjectOfType<BuildRoleControl>();

                var jObject = new JObject();
                if (!string.IsNullOrEmpty(content))
                {
                    if (!content.StartsWith('{') || !content.EndsWith('}'))
                    {
                        File.WriteAllText(file, string.Empty);
                    }
                    else
                        jObject = JObject.Parse(content);
                }

                var item = new JObject
                {
                    ["run time"] = Time.time,
                    ["role count"] = info.RoleCount,
                    ["collision count"] = info.HurtCount
                };
                jObject[DateTime.Now.Ticks.ToString()] = item;

                File.WriteAllText(file, jObject.ToString(Formatting.None));
            };
        }
    }
}