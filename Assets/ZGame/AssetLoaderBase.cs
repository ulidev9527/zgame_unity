using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZGame
{
    public enum E_AssetLoadState
    {
        Pool,
        Create,
        Queue,
        Loading,
        Loaded
    }
    public class AssetLoaderBase
    {
        public string name = "";
        public E_AssetLoadState state = E_AssetLoadState.Pool;
        public object res = null;

        Queue<Action<UnityEngine.Object>> callbackQueue = new();

        // 添加一个执行回调
        public void AddCallback(Action<object> callback)
        {
            callbackQueue.Enqueue(callback);
        }

        // 执行回调
        public void ExcuteCallback()
        {
            while (callbackQueue.Count > 0)
            {
                var cb = callbackQueue.Dequeue();
                try
                {
                    cb.Invoke((UnityEngine.Object)res);
                }
                catch (System.Exception e)
                {
                    Debug.LogError(e.Message);
                }
            }
        }
    }
}
