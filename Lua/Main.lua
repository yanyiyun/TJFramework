
-- TODO: 注释
local function LBehaviourBind(gameObject, luaInst, LBehaviour)
    LBehaviour = LBehaviour or CS.TJ.LBehaviour;
    local comp = gameObject:AddComponent(typeof(LBehaviour));
    comp:Bind(luaInst);
    luaInst.comp = comp;
    if luaInst.OnBind then
        luaInst:OnBind()
    end
end


print("do once")

-- CS.TJ.BundleManager.Create()



return function()
    print("init scene")
    local go = CS.UnityEngine.GameObject.Find("Canvas")
    LBehaviourBind(go, require("Scene1")(), CS.TJ.LBehaviour)
end