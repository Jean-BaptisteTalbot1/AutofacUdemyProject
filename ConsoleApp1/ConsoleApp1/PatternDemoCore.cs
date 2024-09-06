using Autofac;

namespace ConsoleApp1
{
    public class Entity 
    { 
        public delegate Entity Factory();

        private static Random random = new Random();
        private int number;
        public Entity() 
        {
            number = random.Next();
        }

        public override string ToString()
        {
            return "testtt : " + number;
        }
    }

    public class ViewModel
    {
        private readonly Entity.Factory entityFactory;
        public ViewModel(Entity.Factory entityFactory)
        {
            this.entityFactory = entityFactory;
        }

        public void Method()
        {
            var entity = entityFactory();
            Console.WriteLine(entity);
        }
    }

    public class PatternDemoCore
    {
        public static void Mainn(string[] args)
        {
            // Setup
            var cb = new ContainerBuilder();
            cb.RegisterType<Entity>().InstancePerDependency();
            cb.RegisterType<ViewModel>();
            var container = cb.Build();

            var vm = container.Resolve<ViewModel>();
            vm.Method();
            vm.Method();
        }
    }
}
