using Autofac;
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
    private Meta<ConsoleLog, Settings> log;

    public Reporting(Meta<ConsoleLog, Settings> log)
    {
        if (log == null)
        {
            throw new ArgumentNullException(nameof(log));
        }
        this.log = log;
    }

    public void Report()
    {
        log.Value.Write("Starting report");

        //if (log.Metadata["mode"] as string == "verbose")
        if (log.Metadata.LogMode == "verbose")
            log.Value.Write($"VERBOSE MODE : Logger started on {DateTime.Now}");
    }
}

internal class Program
{
    public static void Main(string[] args)
    {
        var builder = new ContainerBuilder();
        //builder.RegisterType<ConsoleLog>().WithMetadata("mode", "verbose");
        builder.RegisterType<ConsoleLog>()
            .WithMetadata<Settings>(c => c.For(x => x.LogMode, "verbose"));
        builder.RegisterType<Reporting>();

        using (var container = builder.Build())
        {
            container.Resolve<Reporting>().Report();
        }
    }
}