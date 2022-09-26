using AuthServer.Models;
using AuthServer.RepositoryLayer.Interfaces;
using AuthServer.Token;
using AuthServer.Token.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthServer.Controller
{
    [Route("e-auction/api/v1/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class ValidateController : ControllerBase
    {
        private readonly ILogger<ValidateController> _logger;
        private readonly ITokenService _tokenService;
        private readonly IConfiguration _configuration;
        private readonly IUserRepository _repositoty;

        public ValidateController(ILogger<ValidateController> logger, ITokenService tokenService, IConfiguration configuration, IUserRepository repositoty)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _repositoty = repositoty ?? throw new ArgumentNullException(nameof(repositoty));
        }

        [HttpPost()]
        public async Task<ActionResult<TokenResponse>> Validate(UserValidationRequestModel request)
        {
            try
            {
                var authorizedUser = await _repositoty.ValidateUser(request.UserName.Trim(), request.Password.Trim());
                if(authorizedUser)
                {
                    var tokenResponse = _tokenService.BuildToken(_configuration.GetValue<string>("Jwt:Key"),
                                                        _configuration.GetValue<string>("Jwt:Issuer"),
                                                        new[]
                                                        {
                                                        _configuration.GetValue<string>("Jwt:Aud1"),
                                                        _configuration.GetValue<string>("Jwt:Aud2"),
                                                        _configuration.GetValue<string>("Jwt:Aud3")
                                                                },
                                                        request.UserName);
                    return tokenResponse;
                }
                return (new TokenResponse
                {
                    Token = string.Empty,
                    IsAuthenticated = false
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Some internal error happened.");
                var details = ProblemDetailsFactory.CreateProblemDetails(HttpContext, 500, "Internal Server Error", null, "Error while generating the token.");
                return StatusCode(500, details);
            }
        }
    }
}
