using System;
using UnityEngine;
using XLua;

namespace TJ
{
    public class AllUpdateLBehaviour : LBehaviour
    {
        Action<LuaTable> cbUpdate;
        Action<LuaTable> cbFixedUpdate;
        Action<LuaTable> cbLateUpdate;

        protected override void BindAction()
        {
            luaInst.Get("Update", out cbUpdate);
            luaInst.Get("FixedUpdate", out cbFixedUpdate);
            luaInst.Get("LateUpdate", out cbLateUpdate);
        }

        protected override void Clear()
        {
            cbUpdate = null;
            cbFixedUpdate = null;
            cbLateUpdate = null;

            base.Clear();
        }

        void Update()
        {
            if (cbUpdate != null) cbUpdate(luaInst);
        }

        void FixedUpdate()
        {
            if (cbFixedUpdate != null) cbFixedUpdate(luaInst);
        }

        void LateUpdate()
        {
            if (cbLateUpdate != null) cbLateUpdate(luaInst);
        }
    }
}
