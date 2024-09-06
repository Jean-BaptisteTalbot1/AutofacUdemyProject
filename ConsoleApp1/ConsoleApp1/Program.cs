using System.Reflection;
using Autofac;
using Autofac.Core;
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
        private readonly Lazy<ConsoleLog> log;

        public Reporting(Lazy<ConsoleLog> log)
        {
            if (log == null)
            {
                throw new ArgumentNullException(nameof(log));
            }
            this.log = log;
            Console.WriteLine($"{nameof(Reporting)} instance created at {DateTime.Now.Ticks}");
        }

        public void Report()
        {
            log.Value.Write("Log started");
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
                container.Resolve<Reporting>().Report();
            }
        }
    }
}