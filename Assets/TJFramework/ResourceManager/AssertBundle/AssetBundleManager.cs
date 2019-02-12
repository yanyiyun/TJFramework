using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using Object = UnityEngine.Object;


namespace TJ
{

    /// <summary>
    /// 注意:
    /// AssetBunlde同步和异步加载无法并行. 但是AssetBundle里面的至于加载是可以同步和异步并行的
    /// </summary>
    public class AssetBundleManager : BundleManager
    {
        AssetBundleManifest manifest;
        //asset在那个bundle. key会被转换成小写
        Dictionary<string, string> assets;
        //bundle包含的asset
        Dictionary<string, List<string>> bundles;

        Dictionary<string, AssetBundleLoader> loaders = new Dictionary<string, AssetBundleLoader>();

        protected override void Init()
        {
            Reset();
        }

        /// <summary>
        /// 清理所有资源. 这应该只在热更新结束后才会调用.
        /// 因为又很多副作用. 
        /// 完整的资源清理.
        /// 如果有异步加载中的AssetBundle, 后续会出错
        /// 如果有异步加载中的Asset, 后续会出错.
        /// </summary>
        public override void Clear()
        {
            manifest = null;
            assets = null;
            bundles = null;

            List<string> loadingBundles = new List<string>();
            List<string> loadingAssets = new List<string>();
            this.StopAllCoroutines();
            foreach (var loader in loaders.Values)
            {
                if (loader.state == AssetBundleLoader.State.Loading)
                {
                    loadingBundles.Add(loader.bundleName);
                }

                if (loader.bundle != null)
                {
                    if (loader.bundle.IsLoadingAsync)
                        loadingAssets.Add(loader.bundle.BundleName);
                    loader.bundle.Dispose(true);
                }
            }
            loaders.Clear();

            foreach (var str in loadingBundles)
            {
                Debug.LogWarningFormat("AssetBundleLoader '{0}' is destroyed, but in loading!", str);
            }
            foreach (var str in loadingBundles)
            {
                Debug.LogWarningFormat("AssetBundleBundle '{0}' is destroyed, but in loading!", str);
            }
        }

        public override void Reset()
        {
            Clear();

            string abmPath = ResourceUtils.FullPathForAssetBundleApi(FilePath(ResourceUtils.AssetBundleFolder));
            if (abmPath == "")
                throw new Exception("AssetBundleManifest file CAN NOT be located");

            AssetBundle ab = AssetBundle.LoadFromFile(abmPath);
            manifest = ab.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
            ab.Unload(false);


            assets = new Dictionary<string, string>();
            bundles = new Dictionary<string, List<string>>();
            byte[] jstr = ResourceUtils.LoadBytes(FilePath(ResourceUtils.AssetBundleFileList));
            var abfl = JsonUtility.FromJson<AssetBundleFileList>(System.Text.Encoding.UTF8.GetString(jstr));
            foreach (var li in abfl.list)
            {
                string[] arr = li.Split(new char[] { '|' }, System.StringSplitOptions.RemoveEmptyEntries);
                string asset = arr[0].Trim().ToLower();
                string bundle = arr[1].Trim();

                assets[asset] = bundle;

                List<string> bl;
                if (!bundles.TryGetValue(bundle, out bl))
                {
                    bl = new List<string>();
                    bundles[bundle] = bl;
                }
                bl.Add(asset);
            }
        }

        public string FilePath(string path)
        {
            return Path.Combine(ResourceUtils.AssetBundleFolder, path);
        }

        public string GetBundleNameFromAssetList(string assetName)
        {
            assetName = assetName.ToLower();
            string bundleName;
            assets.TryGetValue(assetName, out bundleName);
            if (bundleName == null)
                Debug.LogErrorFormat("CANNOT locate {0} in assetlist!", assetName);
            return bundleName;
        }

        public override Asset LoadAsset(string assetName)
        {
            return LoadAsset(assetName, typeof(Object));
        }

        public override Asset LoadAsset(string assetName, Type type)
        {
            string bundleName = GetBundleNameFromAssetList(assetName);
            if (bundleName == null)
                return null;
            var bundle = LoadBundle(bundleName);
            return bundle.LoadAsset(assetName, type);
        }

        public override AssetLoadRequest LoadAssetAsync(string assetName)
        {
            return LoadAssetAsync(assetName, typeof(Object));
        }

        public override AssetLoadRequest LoadAssetAsync(string assetName, Type type)
        {
            string bundleName = GetBundleNameFromAssetList(assetName);
            if (bundleName == null)
                return new AssetBundleAssetLoadRequest();   //fail;

            if (loaders.ContainsKey(bundleName))
            {
                AssetBundleLoader loader = loaders[bundleName];
                if (loader.IsComplete)
                {
                    if (loader.bundle != null)
                        return new AssetBundleAssetLoadRequest(loader.bundle, assetName, type);
                    else
                        return new AssetBundleAssetLoadRequest();   //fail
                }
                else
                {
                    Debug.Assert(loader.state == AssetBundleLoader.State.Loading);
                    return new AssetBundleAssetLoadRequest(LoadBundleAsync(bundleName) as AssetBundleLoaderLoadRequest, assetName, type);
                }
            }
            else
            {
                return new AssetBundleAssetLoadRequest(LoadBundleAsync(bundleName) as AssetBundleLoaderLoadRequest, assetName, type);
            }
        }


