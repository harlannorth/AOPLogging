using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AOPLogging
{

    /// <summary>
    /// Implementor of IReport, will be intercepted
    /// </summary>
    public class FancyReport2 : IReport
    {
        public virtual void DoReport(string something)
        {
            CalledSomething(something);
        }

        public void CalledSomething(string something)
        {
            Console.WriteLine("Did {0}", something);
        }

    }
}
