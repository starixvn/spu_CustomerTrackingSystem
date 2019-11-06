namespace Fanex.NAP.CTS.Test
{
    using Xunit;

    public class UnitTest1
    {
        [Theory]
        [InlineData("1")]
        [InlineData("2")]
        [InlineData(null)]
        public void TestMethod1(string value)
        {
            Assert.Equal("1", value);
        }
    }
}