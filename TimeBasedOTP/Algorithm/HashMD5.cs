using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace TimeBasedOTP
{
    public sealed class HashMd5 : IHashAlgorithm
    {
        public Type Algorithm
        {
            get { return typeof(HMACMD5); }
        }

        public byte[] HashingAMessageWithASecretKey(byte[] iterationNumberByte, byte[] userIdByte)
        {
            using (var hmac = new HMACMD5(userIdByte))
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
