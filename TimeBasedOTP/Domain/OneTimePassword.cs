using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace TimeBasedOTP
{
    [Serializable]
    internal sealed class OneTimePassword
    {
        public const int MAXIMUM_DIGITS = 8;

        private byte[] secret;
        internal IHashAlgorithm digest { get; private set; }
        internal int digits {get; private set;}

        private readonly int[] _iterationPowers = new int[] { 1, 10, 100, 1000, 10000, 100000, 1000000, 10000000, 100000000 };


        internal OneTimePassword(byte[] key, IHashAlgorithm hashAlgorithm, int? returnDigits = null)
        {
            if (returnDigits <= 0 || returnDigits > _iterationPowers.Length - 1)
            {
                StringBuilder msg = new StringBuilder();
                msg.Append("The number of digits should be between 1 and ");
                msg.Append(_iterationPowers.Length);
                throw new ArgumentOutOfRangeException("digits", msg.ToString());
            }

            this.secret = key;
            this.digits = returnDigits ?? MAXIMUM_DIGITS;
            this.digest = hashAlgorithm ?? new HashSha1();
        }

        internal string Generate(string userId, long interval)
        {
            long iterationNumber = interval > 0 ? interval : _iterationPowers[digits];

            byte[] iterationNumberByte = BitConverter.GetBytes(iterationNumber);

            //change To BigEndian - Based on Machine configuration
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(iterationNumberByte);
            }

            byte[] userIdByte = Encoding.ASCII.GetBytes(userId);
            byte[] hash = this.digest.HashingAMessageWithASecretKey(iterationNumberByte, userIdByte);

            //RFC4226 http://tools.ietf.org/html/rfc4226#section-5.4
            int offset = hash[hash.Length - 1] & 0xf; //0xf = 15d
            int binary = ((hash[offset] & 0x7f) << 24) //0x7f = 127d
                | ((hash[offset + 1] & 0xff) << 16) //0xff = 255d
                | ((hash[offset + 2] & 0xff) << 8)
                | (hash[offset + 3] & 0xff);

            int password = shrinkLength(binary); 

            //return password with zero-padding for the number of desired digits
            return password.ToString(new string('0', this.digits));
        }

        private int shrinkLength(int binary)
        {
            return binary % (int)Math.Pow(10, digits);
        }
    }
}
