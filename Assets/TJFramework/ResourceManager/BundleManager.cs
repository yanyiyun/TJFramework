using System;


namespace TJ
{
    public abstract class BundleManager : SingletonC<BundleManager>
    {
        public abstract void Clear();
        public abstract void Reset();
        public abstract Asset LoadAsset(string assetName);
        public abstract Asset LoadAsset(string assetName, Type type);
        public abstract AssetLoadRequest LoadAssetAsync(string assetName);
        public abstract AssetLoadRequest LoadAssetAsync(string assetName, Type type);
        public abstract Bundle LoadBundle(string bundleName);
        public abstract LoaderLoadRequest LoadBundleAsync(string bundleName);
        public abstract void UnloadUnusedBundles(bool unloadAllLoadedObjects);
    }


}
