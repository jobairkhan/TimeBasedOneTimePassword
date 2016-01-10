using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text;
using TimeBasedOTP;

namespace UnitTest.TimeBasedOTP
{
    [TestClass]
    public class TestOneTimePassword
    {
        [TestMethod]
        public void TestingOTPConstructorPassAllValues()
        {
            string secret = "SomeKey";
            byte[] key = Encoding.UTF8.GetBytes(secret);
            int returnDigits = 6;

            IHashAlgorithm hmacSha1 = new HashSha512();
            OneTimePassword generator = new OneTimePassword(key, hmacSha1, returnDigits);

            Assert.IsNotNull(generator as OneTimePassword);
            Assert.AreEqual(returnDigits, generator.digits);
            Assert.AreEqual("HMACSHA512", generator.digest.ToString());
        }

        [TestMethod]
        public void TestingOTPConstructorDefaultValues()
        {
            string secret = "SomeKey";
            byte[] key = Encoding.UTF8.GetBytes(secret);

            OneTimePassword generator = new OneTimePassword(key, null);

            Assert.IsNotNull(generator);
            Assert.AreEqual(OneTimePassword.MAXIMUM_DIGITS, generator.digits);
            Assert.AreEqual("HMACSHA1", generator.digest.ToString());
        }

        [TestMethod]
        public void TestingSha1()
        {
            string secret = "SomeKey";
            byte[] key = Encoding.UTF8.GetBytes(secret);
            long interval = 66778;
            int returnDigits = 6;

            var hmacSha = new HashSha1();
            OneTimePassword generator = new OneTimePassword(key, hmacSha, returnDigits);
            String otp = generator.Generate("12345", interval);

            Assert.IsNotNull(otp);
            Assert.AreEqual(6, otp.Length);
            Assert.AreEqual("337162", otp);
        }

        [TestMethod]
        public void TestingSha512()
        {
            string secret = "SomeKey";
            byte[] key = Encoding.UTF8.GetBytes(secret);
            long interval = 66778;
            int returnDigits = 6;

            var hmac = new HashSha512();
            OneTimePassword generator = new OneTimePassword(key, hmac, returnDigits);
            String otp = generator.Generate("12345", interval);

            Assert.IsNotNull(otp);
            Assert.AreEqual(6, otp.Length);
            Assert.AreEqual("155083", otp);
        }


        [TestMethod]
        public void TestingMD5()
        {
            string secret = "SomeKey";
            byte[] key = Encoding.UTF8.GetBytes(secret);

            var hmac = new HashMd5();
            OneTimePassword generator = new OneTimePassword(key, hmac);
            String otp = generator.Generate("12345", 0);

            Assert.IsNotNull(otp);
            Assert.AreEqual(8, otp.Length);
            Assert.AreEqual("29321840", otp);
        }


        [TestMethod]
        public void TestingUserId()
        {
            string secret = "SomeKey";
            byte[] key = Encoding.UTF8.GetBytes(secret);
            long interval = 1;

            var hmacSha1 = new HashSha512();
            OneTimePassword generator = new OneTimePassword(key, hmacSha1);
            String otp = generator.Generate("user", interval);

            Assert.AreEqual(OneTimePassword.MAXIMUM_DIGITS, otp.Length);
            Assert.AreEqual("23297176", otp);
        }

        [TestMethod]
        public void TestingUserIdWithoutInterval()
        {
            string secret = "SomeKey";
            byte[] key = Encoding.UTF8.GetBytes(secret);

            var hmacSha1 = new HashSha1();
            OneTimePassword generator = new OneTimePassword(key, hmacSha1);
            String otp = generator.Generate("user", 0);

            Assert.AreEqual(OneTimePassword.MAXIMUM_DIGITS, otp.Length);
            Assert.AreEqual("31249860", otp);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TestingDigitsValidation()
        {
            var otp = new OneTimePassword(new byte[2], new HashMd5(), 100);
        }

    }    
}
