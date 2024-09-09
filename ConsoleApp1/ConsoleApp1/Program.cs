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
        private Func<string, SMSLog> smsLog;

        public Reporting(Func<ConsoleLog> consoleLog, Func<string, SMSLog> smsLog)
        {
            if (consoleLog == null)
            {
                throw new ArgumentNullException(nameof(consoleLog));
            }

            this.consoleLog = consoleLog;
            this.smsLog = smsLog;
        }

        public void Report()
        {
            consoleLog().Write("Reporting to console");
            consoleLog().Write("And again");

            smsLog("1234567890").Write("Texting admins...");
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