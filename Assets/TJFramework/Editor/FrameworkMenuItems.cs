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


        const string kAssetBundleSimulationMode = "TJ/Simulation Mode";

        [MenuItem(kAssetBundleSimulationMode)]
        public static void ToggleSimulationMode()
        {
            BundleManager.IsAssetBundleSimulateMode = !BundleManager.IsAssetBundleSimulateMode;
        }

        [MenuItem(kAssetBundleSimulationMode, true)]
        public static bool ToggleSimulationModeValidate()
        {
            Menu.SetChecked(kAssetBundleSimulationMode, BundleManager.IsAssetBundleSimulateMode);
            return true;
        }


        const string kExternLuaMode = "TJ/Extern Lua Mode";

        [MenuItem(kExternLuaMode)]
        public static void ToggleExternLuaMode()
        {
            LuaManager.IsExternLuaMode = !LuaManager.IsExternLuaMode;
        }

        [MenuItem(kExternLuaMode, true)]
        public static bool ToggleExternLuaModeValidate()
        {
            Menu.SetChecked(kExternLuaMode, LuaManager.IsExternLuaMode);
            return true;
        }

    }
}
