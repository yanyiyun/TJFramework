


local Scene2View = {}

function Scene2View.new()
    local inst = {}
    setmetatable(inst, {__index = Scene2View})
    return inst
end

function Scene2View:OnBind()
    print("OnBind, Scene2View")

    --垃圾回收演示
    collectgarbage("collect")
    CS.System.GC.Collect();
    CS.System.GC.WaitForPendingFinalizers()
    CS.TJ.BundleManager.Instance:UnloadUnusedBundles(false)

    self.btnScene = self.comp.transform:Find("btnScene")
    local btnComp = self.btnScene:GetComponent(typeof(CS.UnityEngine.UI.Button))
    btnComp.onClick:AddListener(function()
        print("change scene")
        local scenename = "Assets/TJFramework/Test/Scene/Scene1.unity";
        local bname = CS.TJ.BundleManager.Instance:AssetBundleName(scenename)
        CS.TJ.BundleManager.Instance:LoadBundle(bname);
        CS.UnityEngine.SceneManagement.SceneManager.LoadScene(scenename);

        -- CS.TJ.BundleManager.Instance:LoadBundle("assets/tjframework/test/scene1.unity.ab");
        -- CS.UnityEngine.SceneManagement.SceneManager.LoadScene("Assets/TJFramework/Test/Scene1.unity");
    end)

end

return Scene2View