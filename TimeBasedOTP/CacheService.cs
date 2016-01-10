using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;

namespace TimeBasedOTP
{
    public class CacheService : ICacheService
    {
        //C#6 code: protected ObjectCache Cache => MemoryCache.Default
        protected ObjectCache Cache
        {
            get { return MemoryCache.Default; }
        }

        public virtual T Get<T>(string key)
        {
            return (T)Cache[key];
        }

        public virtual void Set(string key, object data)
        {
            if (data == null)
                return;

            var policy = new CacheItemPolicy();
            Cache.Add(new CacheItem(key, data), policy);
        }

        public virtual void Set(string key, object data, int cacheTime)
        {
            Set(key, data, TimeSpan.FromMinutes(cacheTime));
        }

        public virtual void Set(string key, object data, TimeSpan validFor)
        {
            if (data == null)
                return;

            var policy = new CacheItemPolicy { AbsoluteExpiration = DateTime.UtcNow + validFor };
            Cache.Add(new CacheItem(key, data), policy);
        }
        public virtual void Remove(string key)
        {
            Cache.Remove(key);
        }
        public virtual void Clear()
        {
            foreach (var item in Cache)
                Remove(item.Key);
        }
    }
}
