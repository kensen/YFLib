using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Extensions.Caching.Memory;

namespace YF.Utility.Extensions
{
    /// <summary>
    /// 内存缓存扩展操作类
    /// </summary>
    public static class MemoryCacheExtensions
    {
        /// <summary>
        /// 获取指定键值的强类型数据
        /// </summary>
        /// <typeparam name="T">强类型</typeparam>
        /// <param name="cache"></param>
        /// <param name="key">缓存键值</param>
        /// <returns></returns>
        public static T Get<T>(this MemoryCache cache, string key)
        {
            //object value = cache.Get(key, regionName);
            //if (value is T)
            //{
            //    return (T)value;
            //}
            //return default(T);

            T val ;
            if (key != null && cache.TryGetValue(key, out val))
            {

                return val;
            }
            else
            {
                return default(T);
            }

        }
    }
}
