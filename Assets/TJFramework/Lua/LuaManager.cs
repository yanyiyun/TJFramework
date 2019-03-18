using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using XLua;

namespace TJ
{
    public class LuaManager : Singleton<LuaManager>, IDisposable
    {
        //Lua初始化时的搜索目录
        public static string[] InitSearchPaths = {""};

        LuaEnv luaenv;
        List<string> searchPaths;

        public LuaEnv LuaEnv { get { return luaenv; } }

        //TODO: 搜索规则描述
        public List<string> SearchPaths
        {
            get
            {
                return searchPaths;
            }
            set
            {
                searchPaths = value;
            }
        }

        //后加入的会优先搜索
        public void AddSearchPath(string path)
        {
            searchPaths.Insert(0, path);
        }


        public object[] DoString(string chunk)
        {
            return luaenv.DoString(chunk);
        }

        public bool DoStringSafe(string chunk, out object[] rets)
        {
            rets = null;
            bool err = false;
            try
            {
                rets = luaenv.DoString(chunk);
            }
            catch (Exception e)
            {
                err = true;
                Debug.LogError(e);
            }

            return err;
        }


        //游戏被重置时调用
        public LuaFunction funcEngineBeforeDispose;
        public void SafeCallEngineBeforeDispose()
        {
            if (funcEngineBeforeDispose != null)
            {
                try
                {
                    funcEngineBeforeDispose.Action();
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                }
            }
        }


        void Awake()
        {
            luaenv = new LuaEnv();

            searchPaths = new List<string>(InitSearchPaths);

#if UNITY_EDITOR
            if (IsExternLuaMode)
            {
                luaenv.AddLoader(ExternScriptLoader);
            }
            else
#endif
            {
                luaenv.AddLoader(ScriptLoader);
            }
        }

        public void Dispose()
        {
            funcEngineBeforeDispose = null;

            //在没有清除所有Delegate时, 会抛出异常.
            luaenv.Dispose();
        }


        //public void Clear()
        //{
        //    if (luaenv == null)
        //        return;

        //    funcEngineReset = null;

        //    //在没有清除所有Delegate时, 会抛出异常.
        //    luaenv.Dispose();
        //    luaenv = null;

        //    searchPaths = null;
        //}


        void Update()
        {
            if (luaenv != null)
            {
                luaenv.Tick();
            }
        }

        byte[] ScriptLoader(ref string filepath)
        {
            filepath = filepath.Replace('.', '/') + ".lua.txt";
            foreach (var sp in searchPaths)
            {
                var path = Path.Combine(sp, filepath).Replace('\\', '/');
                //对于AssetBundle的判定方式, 取决于文件列表是否存在资源, 所以不会出现遍历顺序可能出现错误的问题.
                if (BundleManager.Instance.AssetExists(path))
                {
                    Asset asset = BundleManager.Instance.LoadAsset(path);
                    BundleManager.Instance.SetBundleHold(asset.Bundle, true);   //持有资源
                    TextAsset text = asset.RawAsset as TextAsset;
                    if (text != null)
                    {
                        //优化: 可以缓存bundle, 防止被移除
                        filepath = path;
                        return text.bytes;
                    }
                }
            }
            return null;
        }

#if UNITY_EDITOR
        static int externLuaMode = -1;
        public static bool IsExternLuaMode
        {
            get
            {
                if (externLuaMode == -1)
                    externLuaMode = UnityEditor.EditorPrefs.GetBool("ExternLuaMode", false) ? 1 : 0;
                return externLuaMode != 0;
            }
            set
            {
                int newValue = value ? 1 : 0;
                if (newValue != externLuaMode)
                {
                    externLuaMode = newValue;
                    UnityEditor.EditorPrefs.SetBool("ExternLuaMode", value);
                }
            }
        }

        byte[] ExternScriptLoader(ref string filepath)
        {
            filepath = filepath.Replace('.', '/') + ".lua.txt";
            foreach (var sp in searchPaths)
            {
                var path =  Path.Combine(sp, filepath).Replace('\\', '/');
                if (File.Exists(path))
                {
                    filepath = path;
                    return File.ReadAllBytes(path);
                }
            }
            return null;
        }
#endif

    }
}
