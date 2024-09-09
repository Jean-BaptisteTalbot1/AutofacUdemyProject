using System.Reflection;
using Autofac;
using Autofac.Core;
using Autofac.Features.OwnedInstances;
using Module = Autofac.Module;

namespace ConsoleApp1
{
    public interface ILog : IDisposable
    {
        void Write(string message);
    }

    public class ConsoleLog : ILog
    {
        public ConsoleLog()
        {
            Console.WriteLine($"{nameof(ConsoleLog)} instance created at {DateTime.Now.Ticks}");
        }

        public void Write(string message)
        {
            Console.WriteLine(message);
        }

        public void Dispose()
        {
            Console.WriteLine("ConsoleLog instance disposed");
        }
    }

    public class SMSLog : ILog
    {
        private string phoneNumber;

        public SMSLog(string phoneNumber)
        {
            this.phoneNumber = phoneNumber;
        }

        public void Dispose()
        {
        }

        public void Write(string message)
        {
            Console.WriteLine($"SMS to {phoneNumber} : {message}");
        }
    }

    public class Reporting
    {
        private Func<ConsoleLog> consoleLog;

        public Reporting(Func<ConsoleLog> consoleLog)
        {
            if (consoleLog == null)
            {
                throw new ArgumentNullException(nameof(consoleLog));
            }

            this.consoleLog = consoleLog;
        }

        public void Report()
        {
            consoleLog().Write("Reporting to console");
            consoleLog().Write("And again");
        }

        internal class Program
        {
            public static void Main(string[] args)
            {
                var builder = new ContainerBuilder();

                builder.RegisterType<ConsoleLog>();
                builder.RegisterType<SMSLog>();
                builder.RegisterType<Reporting>();

                using (var container = builder.Build())
                {
                    container.Resolve<Reporting>().Report();
                }
            }
        }
    }
}