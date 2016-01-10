using System;
using System.Threading;
using Autofac;
using TimeBasedOTP;

namespace TimeBasedOTPConsole.Ioc
{
    public static class ObjectFactory
    {
        private static readonly Lazy<IContainer> _containerBuilder = new Lazy<IContainer>(
            defaultContainer, LazyThreadSafetyMode.ExecutionAndPublication);

        public static IContainer Container // C#6 code: => _containerBuilder.Value;
        {
            get { return _containerBuilder.Value; }
        }
        private static IContainer defaultContainer()
        {
            var builder = new ContainerBuilder();
            builder.RegisterType<HashSha512>().As<IHashAlgorithm>().InstancePerLifetimeScope();
            builder.RegisterType<CacheService>().As<ICacheService>().InstancePerLifetimeScope();
            builder.RegisterType<CredentialHandler>().As<ICredentialHandler>().InstancePerLifetimeScope();

            return builder.Build();
        }
    }
}
