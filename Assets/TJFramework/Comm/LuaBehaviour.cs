using System;
using UnityEngine;
using XLua;

namespace TJ
{
    public class LuaBehaviour : MonoBehaviour
    {
        LuaTable luaInst;

        Action<LuaTable> luaStart;
        Action<LuaTable> luaOnDestroy;
        Action<LuaTable> luaOnDisable;
        Action<LuaTable> luaOnEnable;


        public virtual void Bind(LuaTable inst)
        {
            luaInst = inst;


            luaInst.Get("Start", out luaStart);
            luaInst.Get("OnDestroy", out luaOnDestroy);
            luaInst.Get("OnDisable", out luaOnDisable);
            luaInst.Get("OnEnable", out luaOnEnable);
        }


        protected virtual void Start()
        {
            if (luaStart != null)
            {
                luaStart(luaInst);
            }
        }

        protected virtual void OnDestroy()
        {
            if (luaOnDestroy != null)
            {
                luaOnDestroy(luaInst);
            }


            luaInst = null;
            luaStart = null;
            luaOnDestroy = null;
        }

        protected virtual void OnDisable()
        {
            if (luaOnDisable != null)
            {
                luaOnDisable(luaInst);
            }
        }

        protected virtual void OnEnable()
        {
            if (luaOnEnable != null)
            {
                luaOnEnable(luaInst);
            }
        }

    }
}
