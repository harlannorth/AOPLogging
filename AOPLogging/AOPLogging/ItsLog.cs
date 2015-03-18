using System;
using Castle.DynamicProxy;


namespace AOPLogging
{
    class ItsLog:IInterceptor
    {
        public void Intercept(IInvocation invocation)
        {

            var name = string.Format("{0}.{1}", invocation.TargetType.FullName, invocation.MethodInvocationTarget.Name);

            Console.WriteLine("{0}.start", name);

            var timed = new System.Diagnostics.Stopwatch();
            timed.Start();
            
            invocation.Proceed();
            
            timed.Stop();


            Console.WriteLine("{0}.end", name);
            Console.Write("{0}:{1}", name, timed.ElapsedMilliseconds);
            
        }
    }
}
