﻿using SellerService.BusinessLayer.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SellerService.RepositoryLayer.Interfaces;
using SellerService.RepositoryLayer;
using Microsoft.Extensions.Logging;
using SellerService.Models;
using Confluent.Kafka;
using Newtonsoft.Json;

namespace SellerService.BusinessLayer
{
    public class SellerBusinessLogic : ISellerBusinessLogic
    {
        private readonly ISellerRepository _sellerRepository;
        private readonly ILogger<SellerBusinessLogic> _logger;
        //private readonly ProducerConfig _producerconfig;
        //private readonly ConsumerConfig _consumerConfig;
        private readonly static Dictionary<string, bool> bidResponseFromBuyer = new Dictionary<string, bool>();
        private readonly static Dictionary<string, List<BidDetails>> bidDetailsResponseFromBuyer = new Dictionary<string, List<BidDetails>>();

        public SellerBusinessLogic(ISellerRepository sellerRepository, ILogger<SellerBusinessLogic> logger)//, ProducerConfig producerConfig, ConsumerConfig consumerConfig)
        {
            _sellerRepository = sellerRepository;
            _logger = logger;
            //_producerconfig = producerConfig;
            //_consumerConfig = consumerConfig;
        }
        public async Task AddProductBLayerAsync(ProductAndSeller productObj)
        {
            await _sellerRepository.AddProductAsync(productObj);
        }

        public async Task DeleteProductBLayerAsync(string productId)
        {
            try
            {
                //Get the added product from DB
                ProductAndSeller productDetails = await _sellerRepository.GetProductAsync(productId);

                //Check the Bid end date
                if (productDetails.BidEndDate < DateTime.Now)
                    throw new KeyNotFoundException("Product cannot be deleted after the BidEnd date.");

                //If Any bid is already placed Dont delete the product
                await TopicMessagePublisher("SellerProducer", "checkBidDetails", productId);

                while (true)
                {
                    if (bidResponseFromBuyer.ContainsKey(productId))
                    {
                        if (bidResponseFromBuyer.FirstOrDefault(x => x.Key == productId).Value == false)
                        {
                            bidResponseFromBuyer.Remove(productId);
                            throw new KeyNotFoundException("Product can not be deleted because there is already a bid placed for this product.");
                        }
                        else
                        {
                            bidResponseFromBuyer.Remove(productId);
                            break;
                        }
                    }
                }

                //Delete the product
                await _sellerRepository.DeleteProductAsync(productId);

            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public async Task<ShowBidsResponse> GetAllBidDetailsAsync(string productId)
        {
            try
            {
                ShowBidsResponse getAllBidsResponse = new ShowBidsResponse();
                var productDetails = await _sellerRepository.GetProductAsync(productId);

                getAllBidsResponse.ProductName = productDetails.ProductName;
                getAllBidsResponse.ShortDescription = productDetails.ProductShortDescription;
                getAllBidsResponse.DetailedDescription = productDetails.ProductDetailedDescription;
                getAllBidsResponse.StartingPrice = productDetails.ProductStartingPrice;
                getAllBidsResponse.Category = productDetails.ProductCategory;
                getAllBidsResponse.BidEndDate = productDetails.BidEndDate;

                //Get the Bid Details
                await TopicMessagePublisher("SellerProducer","GetAllBids",productId);

                while (true)
                {
                    if (bidDetailsResponseFromBuyer.ContainsKey(productId))
                    {
                        getAllBidsResponse.Bids.AddRange(bidDetailsResponseFromBuyer[productId].OrderByDescending(key => key.BidAmount));
                        bidDetailsResponseFromBuyer.Remove(productId);
                        break;
                    }
                }

                return getAllBidsResponse;
            }
            catch (Exception ex)
            {

                throw;
            }
        }
        public async Task TopicMessagePublisher(string topic,string key, string value)
        {
            //try
            //{
            //    using (var producer = new ProducerBuilder<string, string>(_producerconfig).Build())
            //    {
            //        await producer.ProduceAsync(topic, new Message<string, string> { Key = key, Value = value });
            //        producer.Flush(TimeSpan.FromSeconds(10));
            //    }
            //}
            //catch (Exception)
            //{

            //    throw;
            //}
        }

        public void IsBidPresentForProductId(string message)
        {
            try
            {
                var response = JsonConvert.DeserializeObject<IsBidPresentResponse>(message);
                bidResponseFromBuyer.Add(response.ProductId, response.IsBidPresent);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task IsBidDateValidAsync(ValidateDateRequest request)
        {
            try
            {
                var productDetails = await _sellerRepository.GetProductAsync(request.ProductId);
                if (request.BidDate > productDetails.BidEndDate)
                {
                    var response = new ValidateDateResponse { ProductId = request.ProductId, IsValid = false, Operation = request.Operation };
                    await TopicMessagePublisher("SellerProducer", "isBidDateValid", JsonConvert.SerializeObject(response));
                }
                else
                {
                    var response = new ValidateDateResponse { ProductId = request.ProductId, IsValid = true, Operation = request.Operation };
                    await TopicMessagePublisher("SellerProducer", "isBidDateValid", JsonConvert.SerializeObject(response));
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
        public void CollateBidsResponse(string productId, List<BidDetails> bids)
        {
            try
            {
                bidDetailsResponseFromBuyer.Add(productId, bids);
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
