using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TJ
{
    //TODO: 这个可以改成热更新的实例代码, 而不属于框架部分. 毕竟这个定制性很强
    public class ResetManager : Singleton<ResetManager>
    {
        public string resetSceneName;

        bool doing = false;

        public bool CanReset()
        {
            if (!BundleManager.Instance.CanClear())
                return false;

            return true;
        }

        public void Reset()
        {
            if (doing)
                return;

            doing = true;
            SceneManager.sceneUnloaded += OnSceneUnloaded;

            //第0个场景认为不被热更新影响的启动场景.
            SceneManager.LoadScene(0);
        }

        private void OnSceneUnloaded(Scene current)
        {
            SceneManager.sceneUnloaded -= OnSceneUnloaded;

            //TODO: Object Pool, timer or something

            //TODO: 需要增加event, 需要4个事件. event不能是lua的回调, 因为LuaManager理应被Reset
            //如果是自己的代码. 则无所谓

            if (LuaManager.Instance.funcEngineReset != null)
            {
                try
                {
                    LuaManager.Instance.funcEngineReset.Action();
                }
                catch(Exception e)
                {
                    Debug.LogError(e);
                }
            }

            LuaManager.Instance.Clear();
            BundleManager.Instance.Clear();

            BundleManager.Instance.Reset();
            LuaManager.Instance.Reset();


            doing = false;

            Debug.Log("Engine Reset Success!");
        }
    }
}
