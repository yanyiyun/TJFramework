using UnityEngine;
using UnityEditor;

namespace TJ
{
    public class FrameworkMenuItems
    {
        [MenuItem("TJ/Builde AssetBundles")]
        static void BuildAssetBundles()
        {
            AssetBundleBuilder builder = new AssetBundleBuilder(new AssetBundlePathResolver(), Config.AssetBundleBuildRulePath);
            builder.Begin();
            builder.ParseRule();
            builder.Export();
            builder.End();
        }

        const string kSimulationMode = "TJ/Simulation Mode";

        [MenuItem(kSimulationMode)]
        public static void ToggleSimulationMode()
        {
            BundleManager.IsAssetBundleSimulateMode = !BundleManager.IsAssetBundleSimulateMode;
        }

        [MenuItem(kSimulationMode, true)]
        public static bool ToggleSimulationModeValidate()
        {
            Menu.SetChecked(kSimulationMode, BundleManager.IsAssetBundleSimulateMode);
            return true;
        }

    }
}
