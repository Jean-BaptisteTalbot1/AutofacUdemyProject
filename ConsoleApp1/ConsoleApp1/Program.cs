using Autofac;

namespace ConsoleApp1;

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
    private readonly string phoneNumber;

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
    private readonly IList<ILog> allLogs;

    public Reporting(IList<ILog> allLogs)
    {
        if (allLogs == null) throw new ArgumentNullException(nameof(allLogs));

        this.allLogs = allLogs;
    }

    public void Report()
    {
        foreach (var log in allLogs) log.Write($"Hello, this is {log.GetType().Name}");
    }
}

internal class Program
{
    public static void Main(string[] args)
    {
        var builder = new ContainerBuilder();

        builder.RegisterType<ConsoleLog>().As<ILog>();
        builder.Register(c => new SMSLog("123456789")).As<ILog>();
        builder.RegisterType<Reporting>();

        using (var container = builder.Build())
        {
            container.Resolve<Reporting>().Report();
        }
    }
}