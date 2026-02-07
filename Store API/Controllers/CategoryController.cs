using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Store_API.Data;
using Store_API.Models;
using Store_API.Repositories;
using HSC = System.Net.HttpStatusCode;

namespace Store_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ILogger<CategoryController> _logger;
        private readonly ICategoryRepos _repos;
        private readonly IMapper _mapper;
        private APIResponse _apiResponse;
        private APIRequestBody _requestBody;

        public CategoryController(ILogger<CategoryController> logger, ICategoryRepos repos, IMapper mapper)
        {
            _logger = logger;
            _repos = repos;
            _mapper = mapper;
            _apiResponse = new();
            _requestBody = new();
        }

        [HttpGet("All", Name = "GetAllCategories")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> GetAllCategoriesAsync()
        {
            try
            {
                _logger.LogInformation("Getting All Categories");
                var categories = await _repos.GetAllAsync();
                _requestBody.Data = _mapper.Map<List<CategoryDTO>>(categories);
                return Ok(APIRBuilder(true, HSC.OK, _requestBody));
            }catch (Exception ex)
            {
                return CommonExceptionHandling(ex);
            }
        }

        [HttpGet("{id:int}", Name = "GetCategoryById")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> GetCategoryByIdAsync(int id)
        {
            try
            {
                _logger.LogInformation($"Getting A Category By Id: {id}");
                if (id <= 0) return BadRequest("Invalid Id");
                var category = await _repos.GetAsync(c => c.ID == id);
                if (category == null) return NotFound("Couldn't Find A Category With Such Id");
                _requestBody.Data = _mapper.Map<CategoryDTO>(category);
                return Ok(APIRBuilder(true, HSC.OK, _requestBody));
            }catch (Exception ex)
            {
                return CommonExceptionHandling(ex);
            }
        }

        [HttpGet("{name:alpha", Name = "GetCategoryByName")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> GetCategoryByNameAsync(string name)
        {
            try
            {
                _logger.LogInformation($"Getting A Category By Name: {name}");
                if (name.IsNullOrEmpty()) return BadRequest("Invalid Name");
                var category = await _repos.GetAsync(c => c.Name.ToLower() == name.ToLower());
                if (category == null) return NotFound("Couldn't Find A Category With Such Name");
                _requestBody.Data = _mapper.Map<CategoryDTO>(category);
                return Ok(APIRBuilder(true, HSC.OK, _requestBody));
            }catch (Exception ex)
            {
                return CommonExceptionHandling(ex);
            }
        }

        [HttpDelete("{id:int}", Name = "DeleteCategoryById")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> DeleteCategoryByIdAsync(int id)
        {
            try
            {
                _logger.LogInformation($"Deleting A Category By Id: {id}");
                if (id <= 0) return BadRequest("Invalid Id");
                var category = await _repos.GetAsync(c => c.ID == id);
                if (category == null) return NotFound("Couldn't Find A Category With Such Id");
                await _repos.DeleteAsync(category);
                _requestBody.Data = $"Sucesfully Deleted A Category By Id: {id}";
                return Ok(APIRBuilder(true, HSC.OK, _requestBody));
            }catch (Exception ex)
            {
                return CommonExceptionHandling(ex);
            }
        }

        [HttpPost("Create")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> CreateCategoryAsync(CategoryDTO model)
        {
            try
            {
                _logger.LogInformation("Creating A Category");
                if (model == null) return BadRequest("Invalid Model");
                var category = await _repos.CreateAsync(_mapper.Map<Category>(model));
                model.ID = category.ID;
                _requestBody.Data = model;
                return CreatedAtRoute("GetCategoryById", new { id = model.ID }, APIRBuilder(true, HSC.OK, _requestBody));
            }catch (Exception ex)
            {
                return CommonExceptionHandling(ex);
            }
        }

        [HttpPut("Update")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> UpdateCategoryAsync(CategoryDTO model)
        {
            try
            {
                _logger.LogInformation($"Updating A Category With Id: {model.ID}");
                if (model == null) return BadRequest("Invalid Model");
                var category = await _repos.GetAsync(c => c.ID == model.ID);
                if (category == null) return NotFound("Couldn't Find A Category With Such Id");
                await _repos.UpdateAsync(_mapper.Map<Category>(model));
                return NoContent();
            } catch (Exception ex)
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
    }
}
