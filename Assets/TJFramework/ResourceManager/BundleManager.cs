using System;
using UnityEngine;

namespace TJ
{
    /// <summary>
    /// assetName的路径必须以/分隔
    /// bundleName必须全部小写, 且以/分隔
    /// </summary>
    public abstract class BundleManager : MonoBehaviour
    {
        public abstract bool CanClear();
        public abstract void Clear();
        public abstract void Reset();
        public abstract bool AssetExists(string assetName);
        public abstract string AssetBundleName(string assetName);
        public abstract Asset LoadAsset(string assetName);
        public abstract Asset LoadAsset(string assetName, Type type);
        public abstract AssetLoadRequest LoadAssetAsync(string assetName);
        public abstract AssetLoadRequest LoadAssetAsync(string assetName, Type type);
        public abstract Bundle LoadBundle(string bundleName);
        public abstract LoaderLoadRequest LoadBundleAsync(string bundleName);
        public abstract void UnloadUnusedBundles(bool unloadAllLoadedObjects);


        private static BundleManager m_Instance;

        public static BundleManager Instance
        {
            get
            {
                if (m_Instance == null)
                {
#if UNITY_EDITOR
                    if (IsAssetBundleSimulateMode)
                    {
                        CreateInstance<SimulateBundleManager>();
                    }
                    else
#endif
                    {
                        CreateInstance<AssetBundleManager>();
                    }
                }
                return m_Instance;
            }
        }

        static BundleManager CreateInstance<S>() where S : BundleManager
        {
            if (m_Instance == null)
            {
                // Search for existing instance.
                m_Instance = (BundleManager)FindObjectOfType(typeof(BundleManager));

                // Create new instance if one doesn't already exist.
                if (m_Instance == null)
                {
                    // Need to create a new GameObject to attach the singleton to.
                    var singletonObject = new GameObject();
                    m_Instance = singletonObject.AddComponent<S>();
                    singletonObject.name = typeof(S).ToString() + " (Singleton)";

                    // Make instance persistent.
                    DontDestroyOnLoad(singletonObject);
                }
            }

            return m_Instance;
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
