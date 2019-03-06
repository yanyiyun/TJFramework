
local util = require("xlua.util")

return function ()
    local inst = {
        num = 0,

        OnBind = function(self)
            self.btnAddOne = self.comp.transform:Find("btnAddOne")
            local btnComp = self.btnAddOne:GetComponent(typeof(CS.UnityEngine.UI.Button))
            btnComp.onClick:AddListener(function()
                self:AddOne()
            end)

            self.btnChangeScene = self.comp.transform:Find("btnChangeScene")
            local btnComp = self.btnChangeScene:GetComponent(typeof(CS.UnityEngine.UI.Button))
            btnComp.onClick:AddListener(function()
                print("change scene")
                local scenename = "Assets/TJFramework/Test/Scene/Scene2.unity";
                local bname = CS.TJ.BundleManager.Instance:AssetBundleName(scenename)
                CS.TJ.BundleManager.Instance:LoadBundle(bname);
                CS.UnityEngine.SceneManagement.SceneManager.LoadScene(scenename);
            end)

            self.btnResetGame = self.comp.transform:Find("btnResetGame")
            local btnComp = self.btnResetGame:GetComponent(typeof(CS.UnityEngine.UI.Button))
            btnComp.onClick:AddListener(function()
                print("reset game")
                CS.TJ.ResetManager.Instance:Reset()
            end)
        end,

        Start = function(self)
            --协程演示
            self.comp:StartCoroutine(util.cs_generator(function()
                local req = CS.TJ.BundleManager.Instance:LoadAssetAsync("assets/tjframework/test/cylinder.prefab")
                coroutine.yield(req)
                local asset = req.Asset
                asset:Instantiate()
            end))

        end,

        OnDestroy = function(self)
        end,

        AddOne = function(self)
            self.num = self.num + 1
            self.comp.transform:Find("Text"):GetComponent(typeof(CS.UnityEngine.UI.Text)).text = self.num
        end,
    }

    return inst
end
