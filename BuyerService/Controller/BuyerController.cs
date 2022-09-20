using BuyerService.Enums;
using BuyerService.Models;
using BuyerService.RepositoryLayer.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AuthServer.Controller
{
    [Route("e-auction/api/v1/[controller]")]
    [ApiController]
    //[Authorize]
    public class BuyerController : ControllerBase
    {
        private readonly IBuyerRepository _repository;
        private readonly ILogger<BuyerController> _logger;
        public BuyerController(IBuyerRepository repository, ILogger<BuyerController> logger)
        {
            _repository = repository;
            _logger = logger;
        }
        // POST: BuyerController/AddBid
        [HttpPost("place-bid")]
        [ProducesResponseType(typeof(BidAndBuyer), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<AddBidResponse>> AddBid([FromBody] BidAndBuyer bidDetails)
        {
            try
            {
                await _repository.AddBid(bidDetails);
                return new AddBidResponse { BidId = bidDetails.Id, Message = GlobalVariables.AddBidSuccessMsg };
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogError(ex.Message);
                var details = ProblemDetailsFactory.CreateProblemDetails(HttpContext, 400, "Bad request", null, ex.Message);
                return StatusCode(400, details);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Some internal error happened.");
                var details = ProblemDetailsFactory.CreateProblemDetails(HttpContext, 500, "Internal Server Error", null, "Error while adding the Bid.");
                return StatusCode(500, details);
            }
        }

        [HttpPatch("update-bid")]
        [ProducesResponseType(typeof(BidAndBuyer), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> EditBid(string productId, string buyerEmailId, double newBidAmount)
        {
            try
            {
                return Ok(await _repository.UpdateBid(productId, buyerEmailId, Convert.ToDouble(newBidAmount)));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Some internal error happened.");
                var details = ProblemDetailsFactory.CreateProblemDetails(HttpContext, 500, "Internal Server Error", null, "Error while updating the bid amount of the product.");
                return StatusCode(500, details);
            }
        }
    }
}
