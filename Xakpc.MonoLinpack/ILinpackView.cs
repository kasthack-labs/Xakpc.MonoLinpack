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
