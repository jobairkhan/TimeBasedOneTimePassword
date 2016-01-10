using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeBasedOTP
{
    public class CredentialHandler : ICredentialHandler
    {
        private readonly TimeBasedPassword timeBasedOneTimePasswordGenerator;

        public CredentialHandler(IHashAlgorithm algorithm, ICacheService cacheService)
        {
            timeBasedOneTimePasswordGenerator = new TimeBasedPassword(algorithm, cacheService);
        }

        public string GetPassword(string userId)
        {
            return timeBasedOneTimePasswordGenerator.GetPassword(userId);
        }

        public string GetPassword(string secret, DateTime epoch, int timeStep)
        {
            return timeBasedOneTimePasswordGenerator.GetPassword(secret, epoch, timeStep);
        }

        public string GetPassword(string secret, DateTime now, DateTime epoch, int timeStep, int digits)
        {
            return timeBasedOneTimePasswordGenerator.GetPassword(secret, now, epoch, timeStep, digits);
        }
        
        public bool IsValid(string secret, string password, int checkAdjacentIntervals = 1)
        {
            return timeBasedOneTimePasswordGenerator.IsValid(secret, password, checkAdjacentIntervals);
        }
    }
}
