using System;
using UnityEngine;
using XLua;

namespace TJ
{
    public class LBehaviour : MonoBehaviour
    {
        protected LuaTable luaInst;

        Action<LuaTable> cbStart;
        Action<LuaTable> cbOnDestroy;


        public virtual void Bind(LuaTable inst)
        {
            luaInst = inst;

            luaInst.Get("Start", out cbStart);
            luaInst.Get("OnDestroy", out cbOnDestroy);
        }

        protected virtual void Clear()
        {
            cbStart = null;
            cbOnDestroy = null;

            luaInst = null;
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


    public class UpdateLBehaviour : LBehaviour
    {
        Action<LuaTable> cbUpdate;

        public override void Bind(LuaTable inst)
        {
            base.Bind(inst);

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


    public class FixedUpdateLBehaviour : LBehaviour
    {
        Action<LuaTable> cbFixedUpdate;

        public override void Bind(LuaTable inst)
        {
            base.Bind(inst);

            luaInst.Get("FixedUpdate", out cbFixedUpdate);
        }

        protected override void Clear()
        {
            cbFixedUpdate = null;

            base.Clear();
        }

        void FixedUpdate()
        {
            if (cbFixedUpdate != null) cbFixedUpdate(luaInst);
        }
    }


    public class LateUpdateLBehaviour : LBehaviour
    {
        Action<LuaTable> cbLateUpdate;

        public override void Bind(LuaTable inst)
        {
            base.Bind(inst);

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


    public class AllUpdateLBehaviour : LBehaviour
    {
        Action<LuaTable> cbUpdate;
        Action<LuaTable> cbFixedUpdate;
        Action<LuaTable> cbLateUpdate;

        public override void Bind(LuaTable inst)
        {
            base.Bind(inst);

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


    public class OnEnableLBehaviour : LBehaviour
    {
        Action<LuaTable> cbOnDisable;
        Action<LuaTable> cbOnEnable;

        public override void Bind(LuaTable inst)
        {
            base.Bind(inst);

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
