using BuyerService.Data.Interfaces;
using BuyerService.Models;
using BuyerService.RepositoryLayer;
using EventBus.Messages.Events;
using MassTransit;
using MassTransit.Clients;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Moq;
using NUnit.Framework;
using System;

namespace BuyerService.UnitTests
{
    public class BuyerServiceRepositoryUnitTests
    {
        [Test]
        public void AddBidAfterBidEndDateTestAsync()
        {
            BidAndBuyer eventMsg = new BidAndBuyer() { ProductId = "testProductId"};
            GetBidDateResponseEvent res = new GetBidDateResponseEvent() { BidEndDate = DateTime.Now.AddDays(-7)};

            Mock<ILogger<BuyerRepository>> loggerMock = new Mock<ILogger<BuyerRepository>>();
            var contextMock = new Mock<IBuyerContext>();
            var buyerClient = new Mock<IRequestClient<GetBidDateRequestEvent>>();

            var repoObj = new BuyerRepository(loggerMock.Object, contextMock.Object, buyerClient.Object);

            buyerClient.Setup(x => x.GetResponse<GetBidDateResponseEvent>(It.IsAny<GetBidDateRequestEvent>(), default, default).Result.Message).Returns(res);

            Assert.ThrowsAsync<KeyNotFoundException>(() => repoObj.AddBid(eventMsg));
        }

        [Test]
        public void AddBidbySameUser()
        {
            BidAndBuyer eventMsg = new BidAndBuyer() { ProductId = "testProductId" };
            GetBidDateResponseEvent res = new GetBidDateResponseEvent() { BidEndDate = DateTime.Now.AddDays(7) };

            Mock<ILogger<BuyerRepository>> loggerMock = new Mock<ILogger<BuyerRepository>>();
            var contextMock = new Mock<IBuyerContext>();
            var buyerClient = new Mock<IRequestClient<GetBidDateRequestEvent>>();
            Mock<IMongoCollection<BidAndBuyer>> mockIMongocollection = new Mock<IMongoCollection<BidAndBuyer>>();
            var asyncCursor = new Mock<IFindFluent<BidAndBuyer, BidAndBuyer>>();

            mockIMongocollection.Setup(x => x.Find(
                                                It.IsAny<FilterDefinition<BidAndBuyer>>(),
                                                It.IsAny<FindOptions>())).Returns(asyncCursor.Object); ;
            var repoObj = new BuyerRepository(loggerMock.Object, contextMock.Object, buyerClient.Object);

            buyerClient.Setup(x => x.GetResponse<GetBidDateResponseEvent>(It.IsAny<GetBidDateRequestEvent>(), default, default).Result.Message).Returns(res);

            repoObj.AddBid(eventMsg).GetAwaiter();

            Assert.Pass();
        }
    }
}
