#if UNITY_EDITOR
using System;
using Object = UnityEngine.Object;

namespace TJ
{
    public class SimulateBundle : Bundle
    {
        public SimulateBundle(string bundleName)
        {
            BundleName = bundleName;
            IsDispose = false;
        }

        public override string BundleName { get; protected set; }
        public override bool IsDispose { get; protected set; }
        public override void Hold(Object owner) { }
        public override void Return(Object owner) { }

        public override Asset LoadAsset(string assetName)
        {
            return LoadAsset(assetName, typeof(Object));
        }

        public override Asset LoadAsset(string assetName, Type type)
        {
            return SimulateBundleManager.Instance.LoadAsset(assetName, type);
        }

        public override Asset[] LoadAssetWithSubAssets(string assetName)
        {
            return LoadAssetWithSubAssets(assetName, typeof(Object));
        }

        public override Asset[] LoadAssetWithSubAssets(string assetName, Type type)
        {
            return SimulateBundleManager.Instance.LoadAssetWithSubAssets(assetName, type);
        }

        public override AssetLoadRequest LoadAssetAsync(string assetName)
        {
            return LoadAssetAsync(assetName, typeof(Object));
        }

        public override AssetLoadRequest LoadAssetAsync(string assetName, Type type)
        {
            return SimulateBundleManager.Instance.LoadAssetAsync(assetName, type);
        }

        public override AssetLoadRequest LoadAssetWithSubAssetsAsync(string assetName)
        {
            return LoadAssetWithSubAssetsAsync(assetName, typeof(Object));
        }

        public override AssetLoadRequest LoadAssetWithSubAssetsAsync(string assetName, Type type)
        {
            return SimulateBundleManager.Instance.LoadAssetWithSubAssetsAsync(assetName, type);
        }
    }
}

#endif