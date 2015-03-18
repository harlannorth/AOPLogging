using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AOPLogging
{
    
    /// <summary>
    /// Reuseable Stopwatch wrapper
    /// Base concept taken from http://tech.pro/tutorial/1237/simple-c-benchmarking-with-stopwatch
    /// </summary>
    public class Benchmark : IDisposable
    {
        readonly Stopwatch _watch;
        readonly string _name;

        public static Benchmark Start(string name)
        {
            return new Benchmark(name);
        }

        private Benchmark(string name)
        {
            _name = name;
            _watch = new Stopwatch();
            _watch.Start();
        }

        #region IDisposable implementation

        // dispose stops stopwatch and prints time, could do anytying here
        public void Dispose()
        {
            _watch.Stop();
            Console.WriteLine("{0} Total ms: {1}"
                               , _name, _watch.Elapsed.Milliseconds);
        }

        #endregion
    }
}
