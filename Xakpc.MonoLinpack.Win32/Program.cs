using System;
using System.Diagnostics;
using Xakpc.MonoLinpack.Core;

namespace Xakpc.MonoLinpack.Win32 {
    static class Program {
        static void Main() {
            Debug.Listeners.Add(new ConsoleTraceListener());
            for (var i=0;i<100;i++){
                var lp = new Linpack();
                lp.RunBenchmark();
                Console.WriteLine("Mflops/s: " + lp.MFlops +
                                  "  Time: " + lp.Time +
                                  "  Norm Res: " + lp.ResIDN +
                                  "  Precision: " + lp.Eps);
            }
        }
    }
}
