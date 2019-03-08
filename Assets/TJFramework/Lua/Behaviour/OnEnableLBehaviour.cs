using System;
using UnityEngine;
using XLua;

namespace TJ
{
    public class OnEnableLBehaviour : LBehaviour
    {
        Action<LuaTable> cbOnDisable;
        Action<LuaTable> cbOnEnable;

        protected override void BindAction()
        {
            luaInst.Get("OnDisable", out cbOnDisable);
            luaInst.Get("OnEnable", out cbOnEnable);
        }

        protected override void Clear()
        {
            cbOnDisable = null;
            cbOnEnable = null;

            base.Clear();
        }


        void OnDisable()
        {
            if (cbOnDisable != null) cbOnDisable(luaInst);
        }

        void OnEnable()
        {
            if (cbOnEnable != null) cbOnEnable(luaInst);
        }
    }
}
