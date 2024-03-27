using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZGame
{
    public class AssetManager : MonoBehaviour
    {
        // 加载预制体
        public static void LoadPrefab(string name, Action<GameObject> callback)
        {
            // AssetLoader.SELF.Load(firstQueue, typeof(GameObject), name, callback as Action<object>);
        }

    }
}
