using System;


namespace TJ
{
    /// <summary>
    /// assetName的路径必须以/分隔
    /// bundleName必须全部小写, 且以/分隔
    /// </summary>
    public abstract class BundleManager : SingletonC<BundleManager>
    {
        public abstract void Clear();
        public abstract void Reset();
        public abstract bool AssetExists(string assetName);
        public abstract Asset LoadAsset(string assetName);
        public abstract Asset LoadAsset(string assetName, Type type);
        public abstract AssetLoadRequest LoadAssetAsync(string assetName);
        public abstract AssetLoadRequest LoadAssetAsync(string assetName, Type type);
        public abstract Bundle LoadBundle(string bundleName);
        public abstract LoaderLoadRequest LoadBundleAsync(string bundleName);
        public abstract void UnloadUnusedBundles(bool unloadAllLoadedObjects);

        public static void Create()
        {
            if (BundleManager.Instance != null)
                return;

#if UNITY_EDITOR
            if (IsAssetBundleSimulateMode)
            {
                BundleManager.CreateInstance<SimulateBundleManager>();
            }
            else
#endif
            {
                BundleManager.CreateInstance<AssetBundleManager>();
            }
        }

#if UNITY_EDITOR
        static int assetBundleSimulateMode = -1;
        public static bool IsAssetBundleSimulateMode
        {
            get
            {
                if (assetBundleSimulateMode == -1)
                    assetBundleSimulateMode = UnityEditor.EditorPrefs.GetBool("AssetBundleSimulateMode", false) ? 1 : 0;
                return assetBundleSimulateMode != 0;
            }
            set
            {
                int newValue = value ? 1 : 0;
                if (newValue != assetBundleSimulateMode)
                {
                    assetBundleSimulateMode = newValue;
                    UnityEditor.EditorPrefs.SetBool("AssetBundleSimulateMode", value);
                }
            }
        }
#endif

    }


}
