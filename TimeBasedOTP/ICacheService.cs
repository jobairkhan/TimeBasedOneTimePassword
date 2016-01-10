using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeBasedOTP
{
    public interface ICacheService
    {
        T Get<T>(string key);
        void Set(string key, object value);
        void Set(string key, object data, int cacheTime);
        void Set(string key, object value, TimeSpan validFor);
        void Remove(string key);
        void Clear();
    }
}
