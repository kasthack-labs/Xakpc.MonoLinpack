using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Xakpc.MonoLinpack.Core
{
    public interface ILinpackView
    {
        double Mflops { get; set; }
        string Time { set; }
        double NormRes { set; }
        double Precision { set; }
    }
}
