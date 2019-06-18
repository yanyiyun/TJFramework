using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using Object = UnityEngine.Object;


namespace TJ
{
    public class AssetBundleAssetLoadRequest : AssetLoadRequest
    {
        readonly string assetName;
        readonly Type type;
        readonly int mode;
        AssetBundleAsset asset;
        AssetBundleAsset[] allAssets;
        bool complete = false;
        AssetBundleLoaderLoadRequest abllr;


        public override Asset Asset
        {
            get
            {
                return asset;
            }
        }

        public override Asset[] AllAssets
        {
            get
            {
                return allAssets;
            }
        }

        public AssetBundleAssetLoadRequest(AssetBundleBundle bundle, string assetName, Type type, int mode)
        {
            this.assetName = assetName;
            this.type = type;
            this.mode = mode;
            LoadAsset(bundle);
        }

        public AssetBundleAssetLoadRequest()
        {
            //fail
            complete = true;
        }

        public AssetBundleAssetLoadRequest(AssetBundleLoaderLoadRequest abllr, string assetName, Type type, int mode)
        {
            this.abllr = abllr;
            this.assetName = assetName;
            this.type = type;
            this.mode = mode;
        }

        public override bool keepWaiting
        {
            get
            {
                if (abllr != null)
                {
                    if (abllr.keepWaiting)
                        return true;

                    //完成
                    if (abllr.Bundle != null)
                    {
                        LoadAsset(abllr.Bundle as AssetBundleBundle);
                        abllr = null;
                        return true;
                    }
                    else
                    {
                        complete = true;
                        abllr = null;
                        return false;
                    }

                }
                else
                {
                    //等待complete
                    return !complete;
                }
            }
        }

        //------------------------
        void LoadAsset(AssetBundleBundle bundle)
        {
            AssetBundleManager.Instance.StartCoroutine(bundle.LoadAssetAsyncImpl(assetName, type, this, mode));
        }

        public void SetAsset(AssetBundleAsset asset)
        {
            this.asset = asset;
        }

        public void SetAllAssets(AssetBundleAsset[] allAssets)
        {
            this.allAssets = allAssets;
        }

        public void SetComplete()
        {
            complete = true;
        }
    }



    public class AssetBundleLoaderLoadRequest : LoaderLoadRequest
    {
        AssetBundleLoader loader;

        public override Bundle Bundle
        {
            get
            {
                return loader != null ? loader.bundle : null;
            }
        }


        public AssetBundleLoaderLoadRequest(AssetBundleLoader loader)
        {
            this.loader = loader;
        }

        public override bool keepWaiting
        {
            get
            {
                return loader != null && !loader.IsComplete;
            }
        }
    }


}
