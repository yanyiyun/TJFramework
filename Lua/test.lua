require("TJ.functions")
print("test Lua call extern")
print(CS)
print(CS.TJ.LBehaviour, type(CS.TJ.LBehaviour))
dump(CS.TJ.LBehaviour)
-- for k,v in pairs(CS.TJ.LuaBehaviour) do
--     print(k,v)
-- end
local inst = {
    OnBind = function(self)
        print('OnBind')
        print(self.comp)
    end,

    Start = function(self)
        print('Start')
    end,

    OnDestroy = function(self)
        print('OnDestroy')
    end,

    OnDisable = function(self)
        print('OnDisable')
    end,

    OnEnable = function(self)
        print('OnEnable')
    end,

    FixedUpdate = function(self)
        print('FixedUpdate')
    end,
}

print("====", inst)

local go = CS.UnityEngine.GameObject('mytest')
-- local comp = go:AddComponent(typeof(CS.TJ.LBehaviour));
-- comp:Bind(inst);
-- inst.comp = comp;
-- --TODO: 是类
-- print("++++++", type(comp))

local LuaBehaviour = class("LuaBehaviour")

function LuaBehaviour.Aaa()
end

-- TODO: 注释
local function LBehaviourBind(gameObject, inst, compType)
    compType = compType or CS.TJ.LBehaviour;
    local comp = gameObject:AddComponent(typeof(compType));
    comp:Bind(inst);
    inst.comp = comp;
    if inst.OnBind then
        inst:OnBind()
    end
end


LBehaviourBind(go, inst, CS.TJ.LBehaviour)