using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Application.Service;
using Domain.Document;
using Domain.Repository;
using Moq;
using Xunit;

namespace tests.Application.Service
{
    public class DailyCashFlowServiceTest
    {
        [Fact]
        public async Task UpdateCashFlow_NoPreviowsDay()
        {
            var cashFlowApi = new DailyCashFlow("123abcabc", DateOnly.FromDateTime(DateTime.Now), 110.10m);
            var cashFlowDb = new DailyCashFlow("123abcabc", DateOnly.FromDateTime(DateTime.Now), 100.10m) { Id = "abcdefgh" };
            var repoMock = new Mock<IRepository<DailyCashFlow>>();

            repoMock.SetupSequence(
                r => r.FindOne(It.IsAny<Expression<Func<DailyCashFlow, bool>>>())
            )
                .ReturnsAsync(cashFlowDb)
                .ReturnsAsync((DailyCashFlow?) null);

            repoMock.Setup(r => r.Update(cashFlowApi)).ReturnsAsync(cashFlowApi);

            var service = new DailyCashFlowService(repoMock.Object);

            var cashFlowResult = await service.Save(cashFlowApi);

            Assert.Equal("abcdefgh", cashFlowResult.Id);
            Assert.Equal(110.10m, cashFlowResult.Amount);
        }

        [Fact]
        public async Task UpdateCashFlow_WithPreviowsDay_SumYesterdayValueToTodaysValue()
        {
            var cashFlowApi = new DailyCashFlow("123abcabc", DateOnly.FromDateTime(DateTime.Now), 110.10m);
            var cashFlowDb = new DailyCashFlow("123abcabc", DateOnly.FromDateTime(DateTime.Now), 100.10m) { Id = "abcdefgh" };
            var cashFlowYesterday = new DailyCashFlow("123abcabc", DateOnly.FromDateTime(DateTime.Now.AddDays(-1)), 200.10m) { Id = "abcdefg" };
            var repoMock = new Mock<IRepository<DailyCashFlow>>();

            repoMock.SetupSequence(
                r => r.FindOne(It.IsAny<Expression<Func<DailyCashFlow, bool>>>())
            )
                .ReturnsAsync(cashFlowDb)
                .ReturnsAsync(cashFlowYesterday);

            repoMock.Setup(r => r.Update(cashFlowApi)).ReturnsAsync(cashFlowApi);

            var service = new DailyCashFlowService(repoMock.Object);

            var cashFlowResult = await service.Save(cashFlowApi);

            Assert.Equal("abcdefgh", cashFlowResult.Id);
            Assert.Equal(310.20m, cashFlowResult.Amount);
        }
    }
}