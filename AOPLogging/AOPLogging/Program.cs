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
    /// <summary>
    /// console application for showing use of AutoFac AOP's method intercept for logging 
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            var builder = new Autofac.ContainerBuilder();

            //register our fancy report, enable class interception and 
            // set the interception to happen with class ItsLog
            builder.RegisterType<FancyReport>()
                .As<FancyReport>()
                .EnableClassInterceptors()
                .InterceptedBy(typeof (ItsLog));

            builder.RegisterType<FancyReport2>()
                .As<FancyReport2>()
                .EnableClassInterceptors()
                .InterceptedBy(typeof(ItsLog));

            //register ItsLog with the constructor containing the info of our statsd server
            builder.Register(c => new ItsLog("127.0.0.1", 8125));

            var container = builder.Build();

            //use the wired up fancy report and witness the interception
            var intercepted = container.Resolve<FancyReport>();
            intercepted.DoReport("first");
            intercepted.DoReport("second");

            var intercepted2 = container.Resolve<FancyReport2>();
            intercepted2.DoReport("first");
            intercepted2.DoReport("second");
        }
    }
}
