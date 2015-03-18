using System;
using Castle.DynamicProxy;
using StatsdClient;


namespace AOPLogging
{
    class ItsLog:IInterceptor
    {


        /// <summary>
        /// Constructor that takes in the statsd info
        /// </summary>
        /// <param name="statsdServer">the statsd server</param>
        /// <param name="statsdPort">the statsd port</param>
        public ItsLog(string statsdServer, int statsdPort)
        {
            var statsdConfig = new StatsdConfig
            {
                StatsdServerName = statsdServer,
                StatsdPort = statsdPort
            };

            StatsdClient.DogStatsd.Configure(statsdConfig);

        }
        
        /// <summary>
        /// Method performs our interception
        /// </summary>
        /// <param name="invocation">the initially invoked method</param>
        public void Intercept(IInvocation invocation)
        {

            var name = string.Format("{0}.{1}", invocation.TargetType.FullName, invocation.MethodInvocationTarget.Name);
            
            //I want to know this method was started
            DogStatsd.Increment(name);

            //tell me how long the report took
            using (DogStatsd.StartTimer(name))
            {
                invocation.Proceed();
            }

            //I want to know the report was finished
            DogStatsd.Decrement(name);
            
        }
    }

  
}
