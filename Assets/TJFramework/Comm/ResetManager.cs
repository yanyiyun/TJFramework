using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using XLua;

namespace TJ
{
    public class ResetManager : Singleton<ResetManager>
    {
        public string resetSceneName;

        public bool CanReset()
        {
            if (!BundleManager.Instance.CanClear())
                return false;

            return true;
        }

        public void Reset()
        {
            //TODO: Object Pool, timer or something

            LuaManager.Instance.Clear();
            BundleManager.Instance.Clear();


            BundleManager.Instance.Reset();
            LuaManager.Instance.Reset();


            //
        }
    }
}
