using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TimeBasedOTP;

namespace UnitTest.TimeBasedOTP
{
    internal class FakeCacheService : ICacheService
    {
        private Dictionary<string, DateTime> _data = new Dictionary<string, DateTime>();
        

        public virtual T Get<T>(string key)
        {
            if (_data.ContainsKey(key))
            {
                var val = _data[key];
                if (val < DateTime.UtcNow)
                {
                    Remove(key);
                    return default(T);
                }
                return (T)(val.ToString() as object);
            }
            return default(T);
        }

        public virtual void Set(string key, object data)
        {
            if (data == null)
                return;
            addKeyToDictionary(key, data);
        }

        private string addKeyToDictionary(string key, object data)
        {
            _data.Add(key, DateTime.UtcNow);
            return key;
        }

        public virtual void Set(string key, object data, int cacheTime)
        {
            Set(key, data, TimeSpan.FromMinutes(cacheTime));
        }

        public virtual void Set(string key, object data, TimeSpan validFor)
        {
            if (data == null)
                return;

            var insertedKey = addKeyToDictionary(key, data);
            Task.Factory.StartNew(() =>
            {
                Thread.Sleep(validFor.Seconds * 1000);
                if (_data.ContainsKey(key))
                {
                    var absoluteExpiration = DateTime.UtcNow + validFor;
                    _data[key] = absoluteExpiration;
                }
            });
        }

        public virtual void Remove(string key)
        {
            if (_data.ContainsKey(key))
            {
                _data.Remove(key);
            }
        }
        public virtual void Clear()
        {
            foreach (var item in _data)
                Remove(item.Key);
        }
    }
}
