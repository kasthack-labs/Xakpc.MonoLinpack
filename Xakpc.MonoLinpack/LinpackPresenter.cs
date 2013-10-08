using System.Threading.Tasks;

namespace Xakpc.MonoLinpack.Core
{
    public class LinpackPresenter
    {
        ILinpackView _view;

        public LinpackPresenter(ILinpackView view)
        {
            _view = view;
        }

        public async Task CalculateAsync()
        {
            var l = new Linpack();
            await Task.Factory.StartNew(l.RunBenchmark);

            _view.Mflops = l.MFlops;
            _view.NormRes = l.ResIDN;
            _view.Precision = l.Eps;
            _view.Time = l.Time;
        }
    }
}

