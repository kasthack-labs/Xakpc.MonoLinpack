using NUnit.Framework;
using NSubstitute;

namespace Xakpc.MonoLinpack.Core.Test
{
    [TestFixture]
    public class CoreTest
    {        
        ILinpackView _view;
        LinpackPresenter _presenter;

        [TestFixtureSetUp]
        public void Setup()
        {
            this._view = Substitute.For<ILinpackView>();
            this._presenter = new LinpackPresenter(this._view);
        }

        [Test]
        public void BenchTest()
        {
            var lp = new Linpack();
            lp.RunBenchmark();

            Assert.Pass();
        }

        [Test]
        public void CalculateAsyncTest()
        {
            this._presenter.CalculateAsync().Wait();
            Assert.That(this._view.Mflops, Is.EqualTo(130.0).Within(5).Percent);            
        }
    }
}
