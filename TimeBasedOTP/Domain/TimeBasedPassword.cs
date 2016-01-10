using System;
using System.Runtime.Caching;

namespace TimeBasedOTP
{
    [Serializable]
    public sealed class TimeBasedPassword
    {
        #region Variables And Const

        public const int DEFAULT_VALIDITY_PERIODE_SECONDS = 30;

        public static DateTime UNIX_TIME = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        private readonly int _cacheLongevity = 2;
        private readonly string _secret = "@Th1s1$@V3ryL0ngH@$h@lg0r1thm$3CR3CTk3y";

        private readonly IHashAlgorithm _algorithm;
        private readonly ICacheService _storage;

        public int ValidityPeriodSeconds { get; private set;}

        internal TimeBasedPassword(ICacheService cache)
            :this(new HashSha512(), cache)
        {
            
        }

        internal TimeBasedPassword(IHashAlgorithm hmac, ICacheService cache, int validityPeriodSeconds = DEFAULT_VALIDITY_PERIODE_SECONDS)
        {
            this._algorithm = hmac;
            this._storage = cache ?? new CacheService();
            this.ValidityPeriodSeconds = validityPeriodSeconds > 0 ? validityPeriodSeconds : DEFAULT_VALIDITY_PERIODE_SECONDS;
        }

        internal TimeBasedPassword(IHashAlgorithm hmac, CacheService cache, string secrectKey, int validityPeriodSeconds = DEFAULT_VALIDITY_PERIODE_SECONDS)
            :this(hmac, cache)
        {
            this._secret = secrectKey;
        }
        #endregion

        internal string GetPassword(string userId)
        {
            return this.GetPassword(userId, GetCurrentCounter());
        }

        internal string GetPassword(string userId, DateTime epoch, int timeStep)
        {
            long counter = GetCurrentCounter(DateTime.UtcNow, epoch, timeStep);
            return this.GetPassword(userId, counter);
        }

        internal string GetPassword(string userId, DateTime now, DateTime epoch, int timeStep, int digits)
        {
            long counter = GetCurrentCounter(now, epoch, timeStep);
            return this.GetPassword(userId, counter, digits);
        }

        private string GetPassword(string userId, long counter, int digits = 6)
        {
            if (String.IsNullOrWhiteSpace(userId))
            {
                throw new TimeBasedOneTimePasswordException("UserId cannot be empty.");
            }

            byte[] key = System.Text.Encoding.ASCII.GetBytes(_secret);
            var otp = new OneTimePassword(key, _algorithm, digits);
            return otp.Generate(userId, counter);
        }

        public bool IsValid(string userId, string password, int intervals = 1)
        {
            if (String.IsNullOrWhiteSpace(userId))
            {
                throw new TimeBasedOneTimePasswordException("UserId cannot be empty.");
            }

            addToStorage(userId, password);

            if (password == GetPassword(userId))
                return true;

            for (int i = 1; i <= intervals; i++)
            {
                if (password == GetPassword(userId, GetCurrentCounter() + i))
                    return true;

                if (password == GetPassword(userId, GetCurrentCounter() - i))
                    return true;
            }

            return false;
        }

        private void addToStorage(string userId, string password)
        {
            // c#6 code : string cache_key = $"{userId}_{password}"; 
            string cache_key = string.Format("{0}_{1}", userId, password); 
            var key = _storage.Get<string>(cache_key);

            if (!string.IsNullOrWhiteSpace(key))
            {
                throw new TimeBasedOneTimePasswordException("You cannot use the same userId/password combination more than once.");
            }

            _storage.Set(cache_key, cache_key, TimeSpan.FromMinutes(_cacheLongevity));
        }

        private long GetCurrentCounter()
        {
            return GetCurrentCounter(DateTime.UtcNow, UNIX_TIME, this.ValidityPeriodSeconds);
        }

        private static long GetCurrentCounter(DateTime now, DateTime epoch, int timeStep)
        {
            if (timeStep == 0)
            {
                throw new TimeBasedOneTimePasswordException("timeStep cannot be zero");
            }
            return (long)(now - epoch).TotalSeconds / timeStep;
        }
    }
}