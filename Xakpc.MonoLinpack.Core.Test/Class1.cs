using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using NSubstitute;

namespace Xakpc.MonoLinpack.Core.Test
{
    [TestFixture]
    public class CoreTest
    {        
        ILinpackView view;
        LinpackPresenter presenter;

        [TestFixtureSetUp]
        public void Setup()
        {
            view = Substitute.For<ILinpackView>();
            presenter = new LinpackPresenter(view);
        }

        [Test]
        public void BenchTest()
        {
            MonoLinpack.Core.Linpack lp = new Linpack();
            lp.RunBenchmark();

            Assert.Pass();
        }

        [Test]
        public void CalculateAsyncTest()
        {
            presenter.CalculateAsync().Wait();
            Assert.That(view.Mflops, Is.EqualTo(130.0).Within(5).Percent);            
        }
    }
}
