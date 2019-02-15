using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using XLua;

namespace TJ
{
    //TODO: 外部路径
    //LuaEnv的移除
    public class LuaManager : Singleton<LuaManager>
    {
        LuaEnv luaenv;
        List<string> searchPaths = new List<string>() { "" };

        //TODO: 需要修改
        public void AddSearchPath(string path)
        {
            searchPaths.Insert(0, path);
        }

        //TODO:执行语句
        public void Test()
        {
            AddSearchPath("Assets/TJFramework/Test/lua");

            //会扔出异常
            luaenv.DoString(@"require('test')");
        }

        void Awake()
        {
            luaenv = new LuaEnv();
            luaenv.AddLoader(FuncLoader);
        }

        void Update()
        {
            if (luaenv != null)
            {
                luaenv.Tick();
            }
        }

        byte[] FuncLoader(ref string filepath)
        {
            filepath = filepath.Replace('.', '/') + ".lua.bytes";
            foreach (var sp in searchPaths)
            {
                var path = Path.Combine(sp, filepath).Replace('\\', '/');
                if (BundleManager.Instance.AssetExists(path))
                {
                    Asset asset = BundleManager.Instance.LoadAsset(path);
                    TextAsset text = asset.RawAsset as TextAsset;
                    if (text != null)
                    {
                        filepath = path;
                        return text.bytes;
                    }
                }
            }
            return null;
        }


    }


}
