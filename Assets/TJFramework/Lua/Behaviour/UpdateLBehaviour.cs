using System;
using UnityEngine;
using XLua;

namespace TJ
{
    public class UpdateLBehaviour : LBehaviour
    {
        Action<LuaTable> cbUpdate;

        protected override void BindAction()
        {
            luaInst.Get("Update", out cbUpdate);
        }

        protected override void Clear()
        {
            cbUpdate = null;

            base.Clear();
        }

        void Update()
        {
            if (cbUpdate != null) cbUpdate(luaInst);
        }
    }
}
