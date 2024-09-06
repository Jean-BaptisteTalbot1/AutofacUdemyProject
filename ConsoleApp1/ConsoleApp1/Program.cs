using Autofac;
using Autofac.Core;

namespace ConsoleApp1
{
    public interface ILog
    {
        void Write(string message);
    }

    public class ConsoleLog : ILog
    {
        public void Write(string message)
        {
            Console.WriteLine(message);
        }
    }


    public class EmailLog : ILog
    {
        private const string adminEmail = "admin@foo.com";
        public void Write(string message)
        {
            Console.WriteLine($"Email sent to {adminEmail}: {message}");
        }
    }

    public class Engine
    {
        private ILog log;
        private int id;

        public Engine(ILog log, int id)
        {
            this.log = log;
            this.id = id;
        }

        public void Ahead(int power)
        {
            log.Write($"Engine [{id}] ahead {power}");
        }
    }

    public class SMSLog : ILog
    {
        private string phoneNumber;

        public SMSLog(string phoneNumber)
        {
            this.phoneNumber = phoneNumber;
        }
        public void Write(string message)
        {
            Console.WriteLine($"SMS to {phoneNumber} : {message}");
        }
    }

    public class Car
    {
        private Engine engine;
        private ILog log;

        public Car(Engine engine, ILog log)
        {
            this.engine = engine;
            this.log = log;
        }

        public Car(Engine engine)
        {
            this.engine = engine;
            this.log = new EmailLog();
        }

        public void Go()
        {
            engine.Ahead(100);
            log.Write("Car going forward...");
        }
    }

    internal class Program
    {
        public static void Main(string[] args)
        {
            var builder = new ContainerBuilder();

            // Named parameter
            //builder.RegisterType<SMSLog>().As<ILog>()
            //    .WithParameter("phoneNumber", "567-123-1234");
            
            // Typed parameter
            //builder.RegisterType<SMSLog>().As<ILog>()
            //    .WithParameter(new TypedParameter(typeof(string), "678-345-1234"));

            // Resolved parameter
            //builder.RegisterType<SMSLog>().As<ILog>()
            //    .WithParameter(
            //        new ResolvedParameter(
            //            // predicate (returns true if the parameter should be supplied)
            //            (pi, ctx) => pi.ParameterType == typeof(string) && pi.Name == "phoneNumber",
            //            // value accessor
            //            (pi, ctx) => "567-123-1234"
            //            )
            //        );

            Random random = new Random();
            builder.Register((c, p) => new SMSLog(p.Named<string>("phoneNumber")))
                .As<ILog>();

            Console.WriteLine("About to build container");
            var container = builder.Build();

            var log = container.Resolve<ILog>(new NamedParameter("phoneNumber", random.Next().ToString()));
            log.Write("After the build");
        }
    }
}