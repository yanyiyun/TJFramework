/*
 * Tencent is pleased to support the open source community by making xLua available.
 * Copyright (C) 2016 THL A29 Limited, a Tencent company. All rights reserved.
 * Licensed under the MIT License (the "License"); you may not use this file except in compliance with the License. You may obtain a copy of the License at
 * http://opensource.org/licenses/MIT
 * Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the License for the specific language governing permissions and limitations under the License.
*/

using System.Collections.Generic;
using System;
using XLua;
using System.Reflection;
using System.Linq;

//配置的详细介绍请看Doc下《XLua的配置.doc》
public static class ExampleGenConfig
{
    /***************如果你全lua编程，可以参考这份自动化配置***************/
    //--------------begin 纯lua编程配置参考----------------------------
    static List<string> exclude = new List<string> {
        "HideInInspector", "ExecuteInEditMode",
        "AddComponentMenu", "ContextMenu",
        "RequireComponent", "DisallowMultipleComponent",
        "SerializeField", "AssemblyIsEditorAssembly",
        "Attribute", "Types",
        "UnitySurrogateSelector", "TrackedReference",
        "TypeInferenceRules", "FFTWindow",
        "RPC", "Network", "MasterServer",
        "BitStream", "HostData",
        "ConnectionTesterStatus", "GUI", "EventType",
        "EventModifiers", "FontStyle", "TextAlignment",
        "TextEditor", "TextEditorDblClickSnapping",
        "TextGenerator", "TextClipping", "Gizmos",
        "ADBannerView", "ADInterstitialAd",
        "Android", "Tizen", "jvalue",
        "iPhone", "iOS", "Windows", "CalendarIdentifier",
        "CalendarUnit", "CalendarUnit",
        "ClusterInput", "FullScreenMovieControlMode",
        "FullScreenMovieScalingMode", "Handheld",
        "LocalNotification", "NotificationServices",
        "RemoteNotificationType", "RemoteNotification",
        "SamsungTV", "TextureCompressionQuality",
        "TouchScreenKeyboardType", "TouchScreenKeyboard",
        "MovieTexture", "UnityEngineInternal",
        "Terrain", "Tree", "SplatPrototype",
        "DetailPrototype", "DetailRenderMode",
        "MeshSubsetCombineUtility", "AOT", "Social", "Enumerator",
        "SendMouseEvents", "Cursor", "Flash", "ActionScript",
        "OnRequestRebuild", "Ping",
        "ShaderVariantCollection", "SimpleJson.Reflection",
        "CoroutineTween", "GraphicRebuildTracker",
        "Advertisements", "UnityEditor", "WSA",
        "EventProvider", "Apple",
        "ClusterInput", "Motion",
        "UnityEngine.UI.ReflectionMethodsCache", "NativeLeakDetection",
        "NativeLeakDetectionMode", "WWWAudioExtensions", "UnityEngine.Experimental",

        "UnityEngine.CanvasRenderer",
    };

    static List<string> userExclude = new List<string>
    {
        "TJ.SimulateAsset",
        "TJ.SimulateBundle",
        "TJ.SimulateAssetLoadRequest",
        "TJ.SimulateLoaderLoadRequest",
        "TJ.SimulateBundleManager",
    };

    static bool isExcluded(Type type)
    {
        var fullName = type.FullName;
        for (int i = 0; i < exclude.Count; i++)
        {
            if (fullName.Contains(exclude[i]))
            {
                return true;
            }
        }
        return false;
    }

    static bool isUserExcluded(Type type)
    {
        var fullName = type.FullName;
        for (int i = 0; i < userExclude.Count; i++)
        {
            if (fullName.Contains(userExclude[i]))
            {
                return true;
            }
        }
        return false;
    }

    [LuaCallCSharp]
    public static IEnumerable<Type> LuaCallCSharp
    {
        get
        {
            List<string> namespaces = new List<string>() // 在这里添加名字空间
            {
                "UnityEngine",
                "UnityEngine.UI"
            };
            var unityTypes = (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                              where !(assembly.ManifestModule is System.Reflection.Emit.ModuleBuilder)
                              from type in assembly.GetExportedTypes()
                              where type.Namespace != null && namespaces.Contains(type.Namespace) && !isExcluded(type)
                                      && type.BaseType != typeof(MulticastDelegate) && !type.IsInterface && !type.IsEnum
                              select type);

            string[] customAssemblys = new string[] {
                "Assembly-CSharp",
            };
            var customTypes = (from assembly in customAssemblys.Select(s => Assembly.Load(s))
                               from type in assembly.GetExportedTypes()
                               where type.Namespace == null || !type.Namespace.StartsWith("XLua") && !isUserExcluded(type)
                                       && type.BaseType != typeof(MulticastDelegate) && !type.IsInterface && !type.IsEnum
                               select type);
            return unityTypes.Concat(customTypes);
        }
    }

    //自动把LuaCallCSharp涉及到的delegate加到CSharpCallLua列表，后续可以直接用lua函数做callback
    [CSharpCallLua]
    public static List<Type> CSharpCallLua
    {
        get
        {
            var lua_call_csharp = LuaCallCSharp;
            var delegate_types = new List<Type>();
            var flag = BindingFlags.Public | BindingFlags.Instance
                | BindingFlags.Static | BindingFlags.IgnoreCase | BindingFlags.DeclaredOnly;
            foreach (var field in (from type in lua_call_csharp select type).SelectMany(type => type.GetFields(flag)))
            {
                if (typeof(Delegate).IsAssignableFrom(field.FieldType))
                {
                    delegate_types.Add(field.FieldType);
                }
            }

            foreach (var method in (from type in lua_call_csharp select type).SelectMany(type => type.GetMethods(flag)))
            {
                if (typeof(Delegate).IsAssignableFrom(method.ReturnType))
                {
                    delegate_types.Add(method.ReturnType);
                }
                foreach (var param in method.GetParameters())
                {
                    var paramType = param.ParameterType.IsByRef ? param.ParameterType.GetElementType() : param.ParameterType;
                    if (typeof(Delegate).IsAssignableFrom(paramType))
                    {
                        delegate_types.Add(paramType);
                    }
                }
            }
            return delegate_types.Distinct().ToList();
        }
    }

