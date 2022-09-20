using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SellerService.Models;
using SellerService;
using SellerService.Enums;
using SellerService.RepositoryLayer.Interfaces;

namespace AuthServer.Controller
{
    [Route("/e-auction/api/v1/[controller]")]
    [ApiController]
    //[Authorize]
    public class SellerController : ControllerBase
    {
        private readonly ISellerRepository _sellerRepository;
        private readonly ILogger<SellerController> _logger;
        public SellerController(ISellerRepository sellerRepository, ILogger<SellerController> logger)
        {
            _sellerRepository = sellerRepository;
            _logger = logger;
        }

        [HttpPost("add-product")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<AddProductSuccessResponse>> AddProduct([FromBody] ProductAndSeller productToAdd)
        {
            try
            {
                await _sellerRepository.AddProductAsync(productToAdd);
                return new AddProductSuccessResponse { ProductId = productToAdd.Id, Message = GlobalVariables.AddProductSuccessMsg };

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Some internal error happened.");
                var details = ProblemDetailsFactory.CreateProblemDetails(HttpContext, 500, "Internal Server Error", null, "Error while adding the product.");
                return StatusCode(500, details);
            }
        }

        // DELETE product
        [HttpDelete("delete")]
        [ProducesResponseType(typeof(ProductAndSeller), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> DeleteProduct(string productId)
        {
            try
            {
                return Ok(await _sellerRepository.DeleteProductAsync(productId));
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
                var details = ProblemDetailsFactory.CreateProblemDetails(HttpContext, 500, "Internal Server Error", null, "Error while Deleting the product.");
                return StatusCode(500, details);
            }
        }

        [HttpGet("show-bids")]
        [ProducesResponseType(typeof(ShowBidsResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ShowBidsResponse>> ShowBids(string productId)
        {
            try
            {
                var result = await _sellerRepository.GetAllBidDetailsAsync(productId);
                return Ok(result); 
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Some internal error happened.");
                var details = ProblemDetailsFactory.CreateProblemDetails(HttpContext, 500, "Internal Server Error", null, "Error while getting bid details.");
                return StatusCode(500, details);
            }
        }
    }
}
