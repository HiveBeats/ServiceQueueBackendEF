using System;
using WebApi.Models;
using Xunit;

namespace WebApi.Tests.Features.Queue
{
    public class MessageModelTests
    {
        [Fact]
        public void SolveMessageSolves()
        {
            var item = new Message();

            item.Solve();

            Assert.NotNull(item.DateSolved);
        }
    }
}