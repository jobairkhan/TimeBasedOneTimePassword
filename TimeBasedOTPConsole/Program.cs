using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeBasedOTP;
using Autofac;

namespace TimeBasedOTPConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            using (Autofac.ILifetimeScope scope = Ioc.ObjectFactory.Container.BeginLifetimeScope())
            {
                var credential = scope.Resolve<ICredentialHandler>();
                consoleMessage();
                var key = Console.ReadKey();
                while (key.Key != ConsoleKey.Escape)
                {
                    switch (key.KeyChar)
                    {
                        case 'u':
                        case 'U':
                            generatePassword(credential);
                            goto default;
                        case 'v':
                        case 'V':
                            validatePassword(credential);
                            break;
                        default:
                            consoleMessage();
                            break;
                    }
                    key = Console.ReadKey();
                }
            }
        }

        private static void validatePassword(ICredentialHandler credential)
        {
            Console.Clear();
            Console.Write("User Id: ");
            var user = Console.ReadLine();
            Console.Write("Password: ");
            var pass = Console.ReadLine();

            Console.WriteLine("--------------------------------------------------");
            try
            {
                if (credential.IsValid(user, pass, 30))
                {
                    Console.WriteLine("User Authenticated.");
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Magenta;
                    Console.WriteLine("Invalid user id or password.");
                }
            }
            catch (TimeBasedOTP.TimeBasedOneTimePasswordException ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ex.Message);
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Error: {0}", ex.Message);
            }
            Console.ForegroundColor = ConsoleColor.White;
            Console.ReadLine();
            consoleMessage();
        }

        private static void generatePassword(ICredentialHandler credential)
        {
            Console.Clear();
            Console.Write("User Id: ");
            var user = Console.ReadLine();
            try
            {
                var pass = credential.GetPassword(user);
                Console.WriteLine("Password: {0}", pass);
                Console.WriteLine("--------------------------------------------------");
                Console.WriteLine("Note: Password will expire after {0} seconds",
                    TimeBasedOTP.TimeBasedPassword.DEFAULT_VALIDITY_PERIODE_SECONDS);
            }
            catch (TimeBasedOTP.TimeBasedOneTimePasswordException ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Error: {0}", ex.Message);
                Console.ForegroundColor = ConsoleColor.White;
            }
            catch
            {
                //TODO
            }
            Console.ReadLine();
        }

        private static void consoleMessage()
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.Clear();
            Console.WriteLine("**************************************");
            Console.WriteLine("Press Esc to close");
            Console.WriteLine("Enter u to Enter user name and get password");
            Console.WriteLine("Enter v to Validate user name and password");
            Console.WriteLine("**************************************");
        }
    }
}
