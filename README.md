Using Autofac's Aspect Oriented Programming for instrumentation in a class factory situation.
 
When I moved to C# from C++, one of the things I missed the most was macros. Certainly they have a well-deserved reputation for causing unreadable difficult to maintain code, but I was lucky and worked with an architect who had used them in a sparingly to add consistent instrumentation and error handling to what would become a rather sprawling web application. 
 
When it came time to write my own applications in C# one of my frustrations was not having this tool available to me for boilerplate code. I tried to use inheritance or homespun reflection to get around the issue, but one was the wrong tool and the other felt like I was pouring gasoline on the fire. Enter Aspect Oriented Programming and method interception. 
 
Since we're already using Autofac for dependency injection, utilizing Autofac's AOP add on to handle the instrumentation code was an easy choice. Since nothing is ever that straight forward though, the specific case for instrumentation was a series of reports created via class factory, adding a layer of complexity.    
 
    public interface IReport
    {
        void DoReport(string something);
    }

    public class FancyReport : IReport
    {
        public void DoReport(string something)
        {
            Console.WriteLine("Report {0}", something);
        }

    }

This class is pretty simple. It has a function defined in IReport called DoReport that I want to instrument. I don't particularly care if any other functions are instrumented or not.

Before doing anything else, I got Autofac’s Autofac.AOP from nuget. Then, I created a new interceptor class.
 
    public class ItsLog:IInterceptor
    {
        public ItsLog(string statsdServer, int statsdPort)
        {
            var statsdConfig = new StatsdConfig
            {
                StatsdServerName = statsdServer,
                StatsdPort = statsdPort
            };

            StatsdClient.DogStatsd.Configure(statsdConfig);

        }

        public void Intercept(IInvocation invocation)
        {

var name = string.Format("{0}.{1}", invocation.TargetType.FullName, invocation.MethodInvocationTarget.Name);
            
            DogStatsd.Increment(name);

            using (DogStatsd.StartTimer(name))
            {
                invocation.Proceed();
            }

            DogStatsd.Decrement(name);
            
        }
    }


The constructor on the class takes in the necessary elements for configuring our statsd client. The Intercept function will instrument the invoked method. It logs to the statsd that the method was called, uses the statsd library’s build in stopwatch, and then invokes the method that was intercepted. After the invoked method's code completes the interceptor logs that the method finished.
 
If you are using Autofac you should already have the wiring for dependency injection. All but one of the changes for adding method interception can be included there:
 
            builder.RegisterType<FancyReport>()
                .As<FancyReport>()
                .EnableClassInterceptors()
                .InterceptedBy(typeof (ItsLog));

            builder.RegisterType<FancyReport2>()
                .As<FancyReport2>()
                .EnableClassInterceptors()
                .InterceptedBy(typeof(ItsLog));

            builder.Register(c => new ItsLog("127.0.0.1", 8125));

            var container = builder.Build();

When we register the type we also enable the class interceptors and say that the class will be intercepted by ItsLog. We also register ItsLog, with the constructor parameters for our statsd server, at this point so that it is ready to be used.
 
public virtual void DoReport(string something)
 
Above is the only change we have to make to DoReport in FancyReport. The function has to be virtual to allow the interception to happen. Failing to make it virtual will result in an error. Underneath the covers Autofac is creating a proxy class that includes the code from ItsLog, and unless the function is virtual this isn't possible. You can avoid this requirement if you do the interception at the Interface level rather than the class level, but since I have many report classes sharing the IReport interface that wasn't an option.
 
            var intercepted = container.Resolve<FancyReport>();
            intercepted.DoReport("first");
            intercepted.DoReport("second");
 
The code above gets an instance of LoggedType and then calls DoSomething twice with different inputs. The result when the class is invoked and the function is called it will be intercepted by the Intercept function which will call the statsd client.

