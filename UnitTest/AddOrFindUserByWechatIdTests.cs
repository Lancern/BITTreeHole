using System;
using BITTreeHole.Data;
using BITTreeHole.Test.UnitTest.Mocks;
using Xunit;

namespace BITTreeHole.Test.UnitTest
{
    public class AddOrFindUserByWechatIdTests
    {
        [Fact]
        public void NullArguments()
        {
            Assert.ThrowsAsync<ArgumentNullException>(
                      async () => await DataFacadeExtensions.AddOrFindUserByWechatId(null, null))
                  .Wait();

            Assert.ThrowsAsync<ArgumentNullException>(
                      async () => await DataFacadeExtensions.AddOrFindUserByWechatId(null, string.Empty))
                  .Wait();

            Assert.ThrowsAsync<ArgumentNullException>(
                      async () =>
                          await DataFacadeExtensions.AddOrFindUserByWechatId(
                              new MockDataFacadeBuilder().Build(), null))
                  .Wait();
        }
    }
}
