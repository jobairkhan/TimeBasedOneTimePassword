using System;
using System.Security.Cryptography;

namespace TimeBasedOTP
{
    public sealed class HashSha512 : IHashAlgorithm
    {
        public Type Algorithm
        {
            get { return typeof(HMACSHA512); }
        }

        public byte[] HashingAMessageWithASecretKey(byte[] iterationNumberByte, byte[] userIdByte)
        {
            using (var hmac = new HMACSHA512(userIdByte))
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
