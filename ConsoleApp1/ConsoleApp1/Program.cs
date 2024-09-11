using Autofac;
using Autofac.Features.Indexed;
using Autofac.Features.Metadata;

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

public class Settings
{
    public string LogMode { get; set; }
}

public class Reporting
{
    private IIndex<string, ILog> logs;

    public Reporting(IIndex<string, ILog> logs)
    {
        if (logs == null)
        {
            throw new ArgumentNullException(nameof(logs));
        }
        this.logs = logs;
    }

    public void Report()
    {
        logs["cmd"].Write("Starting the report output");
    }
}

internal class Program
{
    public static void Main(string[] args)
    {
        var builder = new ContainerBuilder();
        builder.RegisterType<ConsoleLog>().Keyed<ILog>("cmd");
        builder.Register((c, p) => new SMSLog("12334524")).Keyed<ILog>("sms");
        builder.RegisterType<Reporting>();

        using (var container = builder.Build())
        {
            container.Resolve<Reporting>().Report();
        }
    }
}