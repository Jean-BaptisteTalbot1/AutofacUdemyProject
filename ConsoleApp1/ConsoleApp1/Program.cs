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
        private Owned<ConsoleLog> log;

        public Reporting(Owned<ConsoleLog> log)
        {
            if (log == null)
            {
                throw new ArgumentNullException(nameof(log));
            }
            this.log = log;
            Console.WriteLine("Reporting initialized");

        }

        public void ReportOnce()
        {
            log.Value.Write("Log started");
            log.Dispose();
        }
    }

    internal class Program
    {
        public static void Main(string[] args)
        {
            var builder = new ContainerBuilder();

            builder.RegisterType<ConsoleLog>();
            builder.RegisterType<Reporting>();

            using (var container = builder.Build())
            {
                container.Resolve<Reporting>().ReportOnce();
                Console.WriteLine("Done reporting");
            }
        }
    }
}