using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Xunit;

namespace Tests
{
    public class TestClass
    {
        [VsFact]
        void FactTest()
            => Assert.NotNull(Package.GetGlobalService(typeof(SVsWebBrowsingService)));

        [VsTheory]
        [InlineData(123)]
        void TheoryTest(int n)
        {
            Assert.NotNull(Package.GetGlobalService(typeof(SVsWebBrowsingService)));
            Assert.Equal(123, n);
        }
    }
}