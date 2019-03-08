using System;
using UnityEngine;
using XLua;

namespace TJ
{
    public class LateUpdateLBehaviour : LBehaviour
    {
        Action<LuaTable> cbLateUpdate;

        protected override void BindAction()
        {
            luaInst.Get("LateUpdate", out cbLateUpdate);
        }

        protected override void Clear()
        {
            cbLateUpdate = null;

            base.Clear();
        }

        void LateUpdate()
        {
            if (cbLateUpdate != null) cbLateUpdate(luaInst);
        }
    }
}