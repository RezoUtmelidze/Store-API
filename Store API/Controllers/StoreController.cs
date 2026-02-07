using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Store_API.Data;
using Store_API.Models;
using Store_API.Repositories;
using HSC = System.Net.HttpStatusCode;

namespace Store_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StoreController : ControllerBase
    {
        private readonly ILogger<StoreController> _logger;
        private readonly IStoreRepos _repos;
        private readonly IMapper _mapper;
        private APIResponse _apiResponse;
        private APIRequestBody _requestBody;

        public StoreController(ILogger<StoreController> logger, IStoreRepos repos, IMapper mapper)
        {
            _logger = logger;
            _repos = repos;
            _mapper = mapper;
            _apiResponse = new();
            _requestBody = new();
        }

        [HttpGet("All", Name = "GetAllStores")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> GetAllStoresAsync()
        {
            try
            {
                _logger.LogInformation("Getting All Stores");
                List<Store> Stores = await _repos.GetAllAsync();
                _requestBody.Data = _mapper.Map<List<StoreDTO>>(Stores);
                return Ok(APIRBuilder(true, HSC.OK, _requestBody));
            }
            catch (Exception ex)
            {
                return CommonExceptionHandling(ex);
            }
        }

        [HttpGet]
        [Route("{id:int}", Name = "GetStoreById")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<APIResponse>> GetStoreByIdAsync(int id)
        {
            try
            {
                _logger.LogInformation($"Getting A Store With Id: {id}");
                if (id <= 0) return BadRequest("Invalid Id");
                Store store = await _repos.GetAsync(s => s.ID == id);
                if (store == null) return NotFound("Couldn't Find A Store With Such Id");
                _requestBody.Data = _mapper.Map<StoreDTO>(store);
                return Ok(APIRBuilder(true, HSC.OK, _requestBody));
            }
            catch (Exception ex)
            {
                return CommonExceptionHandling(ex);
            }
        }

        [HttpDelete("{id:int}", Name = "DeleteStoreById")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> DeleteStoreByIdAsync(int id)
        {
            try
            {
                _logger.LogInformation($"Deleting A Store By Id: {id}");
                if (id <= 0) return BadRequest("Invalid Id");
                Store store = await _repos.GetAsync(s => s.ID == id);
                if (store == null) return NotFound("Couldn't Find A Store With Such Id");
                await _repos.DeleteAsync(store);
                _requestBody.Data = $"Succesfully Deleted A Store By Id: {id}";
                return Ok(APIRBuilder(true, HSC.OK, _requestBody));
            }
            catch (Exception ex)
            {
                return CommonExceptionHandling(ex);
            }
        }

        [HttpPost("Create")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> CreateStoreAsync([FromBody] StoreDTO model)
        {
            try
            {
                _logger.LogInformation("Creating New Store");
                if (model == null) return BadRequest("Invalid Model");
                var newStore = await _repos.CreateAsync(_mapper.Map<Store>(model));
                model.ID = newStore.ID;
                _requestBody.Data = model;
                return CreatedAtRoute("GetStoreById", new {id = model.ID}, APIRBuilder(true, HSC.OK, _requestBody));
            }
            catch (Exception ex)
            {
                return CommonExceptionHandling(ex);
            }
        }

        [HttpPut("Update")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> UpdateStoreAsync([FromBody] StoreDTO model)
        {
            try
            {
                _logger.LogInformation($"Updating A Store With Id: {model.ID}");
                if (model == null) return BadRequest("Invalid Model");
                var store = await _repos.GetAsync(s => s.ID == model.ID);
                if (store == null) return NotFound("Couldn't Find A Store With Such Id");
                await _repos.UpdateAsync(_mapper.Map<Store>(model));
                return NoContent();
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
    }
}
