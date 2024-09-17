using System;
using System.Collections.Generic;
using System.Reflection;
using Autofac;
using Autofac.Core;
using Module = Autofac.Module;

namespace AutofacSamples
{
    public class IResource
    {

    }

    class SingletonResource : IResource
    {
    }

    public class InstancePerDependencyResource: IResource, IDisposable
    {
        public InstancePerDependencyResource()
        {
            Console.WriteLine("Instance per dep created");
        }

        public void Dispose()
        {
            Console.WriteLine("Instance per dep disposed");
        }
    }

    public class ResourceManager
    {
        public ResourceManager(IEnumerable<IResource> resources)
        {
            Resources = resources ?? throw new ArgumentNullException(nameof(resources));
            Console.WriteLine("ResourceManager.ctor");
        }

        public IEnumerable<IResource> Resources { get; set; }
    }

    internal class Program
    {
        public static void Main(string[] args)
        {
            var builder = new ContainerBuilder();
            builder.RegisterType<ResourceManager>().SingleInstance();
            builder.RegisterType<SingletonResource>()
                .As<IResource>().SingleInstance();
            builder.RegisterType<InstancePerDependencyResource>()
                .As<IResource>();

            using (var container = builder.Build())
            {
                using (var scope = container.BeginLifetimeScope())
                {
                    var resourceManager = scope.Resolve<ResourceManager>();
                    Console.WriteLine("ResourceManager.Resources.Count: " + resourceManager.Resources.Count());
                }
            }
        }
    }
}