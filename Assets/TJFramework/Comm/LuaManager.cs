using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using XLua;

namespace TJ
{
    public class LuaManager : Singleton<LuaManager>
    {
        LuaEnv luaenv;
        List<string> searchPaths = new List<string>() { "" };

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


        public void DoString(string chunk)
        {
            luaenv.DoString(chunk);
        }

        public bool DoStringSafe(string chunk)
        {
            bool err = false;
            try
            {
                luaenv.DoString(chunk);
            }
            catch (Exception e)
            {
                err = true;
                Debug.LogError(e);
            }

            return err;
        }

        public void Clear()
        {
            if (luaenv != null)
            {
                //在没有清除所有Delegate时, 会抛出异常.
                luaenv.Dispose();

                luaenv = null;
            }
        }

        public void Reset()
        {
            Clear();

            luaenv = new LuaEnv();

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



        void Awake()
        {
            Reset();
        }

        void Update()
        {
            if (luaenv != null)
            {
                luaenv.Tick();
            }
        }

        byte[] ScriptLoader(ref string filepath)
        {
            filepath = filepath.Replace('.', '/') + ".lua.bytes";
            foreach (var sp in searchPaths)
            {
                var path = Path.Combine(Config.LuaScriptDirectory, Path.Combine(sp, filepath)).Replace('\\', '/');
                if (BundleManager.Instance.AssetExists(path))
                {
                    Asset asset = BundleManager.Instance.LoadAsset(path);
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
            filepath = filepath.Replace('.', '/') + ".lua";
            foreach (var sp in searchPaths)
            {
                var path = Path.Combine(Config.LuaScriptExternDirectory, Path.Combine(sp, filepath)).Replace('\\', '/');
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
