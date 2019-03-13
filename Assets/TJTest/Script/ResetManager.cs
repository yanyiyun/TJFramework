using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TJ
{
    //TODO: 这个可以改成热更新的实例代码, 而不属于框架部分. 毕竟这个定制性很强
    //这个不用写成manager
    //可以改成一个factory生成的普通类. 执行完成后, 也把自己移除了.
    public class ResetManager : Singleton<ResetManager>, IDisposable
    {
        public string resetSceneName;

        bool doing = false;

        public void Dispose()
        {
        }

        public bool CanReset()
        {
            if (!BundleManager.Instance.CanDispose())
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

            
            LuaManager.Instance.SafeCallFuncEngineReset();
            

            LuaManager.DoDispose();
            BundleManager.DoDispose();


            doing = false;

            ResetManager.DoDispose();   //TODO:不要这么写

            Debug.Log("Engine Reset Success!");
        }
    }
}
