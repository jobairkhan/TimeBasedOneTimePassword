using System;
using System.Security.Cryptography;

namespace TimeBasedOTP
{
    public sealed class HashSha1 : IHashAlgorithm
    {
        public Type Algorithm
        {
            get { return typeof(HMACSHA1); }
        }

        public byte[] HashingAMessageWithASecretKey(byte[] iterationNumberByte, byte[] userIdByte)
        {
            using (var hmac = new HMACSHA1(userIdByte, true))
            {
                byte[] hash = hmac.ComputeHash(iterationNumberByte);
                return hash;
            }
        }

        public override string ToString()
        {
            return Algorithm.Name;
        }
    }
}
