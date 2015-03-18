using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using Autofac.Extras.DynamicProxy2;
using Castle.DynamicProxy;

namespace AOPLogging
{
    class Program
    {
        static void Main(string[] args)
        {
            var builder = new Autofac.ContainerBuilder();

            builder.RegisterType<FancyReport>()
                .As<FancyReport>()
                .EnableClassInterceptors()
                .InterceptedBy(typeof (ItsLog));

            builder.RegisterType<LoggedType2>()
                .As<LoggedType2>()
                .EnableClassInterceptors()
                .InterceptedBy(typeof(ItsLog));


            builder.RegisterType<ItsLog>();
            //typed registration
            builder.RegisterCallback(c => new ItsLog());

            var container = builder.Build();

            //use it
            var intercepted = container.Resolve<FancyReport>();
            intercepted.DoReport("first");
            intercepted.DoReport("second");

            var intercepted2 = container.Resolve<LoggedType2>();
            intercepted2.DoReport("first");
            intercepted2.DoReport("second");

        }

     
    }

    public interface IReport
    {
        void DoReport(string something);
    }

    public class FancyReport : IReport
    {
        public virtual void DoReport(string something)
        {
            Console.WriteLine("Report {0}", something);
        }

    }

    public class LoggedType2 : IReport
    {
        public virtual void DoReport(string something)
        {
            CalledSomething(something);
        }

        public void CalledSomething(string something)
        {
            Console.WriteLine("Do {0}", something);
        }

    }
    
}
