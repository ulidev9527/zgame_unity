using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace ZGame
{

    public class AssetLoader : MonoBehaviour
    {
        readonly static Dictionary<string, AssetLoaderBase> assetDict = new(); // 所有资源
        readonly static List<AssetLoaderBase> callbackList = new(); // 回调检查列表
        readonly static List<AssetLoaderBase> loadingList = new(); // 正在加载的资源
        readonly static Queue<AssetLoaderBase> firstQueue = new(); // 优先加载队列, 此队列中的资源会优先加载
        readonly static Queue<AssetLoaderBase> lazyQueue = new(); // 懒加载队列, 此队列中的资源会在其它资源加载完成后自动在后台加载
        readonly static Dictionary<Type, Type> loaderDict = new(){
           {typeof(GameObject) , typeof(AssetLoaderPrefab)}
        }; // 加载器映射
        readonly static Dictionary<Type, Queue<AssetLoaderBase>> loaderPool = new(); // 加载器池
        static int loadingMaxCount = 5; // 最大加载数量

        // 设置加载器
        public static void SetLoader(Type resType, Type loaderType)
        {
            Debug.LogFormat("set loader:{0} use {1}", resType.FullName, loaderType.FullName);
            loaderDict.Add(resType, loaderType);
        }

        // 获取一个加载器
        public static T GetLoader<T>(Type resType, string resName) where T : AssetLoaderBase
        {
            if (!loaderDict.ContainsKey(resType))
            {
                Debug.LogErrorFormat("load {0}, but loader not found {1}", resName, resType.FullName);
                return null; // 没有加载器
            }

            if (string.IsNullOrEmpty(resName))
            {
                Debug.LogError("load resName is null or empty ");
                return null;
            }

            // 查询加载队列
            if (assetDict.ContainsKey(resName))
            {
                return assetDict[resName] as T;
            }

            T loader = null;
            // 查询加载器对象池
            if (loaderPool.ContainsKey(resType))
            {
                var pool = loaderPool[resType];
                if (pool.Count > 0)
                {
                    loader = pool.Dequeue() as T;
                    loader.name = resName;
                    loader.state = E_AssetLoadState.Create;
                }
            }

            if (loader == null)
            {
                // 创建新的加载器
                Type loaderType = loaderDict[resType];
                loader = loaderType.Assembly.CreateInstance(loaderType.FullName) as T;
                loader.name = resName;
                loader.state = E_AssetLoadState.Create;
                assetDict[resName] = loader;
            }
            return loader;
        }

        // 优先加载
        public static void Load(Queue<AssetLoaderBase> queue, Type resType, string resName, Action<object> callback)
        {
            // 获取加载器
            AssetLoaderPrefab loader = GetLoader<AssetLoaderPrefab>(resType, resName);
            if (loader == null) return;

            // 添加回调
            loader.AddCallback(callback);

            // 根据状态执行对应逻辑
            switch (loader.state)
            {
                case E_AssetLoadState.Create:
                    queue.Enqueue(loader);
                    break;
                case E_AssetLoadState.Loaded:
                    callbackList.Add(loader);
                    break;
            }
        }


        // 加载预制体
        public static void LoadPrefab(string name, Action<GameObject> callback)
        {
            Load(firstQueue, typeof(GameObject), name, callback as Action<object>);
        }

        private void Start() {
            Debug.Log("AssetLoader start");
        }

        // 加载资源逻辑
        private IEnumerator LoadAsset(AssetLoaderBase loader)
        {
            Debug.LogFormat("load {0} use {1}", loader.name, loader.GetType().FullName);
            var aa = Addressables.LoadAssetAsync<object>(loader.name);
            aa.Completed += (res) =>
            {
                Debug.Log(aa.Status);
                Debug.Log(res.Status);
                Debug.LogFormat("load {0} use {1} complete", loader.name, loader.GetType().FullName);
            };
            yield return aa.WaitForCompletion();
        }

        // 更新
        private void Update()
        {
            // 检查是否有回调触发
            for (int i = callbackList.Count - 1; i >= 0; i--)
            {
                var loader = callbackList[i];
                if (loader.state == E_AssetLoadState.Loaded)
                {
                    loader.ExcuteCallback();
                    callbackList.RemoveAt(i);
                }
            }

            // 检查是否有加载完成的
            for (int i = loadingList.Count - 1; i >= 0; i--)
            {
                var loader = loadingList[i];
                if (loader.state == E_AssetLoadState.Loaded)
                {
                    loadingList.RemoveAt(i);
                    callbackList.Add(loader);
                }
            }

            // 检查是否可以加载
            while (true)
            {
                if (loadingList.Count >= loadingMaxCount) break;

                // 优先加载
                if (firstQueue.Count > 0)
                {
                    var loader = firstQueue.Dequeue();
                    loadingList.Add(loader);
                    StartCoroutine(LoadAsset(loader));
                }
                else if (lazyQueue.Count > 0)
                {
                    var loader = firstQueue.Dequeue();
                    loadingList.Add(loader);
                    StartCoroutine(LoadAsset(loader));
                }
                else
                {
                    break;
                }
            }
        }
    }
}
