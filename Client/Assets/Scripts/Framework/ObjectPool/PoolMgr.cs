/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/01/14 18:14:29
** desc:  对象池管理;
*********************************************************************************/

using MEC;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = System.Object;

namespace Framework
{
    public class PoolMgr : MonoSingleton<PoolMgr>
    {
        /// <summary>
        /// C# Object Pool;
        /// </summary>
        private Dictionary<Type, Object> _pool = new Dictionary<Type, Object>();

        /// <summary>
        /// Unity Object Pool;
        /// </summary>
        private GameObjectPool _unityObjectPool = new GameObjectPool();

        public GameObject Root { get; private set; }
        public ManagerInitEventHandler PoolMgrInitHandler { get; set; }

        private void Awake()
        {
            Root = GameObject.Find("_resPoolRoot");
            if (Root == null)
            {
                Root = new GameObject("_resPoolRoot");
                DontDestroyOnLoad(Root);
            }
        }

        /// <summary>
        /// 初始化;
        /// </summary>
        public override void Init()
        {
            base.Init();
            CoroutineMgr.Instance.RunCoroutine(ClearPool());
        }

        /// <summary>
        /// 获取对象池目标组件;
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="args">初始化参数</param>
        /// <returns></returns>
        public T Get<T>(params Object[] args) where T : new()
        {
            ObjectPool<T> pool;
            Object temp;
            if (_pool.TryGetValue(typeof(T), out temp))
            {
                pool = temp as ObjectPool<T>;
            }
            else
            {
                pool = CreatePool<T>();
            }
            T t = pool.Get();
            IPool target = t as IPool;
            if (target != null)
                target.OnGet(args);
            return t;
        }

        /// <summary>
        /// 释放对象池组件;
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="type"></param>
        public void Release<T>(T type) where T : new()
        {
            IPool target = type as IPool;
            if (target != null)
                target.OnRelease();
            ObjectPool<T> pool;
            Object temp;
            if (_pool.TryGetValue(typeof(T), out temp))
            {
                pool = temp as ObjectPool<T>;
            }
            else
            {
                pool = CreatePool<T>();
            }
            pool.Release(type);
        }

        /// <summary>
        /// 创建对象池;
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        private ObjectPool<T> CreatePool<T>() where T : new()
        {
            Object temp;
            if (_pool.TryGetValue(typeof(T), out temp))
            {
                return temp as ObjectPool<T>;
            }
            else
            {
                ObjectPool<T> pool = new ObjectPool<T>(null, null);
                _pool[typeof(T)] = pool;
                return pool;
            }
        }

        /// <summary>
        /// 获取GameObject;
        /// </summary>
        /// <param name="type"></param>
        /// <param name="assetName"></param>
        /// <returns></returns>
        public GameObject Clone(GameObject go)
        {
            return _unityObjectPool.Clone(go);
        }

        /// <summary>
        /// 贮存GameObject;
        /// </summary>
        /// <param name="type"></param>
        /// <param name="assetName"></param>
        /// <param name="element"></param>
        public void Release(GameObject element)
        {
            _unityObjectPool.Release(element);
        }

        /// <summary>
        /// 销毁对象池;
        /// </summary>
        public IEnumerator<float> ClearPool()
        {
            _pool.Clear();
            IEnumerator<float> _goPoolItor = _unityObjectPool.ClearGameObjectPool();
            while (_goPoolItor.MoveNext())
            {
                yield return Timing.WaitForOneFrame;
            }
            Resources.UnloadUnusedAssets();
            System.GC.Collect();
            if (PoolMgrInitHandler != null)
                PoolMgrInitHandler();
        }
    }
}