    //C#静态调用Lua的配置（包括事件的原型），仅可以配delegate，interface
    [CSharpCallLua]
    public static List<Type> CSharpCallLuaExt = new List<Type>() {
                    typeof(System.Collections.IEnumerator)
                };

    //--------------end 纯lua编程配置参考----------------------------


    //黑名单
    [BlackList]
    public static List<List<string>> BlackList = new List<List<string>>()  {
                new List<string>(){"System.Xml.XmlNodeList", "ItemOf"},
                new List<string>(){"UnityEngine.WWW", "movie"},
    #if UNITY_WEBGL
                new List<string>(){"UnityEngine.WWW", "threadPriority"},
    #endif
                new List<string>(){"UnityEngine.Texture2D", "alphaIsTransparency"},
                new List<string>(){"UnityEngine.Security", "GetChainOfTrustValue"},
                new List<string>(){"UnityEngine.CanvasRenderer", "onRequestRebuild"},
                new List<string>(){"UnityEngine.Light", "areaSize"},
                new List<string>(){"UnityEngine.Light", "lightmapBakeType"},
                new List<string>(){"UnityEngine.WWW", "MovieTexture"},
                new List<string>(){"UnityEngine.WWW", "GetMovieTexture"},
                new List<string>(){"UnityEngine.AnimatorOverrideController", "PerformOverrideClipListCleanup"},
    #if !UNITY_WEBPLAYER
                new List<string>(){"UnityEngine.Application", "ExternalEval"},
    #endif
                new List<string>(){"UnityEngine.GameObject", "networkView"}, //4.6.2 not support
                new List<string>(){"UnityEngine.Component", "networkView"},  //4.6.2 not support
                new List<string>(){"System.IO.FileInfo", "GetAccessControl", "System.Security.AccessControl.AccessControlSections"},
                new List<string>(){"System.IO.FileInfo", "SetAccessControl", "System.Security.AccessControl.FileSecurity"},
                new List<string>(){"System.IO.DirectoryInfo", "GetAccessControl", "System.Security.AccessControl.AccessControlSections"},
                new List<string>(){"System.IO.DirectoryInfo", "SetAccessControl", "System.Security.AccessControl.DirectorySecurity"},
                new List<string>(){"System.IO.DirectoryInfo", "CreateSubdirectory", "System.String", "System.Security.AccessControl.DirectorySecurity"},
                new List<string>(){"System.IO.DirectoryInfo", "Create", "System.Security.AccessControl.DirectorySecurity"},
                new List<string>(){"UnityEngine.MonoBehaviour", "runInEditMode"},

                //
                new List<string>(){ "UnityEngine.CanvasRenderer", "OnRequestRebuild"},
                new List<string>(){ "TJ.AssetBundlePathResolver", "BundleSavePath"},
                new List<string>(){ "TJ.AssetBundlePathResolver", "GetEditorModePath", "System.String"},
                new List<string>(){ "TJ.BundleManager", "IsAssetBundleSimulateMode"},
                new List<string>(){ "TJ.LuaManager", "IsExternLuaMode"},
                new List<string>(){ "UnityEngine.AnimatorControllerParameter", "name"},
                new List<string>(){ "UnityEngine.AudioSettings", "GetSpatializerPluginNames"},
                new List<string>(){ "UnityEngine.AudioSettings", "SetSpatializerPluginName", "System.String"},
                new List<string>(){ "UnityEngine.UI.Text", "OnRebuildRequested"},
                new List<string>(){ "UnityEngine.Caching", "SetNoBackupFlag", "UnityEngine.CachedAssetBundle"},
                new List<string>(){ "UnityEngine.Caching", "SetNoBackupFlag", "System.String", "UnityEngine.Hash128"},
                new List<string>(){ "UnityEngine.Caching", "SetNoBackupFlag", "System.String", "System.Int32"},
                new List<string>(){ "UnityEngine.Caching", "ResetNoBackupFlag", "UnityEngine.CachedAssetBundle"},
                new List<string>(){ "UnityEngine.Caching", "ResetNoBackupFlag", "System.String", "UnityEngine.Hash128"},
                new List<string>(){ "UnityEngine.DrivenRectTransformTracker", "StartRecordingUndo"},
                new List<string>(){ "UnityEngine.DrivenRectTransformTracker", "StopRecordingUndo"},
                new List<string>(){ "UnityEngine.UI.Graphic", "OnRebuildRequested"},
                new List<string>(){ "UnityEngine.Input", "IsJoystickPreconfigured", "System.String"},
                new List<string>(){ "UnityEngine.LightProbeGroup", "dering"},
                new List<string>(){ "UnityEngine.LightProbeGroup", "probePositions"},
                new List<string>(){ "UnityEngine.Light", "SetLightDirty"},
                new List<string>(){ "UnityEngine.Light", "shadowRadius"},
                new List<string>(){ "UnityEngine.Light", "shadowAngle"},
                new List<string>(){ "UnityEngine.ParticleSystemForceField", "FindAll"},
                new List<string>(){ "UnityEngine.QualitySettings", "streamingMipmapsRenderersPerFrame"},
                new List<string>(){ "UnityEngine.Texture", "imageContentsHash"},
            };
}
