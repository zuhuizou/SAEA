﻿/****************************************************************************
*Copyright (c) 2018 Microsoft All Rights Reserved.
*CLR版本： 4.0.30319.42000
*机器名称：WENLI-PC
*公司名称：Microsoft
*命名空间：SAEA.Commom
*文件名： OuterMemoryCacheHelper
*版本号： V2.1.5.0
*唯一标识：13ed79e2-020d-45aa-a67f-ec00cf30da2d
*当前的用户域：WENLI-PC
*创建人： yswenli
*电子邮箱：wenguoli_520@qq.com
*创建时间：2018/10/9 14:31:54
*描述：
*
*=====================================================================
*修改标记
*创建时间：2018/10/9 14:31:54
*修改人： yswenli
*版本号： V2.1.5.0
*描述：
*
*****************************************************************************/
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SAEA.Common
{
    /// <summary>
    /// 自定义缓存
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class OuterMemoryCacheHelper<T> : IDisposable
    {
        ConcurrentDictionary<string, MemoryCachItem<T>> _dic;

        bool _disposed = false;

        object _synclocker = new object();

        /// <summary>
        /// 自定义缓存
        /// </summary>
        public OuterMemoryCacheHelper()
        {
            _dic = new ConcurrentDictionary<string, MemoryCachItem<T>>();
        }

        public void Set(string key, T value, TimeSpan timeOut)
        {
            var mc = new MemoryCachItem<T>() { Key = key, Value = value, Expired = DateTimeHelper.Now.AddSeconds(timeOut.TotalSeconds) };
            _dic.AddOrUpdate(key, mc, (k, v) => { return mc; });
        }

        public T Get(string key)
        {
            _dic.TryGetValue(key, out MemoryCachItem<T> mc);
            if (mc != null && mc.Value != null)
            {
                return mc.Value;
            }
            return default(T);
        }

        public void Active(string ID, TimeSpan timeOut)
        {
            lock (_synclocker)
            {
                var item = Get(ID);
                if (item != null)
                {
                    Set(ID, item, timeOut);
                }
            }
        }


        public void Del(string key)
        {
            _dic.TryRemove(key, out MemoryCachItem<T> mc);
        }

        public IEnumerable<MemoryCachItem<T>> List
        {
            get
            {
                return _dic.Values.ToList();
            }
        }

        public void Clear()
        {
            _dic.Clear();
        }

        public void Dispose()
        {
            _disposed = true;
            _dic.Clear();
            _dic = null;
        }
    }
    /// <summary>
    /// 缓存项
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class MemoryCachItem<T>
    {
        public string Key
        {
            get; set;
        }

        public T Value
        {
            get; set;
        }

        public DateTime Expired
        {
            get; set;
        }
    }
}
