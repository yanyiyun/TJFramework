#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;


namespace TJ
{
    public class SimulateBundleManager : BundleManager
    {
        Dictionary<string, SimulateBundle> bundles = new Dictionary<string, SimulateBundle>();

        public override void Clear()
        {
            bundles.Clear();
        }

        public override void Reset()
        {
            Clear();
        }

        public override Asset LoadAsset(string assetName)
        {
            return LoadAsset(assetName, typeof(Object));
        }

        public override Asset LoadAsset(string assetName, Type type)
        {
            Object rawasset = AssetDatabase.LoadAssetAtPath(assetName, type);
            if (rawasset == null)
                return null;

            string bundleName = ResourceUtils.ConvertToABName(assetName) + ".ab";
            SimulateBundle bundle = LoadBundle(bundleName) as SimulateBundle;

            return new SimulateAsset(rawasset, assetName, bundle);
        }

        public override AssetLoadRequest LoadAssetAsync(string assetName)
        {
            return LoadAssetAsync(assetName, typeof(Object));
        }

        public override AssetLoadRequest LoadAssetAsync(string assetName, Type type)
        {
            Object rawasset = AssetDatabase.LoadAssetAtPath(assetName, type);
            if (rawasset == null)
                return new SimulateAssetLoadRequest(null);

            string bundleName = ResourceUtils.ConvertToABName(assetName) + ".ab";
            SimulateBundle bundle = LoadBundle(bundleName) as SimulateBundle;

            var asset =  new SimulateAsset(rawasset, assetName, bundle);
            return new SimulateAssetLoadRequest(asset);
        }

        public override Bundle LoadBundle(string bundleName)
        {
#if DEBUG
            if (System.Text.RegularExpressions.Regex.IsMatch(bundleName, "[A-Z]"))
                Debug.LogError("AssertBundle name must be lowercase letters");
#endif

            SimulateBundle bundle;
            if (!bundles.TryGetValue(bundleName, out bundle))
            {
                bundle = new SimulateBundle(bundleName);
                bundles.Add(bundleName, bundle);
            }
            return bundle;
        }

        public override LoaderLoadRequest LoadBundleAsync(string bundleName)
        {
#if DEBUG
            if (System.Text.RegularExpressions.Regex.IsMatch(bundleName, "[A-Z]"))
                Debug.LogError("AssertBundle name must be lowercase letters");
#endif

            SimulateBundle bundle;
            if (!bundles.TryGetValue(bundleName, out bundle))
            {
                bundle = new SimulateBundle(bundleName);
                bundles.Add(bundleName, bundle);
            }

            return new SimulateLoaderLoadRequest(bundle);
        }

        public override void UnloadUnusedBundles(bool unloadAllLoadedObjects) { }
    }


}

#endif