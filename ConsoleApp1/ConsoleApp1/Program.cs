using Autofac;

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

        public Engine(ILog log)
        {
            this.log = log;
            id = new Random().Next();
        }

        public void Ahead(int power)
        {
            log.Write($"Engine [{id}] ahead {power}");
        }
    }

    public class Car
    {
        private Engine engine;
        private ILog log;

        public Car(Engine engine)
        {
            this.engine = engine;
            this.log = new EmailLog();
        }

        public Car(Engine engine, ILog log)
        {
            this.engine = engine;
            this.log = log;
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

            // Whenever someone asks for a ILog, give them a ConsoleLog. So if Engine or Car asks for a ILog, it will get a ConsoleLog.
            builder.RegisterType<ConsoleLog>().As<ILog>();
            builder.RegisterType<Engine>();

            // Here we have two constructors for Car. The first one is the default one, and the second one is the one that takes an Engine and a ILog.
            // So if we ask for a Car, the container will use the second constructor to create it because it has the most parameters and by default
            // the UsingConstructor takes the most complex.
            builder.RegisterType<Car>()
                .UsingConstructor(typeof(Engine));
            // The UsingConstructor() method is used to specify which constructor to use when creating an instance of a type.
            // If we have multiple constructors, we can use the UsingConstructor() method to specify which one to use.
            // If we need to use a specific constructor, we can pass the types of the parameters of that constructor to the UsingConstructor() method
            // to specify which constructor to use by writing explicitly the types of the parameters of that constructor like this:
            // builder.RegisterType<Car>().UsingConstructor(typeof(Engine)); -> We take the public Car(Engine engine) signature.

            // Having registered all those types on the builder, the builder can now be used to construct the container.
            IContainer container = builder.Build();
            
            // So this container can be used to resolve types and instantiate them.
            // Then to get an instance of Car, we can ask the container to resolve it.  
            var car = container.Resolve<Car>();

            // Once we have registered the ConsoleLog as a ILog, the container will automatically inject it into the Engine and Car.
            // ALso, the consoleLog will not be accessible to the resolvers anymore because we have registered it as a ILog.
            // ***************
            //// That is to say, that something like that will not work: var log = container.Resolve<ConsoleLog>();
            //// But if we want to register a ConsoleLog as well, we can do it like this: builder.RegisterType<ConsoleLog>().AsSelf(); and then the Resole<ConsoleLog>() will work.
            // ***************

            car.Go();
        }
    }
}