﻿/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/01/10 23:22:57
** desc:  组件抽象基类
*********************************************************************************/

using MEC;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    /// <summary>
    /// 组件抽象基类;
    /// </summary>
    public abstract class AbsComponent : IPool
    {
        private long _id;
        private bool _enable = true;
        private bool _isLoaded = false;
        private AbsEntity _entity;
        private GameObject _componentGo = null;
        private Action<AbsComponent> _initCallBack;

        public long ID { get { return _id; } }
        public bool Enable { get { return _enable; } set { _enable = value; } }
        public bool IsLoaded { get { return _isLoaded; } set { _isLoaded = value; } }
        public AbsEntity Entity { get { return _entity; } set { _entity = value; } }
        public GameObject ComponentGo { get { return _componentGo; } set { _componentGo = value; } }
        public Action<AbsComponent> InitCallBack { get { return _initCallBack; } set { _initCallBack = value; } }

        public virtual void AwakeEx() { }
        public virtual void UpdateEx() { }
        public virtual void LateUpdateEx() { }
        public virtual void OnDestroyEx() { }
        /// <summary>
        /// 初始化Component;
        /// </summary>
        /// <param name="entity">entity</param>
        /// <param name="go">gameObject</param>
        public virtual void OnInitComponent(AbsEntity entity, GameObject go)
        {
            _id = IdGenerater.GenerateId();
            OnAttachEntity(entity);
            OnAttachComponentGo(go);
            if (InitCallBack != null)
            {
                InitCallBack(this);
            }
        }
        /// <summary>
        /// 充值Component;
        /// </summary>
        public virtual void OnResetComponent()
        {
            DeAttachEntity();
            DeAttachComponentGo();
            _id = 0;
            _entity = null;
            _enable = true;
            _componentGo = null;
            _initCallBack = null;
        }
        /// <summary>
        /// Component附加Entity;
        /// </summary>
        /// <param name="entity"></param>
        public virtual void OnAttachEntity(AbsEntity entity)
        {
            Entity = entity;
        }
        /// <summary>
        /// Component附加GameObject;
        /// </summary>
        /// <param name="go"></param>
        public virtual void OnAttachComponentGo(GameObject go)
        {
            ComponentGo = go;
        }
        /// <summary>
        /// 重置Entity的附加;
        /// </summary>
        public abstract void DeAttachEntity();
        /// <summary>
        /// 重置GameObject的附加;
        /// </summary>
        public abstract void DeAttachComponentGo();
        /// <summary>
        /// 对象池Get;
        /// </summary>
        /// <param name="args"></param>
        public virtual void OnGet(params System.Object[] args) { }
        /// <summary>
        /// 对象池Release;
        /// </summary>
        public virtual void OnRelease() { }
    }
}
