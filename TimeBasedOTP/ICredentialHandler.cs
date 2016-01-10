using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeBasedOTP
{
    public interface ICredentialHandler
    {
        string GetPassword(string userId);
        bool IsValid(string userId, string password, int interval);
        string GetPassword(string userId, DateTime date, int timeStep);
    }
}
