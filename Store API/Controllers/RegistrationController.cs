using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Store_API.Models;
using HSC = System.Net.HttpStatusCode;

namespace Store_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegistrationController : ControllerBase
    {
        private readonly ILogger<RegistrationController> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private APIRequestBody _requestBody;

        public RegistrationController(ILogger<RegistrationController> logger, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _logger = logger;
            _userManager = userManager;
            _signInManager = signInManager;
            _requestBody = new();
        }

        [HttpPost("Register")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<APIResponse>> RegisterUserAsync([FromBody] RegisterDTO userDTO)
        {
            try
            {
                _logger.LogInformation("Registering New User");
                if (userDTO == null || IsValid(new[] { userDTO.Name, userDTO.Email, userDTO.Password })) return BadRequest();
                ApplicationUser newUser = new()
                {
                    UserName = userDTO.Name,
                    Email = userDTO.Email
                };
                var result = await _userManager.CreateAsync(newUser, userDTO.Password);
                if (!result.Succeeded) return BadRequest(result.Errors);
                _requestBody.Data = result;
                return Ok(APIRBuilder(true, HSC.OK, _requestBody));
            }catch (Exception ex)
            {
                return CommonExceptionHandling(ex);
            }
        }

        [HttpPost("Login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<APIResponse>> LoginUserAsync([FromBody] LoginUserDTO user)
        {
            try
            {
                _logger.LogInformation("Logging In User");
                if (user == null || IsValid(new[] { user.Name, user.Email, user.Password })) return BadRequest();
                var existingUser = await _userManager.FindByEmailAsync(user.Email);
                if (existingUser == null) return BadRequest("Invalid Credinteals");
                var result = await _signInManager.PasswordSignInAsync(existingUser, user.Password, true, false);
                if (!result.Succeeded) return BadRequest("Invalid Credinteals");
                _requestBody.Data = "Successfully Logged In User";
                return Ok(APIRBuilder(true, HSC.OK, _requestBody));

            }
            catch (Exception ex)
            {
                return CommonExceptionHandling(ex);
            }
        }

        [HttpPost("LogOut")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<APIResponse>> LogOutUserAsync()
        {
            try
            {
                _logger.LogInformation("Logging Out User");
                await _signInManager.SignOutAsync();
                _requestBody.Data = "Successfully Signed Out User";
                return Ok(APIRBuilder(true, HSC.OK, _requestBody));
            }
            catch (Exception ex)
            {
                return CommonExceptionHandling(ex);
            }
        }

        private APIResponse APIRBuilder(bool Status, HSC StatusCode, [FromBody] APIRequestBody requestBody)
        {
            APIResponse apiResponse = new APIResponse()
            {
                Status = Status,
                StatusCode = StatusCode,
                Data = requestBody.Data,
                Errors = requestBody.Errors
            };
            if (apiResponse == null) return null;
            return apiResponse;
        }

        private APIResponse CommonExceptionHandling(Exception ex)
        {
            List<string> errors = new List<string>();
            errors.Add(ex.Message);
            _requestBody.Errors = errors;
            return APIRBuilder(false, HSC.InternalServerError, _requestBody);
        }

        private bool IsValid(IEnumerable<string> messages) => messages.Any(string.IsNullOrWhiteSpace);
    }
}
