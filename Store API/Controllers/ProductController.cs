using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Store_API.Data;
using Store_API.Models;
using Store_API.Repositories;
using HSC = System.Net.HttpStatusCode;

namespace Store_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly ILogger<ProductController> _logger;
        private readonly IProductRepos _repos;
        private readonly IMapper _mapper;
        private readonly StoreWADbContext _context;
        private APIResponse _apiResponse;
        private APIRequestBody _requestBody;

        public ProductController(ILogger<ProductController> logger, IProductRepos repos, IMapper mapper, StoreWADbContext context)
        {
            _logger = logger;
            _repos = repos;
            _mapper = mapper;
            _context = context;
            _apiResponse = new();
            _requestBody = new();
        }

        [HttpGet("All", Name = "GetAllProducts")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> GetAllProductsAsync()
        {
            try
            {
                _logger.LogInformation("Getting All Products");
                var products = await _repos.GetAllProductsWithCategoriesAsync();
                _requestBody.Data = _mapper.Map<List<ProductDTO>>(products);
                return Ok(APIRBuilder(true, HSC.OK, _requestBody));
            }catch (Exception ex)
            {
                return CommonExceptionHandling(ex);
            }
        }

        [HttpGet("{id:int}", Name = "GetProductById")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> GetProductByIdAsync(int id)
        {
            try
            {
                _logger.LogInformation($"Getting A Product By Id: {id}");
                if (id <= 0) return BadRequest("Invalid Id");
                var product = await _repos.GetAsync(p => p.ID == id);
                if (product == null) return NotFound();
                _requestBody.Data = _mapper.Map<ProductDTO>(product);
                return Ok(APIRBuilder(true, HSC.OK, _requestBody));
            }catch (Exception ex)
            {
                return CommonExceptionHandling(ex);
            }
        }

        [HttpGet("Filter", Name = "GetProductsByFilter")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> GetProductsByFilterAsync(int? categoryId, int? from, int? to)
        {
            try
            {
                _logger.LogInformation("Getting Products By Filter");
                if (categoryId.HasValue && categoryId <= 0) return BadRequest("Invalid Id");
                if (from.HasValue && to.HasValue && to < from) return BadRequest("Invalid Price Range");
                IQueryable<Product> query = _context.Products.AsQueryable();
                if (categoryId.HasValue) query = query.Where(p => p.Categories.Any(c => c.ID == categoryId));
                if (from.HasValue) query = query.Where(p => p.Price >= from);
                if (to.HasValue) query = query.Where (p => p.Price <= to);
                if (!query.Any()) return NotFound();
                _requestBody.Data = _mapper.Map<List<ProductDTO>>(await query.ToListAsync());
                return Ok(APIRBuilder(true, HSC.OK, _requestBody));
            }catch (Exception ex)
            {
                return CommonExceptionHandling(ex);
            }
        }

        [HttpDelete("{id:int}", Name = "DeleteProductById")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> DeleteProductByIdAsync(int id)
        {
            try
            {
                _logger.LogInformation($"Deleting A Product By Id: {id}");
                if ( id <= 0 ) return BadRequest("Invalid Id");
                var product = await _repos.GetAsync(p => p.ID == id);
                if (product == null) return NotFound("Couldn't Find A Product With Such Id");
                await _repos.DeleteAsync(product);
                _requestBody.Data = $"Succesfully Deleted A Product By Id: {id}";
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
        public async Task<ActionResult<APIResponse>> CreateProductAsync(ProductDTO model)
        {
            try
            {
                _logger.LogInformation("Creating A Product");
                if ( model == null ) return BadRequest("Invalid Model");
                var fullModel = _mapper.Map<Product>(model);
                fullModel.Categories = await _context.Categories.Where(c => model.CategoryIDs.Contains(c.ID)).ToListAsync();
                var product = await _repos.CreateAsync(fullModel);
                _logger.LogInformation($"Category: {product.Categories.First().ID}");
                model.ID = product.ID;
                _requestBody.Data = model;
                return CreatedAtRoute("GetProductById", new  { id = model.ID }, APIRBuilder(true, HSC.OK, _requestBody));
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
        public async Task<ActionResult<APIResponse>> UpdateProductAsync(ProductDTO model)
        {
            try
            {
                _logger.LogInformation($"Updating A Product With Id: {model.ID}");
                if ( model == null ) return BadRequest("Invalid Model");
                var product = await _repos.GetAsync(p => p.ID == model.ID);
                if ( product == null ) return NotFound("Couldn't Find A Product With Such Id");
                await _repos.UpdateAsync(_mapper.Map<Product>(model));
                return NoContent();
            }catch (Exception ex)
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
