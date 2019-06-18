#if UNITY_EDITOR


namespace TJ
{
    public class SimulateAssetLoadRequest : AssetLoadRequest
    {
        readonly SimulateAsset asset;
        readonly SimulateAsset[] allAssets;

        public SimulateAssetLoadRequest(SimulateAsset asset)
        {
            this.asset = asset;
            this.allAssets = new SimulateAsset[0];
        }

        public SimulateAssetLoadRequest(SimulateAsset[] allAssets)
        {
            if (allAssets.Length > 0)
                this.asset = allAssets[0];
            this.allAssets = allAssets;
        }

        public override Asset Asset { get { return asset; } }

        public override Asset[] AllAssets { get { return allAssets; } }

        public override bool keepWaiting { get { return false; } }
    }

    public class SimulateLoaderLoadRequest : LoaderLoadRequest
    {
        readonly SimulateBundle bundle;

        public override Bundle Bundle { get { return bundle; } }

        public SimulateLoaderLoadRequest(SimulateBundle bundle)
        {
            this.bundle = bundle;
        }

        public override bool keepWaiting { get { return false; } }
    }

}

#endif
