using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TimeBasedOTP;

namespace UnitTest.TimeBasedOTP
{
    [TestClass]
    public class TestTimeBasedOneTimePassword
    {
        ICacheService _storage = new FakeCacheService();

        [TestMethod]
        public void TestingCreateAnInstanceOfTimeBasedWrapper()
        {
            var objTotp = new TimeBasedPassword(_storage);
            Assert.IsNotNull(objTotp);
            Assert.AreEqual(TimeBasedPassword.DEFAULT_VALIDITY_PERIODE_SECONDS, objTotp.ValidityPeriodSeconds);
        }

        [TestMethod]
        public void TestingTimeBasedWrapperDefaultValues()
        {
            var generator = new TimeBasedPassword(_storage);
            var time = new DateTime(2016, 1, 1);
            string otp = generator.GetPassword("12345", time, TimeBasedPassword.DEFAULT_VALIDITY_PERIODE_SECONDS);

            Assert.IsNotNull(otp);
            Assert.AreEqual(6, otp.Length);
        }

        [TestMethod]
        public void TestingPasswordShouldVaryByValidityPeriod()
        {
            var generator = new TimeBasedPassword(_storage);
            var time = new DateTime(2016, 1, 1);
            string password1 = generator.GetPassword("12345", time, 1);
            string password2 = generator.GetPassword("12345", time, 2);
            string password3 = generator.GetPassword("12345", time, 1);

            Assert.AreNotEqual(password1, password2);
            Assert.AreEqual(password1, password3);
        }

        [TestMethod]
        public void TestingPasswordVariesByUser()
        {
            var generator = new TimeBasedPassword(_storage);
            var time = new DateTime(2016, 1, 1);
            string password1 = generator.GetPassword("user1", time, 1);
            string password2 = generator.GetPassword("user2", time, 1);
            string password3 = generator.GetPassword("user1", time, 1);

            Assert.AreNotEqual(password1, password2);
            Assert.AreEqual(password1, password3);
        }

        [TestMethod]
        public void TestingPasswordNotVariedByTimeOnly()
        {
            var generator = new TimeBasedPassword(_storage);
            string password1 = generator.GetPassword("user1");

            var time = DateTime.UtcNow;
            string password2TimeDefined = generator.GetPassword("user1", time, 30);

            System.Threading.Thread.Sleep(2000);
            string password3 = generator.GetPassword("user1");
            string password4TimeDefined = generator.GetPassword("user1", time, 2);

            Assert.AreNotEqual(password1, password2TimeDefined);
            Assert.AreEqual(password1, password3);
            Assert.AreNotEqual(password2TimeDefined, password4TimeDefined);
        }
        
        [TestMethod]
        [ExpectedException(typeof(TimeBasedOneTimePasswordException))]
        public void TestingInvalidTimeStep()
        {
            var generator = new TimeBasedPassword(_storage);
            string password1 = generator.GetPassword("user1", DateTime.UtcNow, 0);
        }

        [TestMethod]
        [ExpectedException(typeof(TimeBasedOneTimePasswordException))]
        public void TestingSecondTimeValidation()
        {
            var robo = new TimeBasedPassword(null, _storage, 10);
            string user = "user1";
            string password1 = robo.GetPassword(user);
            Assert.IsTrue(robo.IsValid(user, password1));
            bool shouldThrowException = robo.IsValid(user, password1);
        }

        [TestMethod]
        public void TestingVerifyValidityPeriods()
        {
            var robo = new TimeBasedPassword(null, _storage, 3);
            string user = "user1";
            string password1 = robo.GetPassword(user);
            Assert.IsTrue(robo.IsValid(user, password1));
            System.Threading.Thread.Sleep(3100);
            string password2 = robo.GetPassword(user);
            System.Threading.Thread.Sleep(5000);
            Assert.IsFalse(robo.IsValid(user, password2, 1));
        }


        [TestMethod]
        public void TestingPasswordCheckAdjacentIntervals()
        {
            var robo = new TimeBasedPassword(null, _storage, 3);
            string user = "user2";

            string password2 = robo.GetPassword(user);
            System.Threading.Thread.Sleep(2100);
            Assert.IsTrue(robo.IsValid(user, password2, 3));
        }
    }
}
