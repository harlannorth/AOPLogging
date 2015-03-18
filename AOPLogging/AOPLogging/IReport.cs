using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AOPLogging
{
    /// <summary>
    /// Interface that represents reports
    /// </summary>
    public interface IReport
    {
        void DoReport(string something);
    }
}
