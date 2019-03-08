using System;
using UnityEngine;
using XLua;

namespace TJ
{
    public class LBehaviour : MonoBehaviour
    {
        public string moduleName = "";

        protected LuaTable luaInst;

        Action<LuaTable> cbStart;
        Action<LuaTable> cbOnDestroy;


        public void Bind(LuaTable inst)
        {
            luaInst = inst;

            luaInst.Get("Start", out cbStart);
            luaInst.Get("OnDestroy", out cbOnDestroy);
            BindAction();

            luaInst.Set("comp", this);
            Action<LuaTable> cbOnBind;
            luaInst.Get("OnBind", out cbOnBind);
            if (cbOnBind != null) cbOnBind(luaInst);
        }

        protected virtual void BindAction()
        {
        }

        protected virtual void Clear()
        {
            cbStart = null;
            cbOnDestroy = null;

            luaInst.Set<string, object>("comp", null);

            luaInst = null;
        }

        void Awake()
        {
            if (moduleName.Trim().Length != 0)
            {
                object[] rets = LuaManager.Instance.DoString(string.Format("return require('{0}')", moduleName.Trim()));
                if (rets.Length >= 1)
                {
                    LuaTable tluaInst = null;
                    if (rets[0] is LuaTable)
                    {
                        LuaTable cls = rets[0] as LuaTable;
                        Func<LuaTable> funcNew;
                        cls.Get("new", out funcNew);
                        if (funcNew != null)
                        {
                            tluaInst = funcNew();
                            funcNew = null;
                        }
                        else
                        {
                            tluaInst = cls;
                        }
                    }
                    else if (rets[0] is LuaFunction)
                    {
                        LuaFunction func = rets[0] as LuaFunction;
                        tluaInst = func.Func<LuaTable>();
                    }

                    if (tluaInst != null)
                    {
                        Bind(tluaInst);
                    }
                }
            }
        }

        void Start()
        {
            if (cbStart != null) cbStart(luaInst);
        }

        void OnDestroy()
        {
            if (cbOnDestroy != null) cbOnDestroy(luaInst);
            Clear();
        }
    }
}