        public override Bundle LoadBundle(string bundleName)
        {
#if DEBUG
            if (System.Text.RegularExpressions.Regex.IsMatch(bundleName, "[A-Z]"))
                Debug.LogError("AssertBundle name must be lowercase letters");
#endif

            AssetBundleLoader loader;
            if (loaders.TryGetValue(bundleName, out loader))
            {
                if (loader.state == AssetBundleLoader.State.Loading)
                    Debug.LogError(string.Format("AssetBundleLoader '{0}' is loading aync, CANNOT load sync!", loader.bundleName));
                return loader.bundle;
            }


            if (!bundles.ContainsKey(bundleName))
            {
                Debug.LogError("CANNOT find AssetBundle: " + bundleName);
                return null;
            }


            loader = LoadBundleAndDepImpl(bundleName);
            return loader != null ? loader.bundle : null;
        }


        AssetBundleLoader LoadBundleAndDepImpl(string bundleName)
        {
            AssetBundleLoader abLoader;
            if (loaders.TryGetValue(bundleName, out abLoader))
            {
                return abLoader;
            }

            string[] deps = manifest.GetDirectDependencies(bundleName);
            foreach (var dep in deps)
            {
                //递归加载Loader
                LoadBundleAndDepImpl(dep);
            }

            //创建
            abLoader = new AssetBundleLoader
            {
                bundleName = bundleName,
                bundleFullPath = ResourceUtils.FullPathForAssetBundleApi(FilePath(bundleName)),
                waitDepCount = deps.Length,
            };
            bool isAyncCollision = false;
            foreach (var dep in deps)
            {
                AssetBundleLoader deplo;
                if (loaders.TryGetValue(dep, out deplo))
                {
                    if (deplo.IsComplete)
                    {
                        abLoader.DepComplete(deplo.state == AssetBundleLoader.State.Error);
                    }
                    else
                    {
                        Debug.Assert(deplo.state == AssetBundleLoader.State.Loading, "AssetBundleLoader must be loading");
                        Debug.LogError(string.Format("AssetBundleLoader '{0}' is loading aync, CANNOT load sync!", dep));
                        isAyncCollision = true;
                    }
                }
                else
                    isAyncCollision = true;
            }

            //异步冲突导致的错误, 就不要加入管理器. 这样可以确保下次可以正常被加载
            if (isAyncCollision)
                return null;

            Debug.Assert(!loaders.ContainsKey(bundleName), "AssetBundleLoader exists.");
            loaders[bundleName] = abLoader;

            abLoader.Load();
            Debug.Assert(abLoader.IsComplete, "AssetBundleLoader load sync must complete");
            return abLoader;
        }


        public override LoaderLoadRequest LoadBundleAsync(string bundleName)
        {
#if DEBUG
            if (System.Text.RegularExpressions.Regex.IsMatch(bundleName, "[A-Z]"))
                Debug.LogError("AssertBundle name must be lowercase letters");
#endif

            AssetBundleLoader loader;
            if (loaders.TryGetValue(bundleName, out loader))
            {
                return new AssetBundleLoaderLoadRequest(loader);
            }

            if (!bundles.ContainsKey(bundleName))
            {
                Debug.LogError("CANNOT find AssetBundle: " + bundleName);
                return new AssetBundleLoaderLoadRequest(null);
            }

            loader = LoadBundleAndDepAsyncImpl(bundleName);
            return new AssetBundleLoaderLoadRequest(loader);
        }


        AssetBundleLoader LoadBundleAndDepAsyncImpl(string bundleName)
        {
            AssetBundleLoader abLoader;
            if (loaders.TryGetValue(bundleName, out abLoader))
            {
                return abLoader;
            }

            string[] deps = manifest.GetDirectDependencies(bundleName);
            foreach (var dep in deps)
            {
                //递归加载Loader
                LoadBundleAndDepAsyncImpl(dep);
            }

            //创建
            abLoader = new AssetBundleLoader
            {
                bundleName = bundleName,
                bundleFullPath = ResourceUtils.FullPathForAssetBundleApi(FilePath(bundleName)),
                waitDepCount = deps.Length,
            };
            foreach (var dep in deps)
            {
                var deplo = loaders[dep];
                if (deplo.IsComplete)
                {
                    abLoader.DepComplete(deplo.state == AssetBundleLoader.State.Error);
                }
                else
                {
                    deplo.OnComplete += abLoader.DepComplete;
                }
            }

            Debug.Assert(!loaders.ContainsKey(bundleName), "AssetBundleLoader exists.");
            loaders[bundleName] = abLoader;

            StartCoroutine(abLoader.LoadAsync());
            return abLoader;
        }


        /// <summary>
        /// 移除无用的Bundle. 就是没有被引用的
        /// 这个函数和Resources.UnloadUnusedAssets();配合使用, 效果更佳哦
        /// </summary>
        public override void UnloadUnusedBundles(bool unloadAllLoadedObjects)
        {
            bool hasUnusedBundle = false;
            do
            {
                List<string> movekeys = new List<string>();
                foreach (var loader in loaders.Values)
                {
                    if (loader.bundle != null)
                    {
                        if (loader.bundle.IsUnused)
                            movekeys.Add(loader.bundleName);
                    }
                }
                foreach (var key in movekeys)
                {
                    loaders[key].bundle.Dispose(unloadAllLoadedObjects);
                    loaders.Remove(key);
                }
                hasUnusedBundle = movekeys.Count != 0;
            }
            while (hasUnusedBundle);
        }


        //---------------------------------------------------------------------
        public HashSet<AssetBundleBundle> CollectDepAssetBundleBundles(string bundleName)
        {
            HashSet<AssetBundleBundle> set = new HashSet<AssetBundleBundle>();
            string[] deps = manifest.GetDirectDependencies(bundleName);
            foreach (var dep in deps)
            {
                Debug.Assert(loaders[dep].bundle != null);
                set.Add(loaders[dep].bundle);
            }
            return set;
        }

    }



    [System.Serializable]
    public class AssetBundleFileList
    {
        public string[] list;
    }


}
