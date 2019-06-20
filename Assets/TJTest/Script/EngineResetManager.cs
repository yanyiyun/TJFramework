using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TJ;
using System.Collections;

public class EngineResetManager : Singleton<EngineResetManager>, IDisposable
{
    public static int resetSceneIndex = 0;

    public static bool doing = false;

    public void Dispose()
    {
    }


    public void Reset()
    {
        if (doing)
            return;

        StartCoroutine(Reseting());
    }

    IEnumerator Reseting()
    {
        doing = true;

        if (!BundleManager.Instance.CanDispose())
            yield return null;

        //SceneManager.sceneUnloaded += OnSceneUnloaded;    //这时候回调不可靠

        SceneManager.LoadScene(resetSceneIndex);
    }

    public static void TryDoReset()
    {
        if (!doing)
            return;

        //注销回调
        //SceneManager.sceneUnloaded -= OnSceneUnloaded;

        //TODO: Object Pool, timer or something


        //通知lua
        var lfunc = LuaManager.Instance.GetLuaFunction("EngineBeforeDispose");
        if (lfunc != null)
        {
            try
            {
                lfunc.Action();
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }


        //清理单件
        LuaManager.DoDispose();
        BundleManager.DoDispose();


        //
        doing = false;
        //自己最后清理
        EngineResetManager.DoDispose();


        Debug.Log("Engine Reset Success!");
    }
}

