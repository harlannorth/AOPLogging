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
    public class FancyReport : IReport
    {
        public virtual void DoReport(string something)
        {
            Console.WriteLine("Report {0}", something);

        }

    }
}
