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
    public class EmployeeController : ControllerBase
    {
        private readonly ILogger<EmployeeController> _logger;
        private readonly IEmployeeRepos _repos;
        private readonly IMapper _mapper;
        private APIResponse _apiResponse;
        private APIRequestBody _requestBody;

        public EmployeeController(ILogger<EmployeeController> logger, IEmployeeRepos repos, IMapper mapper)
        {
            _logger = logger;
            _repos = repos;
            _mapper = mapper;
            _apiResponse = new();
            _requestBody = new();
        }

        [HttpGet("All", Name = "GetAllEmployees")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> GetAllEmployees()
        {
            try
            {
                _logger.LogInformation("Getting All Employees");
                List<Employee> employees = await _repos.GetAllAsync();
                _requestBody.Data = _mapper.Map<List<EmployeeDTO>>(employees);
                return Ok(APIRBuilder(true, HSC.OK, _requestBody));
            } catch (Exception ex)
            {
                return CommonExceptionHandling(ex);
            }
        }

        [HttpGet("{id:int}", Name = "GetEmployeeById")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> GetEmployeeByIdAsync(int id)
        {
            try
            {
                _logger.LogInformation($"Getting An Employee By Id: {id}");
                if (id <= 0) return BadRequest("Invalid Id");
                var employee = await _repos.GetAsync(e => e.ID == id);
                if (employee == null) return NotFound("Couldn't Find An Employee With Such Id");
                _requestBody.Data = _mapper.Map<EmployeeDTO>(employee);
                return Ok(APIRBuilder(true, HSC.OK, _requestBody));
            } catch (Exception ex)
            {
                return CommonExceptionHandling(ex);
            }
        }

        [HttpGet("{position:alpha}", Name = "GetEmployeesByPosition")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<ActionResult<APIResponse>> GetEmployeesByPosition(string position)
        {
            try
            {
                _logger.LogInformation($"Getting Employees By Position: {position}");
                if (string.IsNullOrWhiteSpace(position)) return BadRequest("Invalid Position");
                var employees = await _repos.GetEmployeesAsync(e => e.Position == position);
                if (employees == null) return NotFound();
                _requestBody.Data = _mapper.Map<List<EmployeeDTO>>(employees);
                return Ok(APIRBuilder(true, HSC.OK, _requestBody));
            }
            catch (Exception ex)
            {
                return CommonExceptionHandling(ex);
            }
        }

        [HttpDelete("{id:int}", Name = "DeleteEmployeeById")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> DeleteEmployeeByIdAsync(int id)
        {
            try
            {
                _logger.LogInformation($"Deleting An Employee By Id: {id}");
                if ( id <= 0 ) return BadRequest("Invalid Id");
                var employee = await _repos.GetAsync(e => e.ID == id);
                if (employee == null) return NotFound("Couldn't Find An Employee With Such Id");
                await _repos.DeleteAsync(employee);
                _requestBody.Data = $"Succesfully Deleted An Employee By Id: {id}";
                return Ok(APIRBuilder(true, HSC.OK, _requestBody));
            }catch (Exception ex)
            {
                return CommonExceptionHandling(ex);
            }
        }

        [HttpPost("Create")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<APIResponse>> CreateEmployeeAsync(EmployeeDTO model)
        {
            try
            {
                _logger.LogInformation($"Creating New Employee");
                if ( model == null ) return BadRequest("Invalid Model");
                var newEmployee = await _repos.CreateAsync(_mapper.Map<Employee>(model));
                model.ID = newEmployee.ID;
                _requestBody.Data = model;
                return CreatedAtRoute("GetEmployeeById", new {id = model.ID}, APIRBuilder(true, HSC.OK, _requestBody));
            }catch (Exception ex)
            {
                return CommonExceptionHandling(ex);
            }
        }

        [HttpPut("Update")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> UpdateEmployeeAsync(EmployeeDTO model)
        {
            try
            {
                _logger.LogInformation($"Updating An Employee With Id: {model.ID}");
                if ( model == null ) return BadRequest("Invalid Model");
                var employee = await _repos.GetAsync(e => e.ID == model.ID);
                if ( employee == null ) return NotFound("Couldn't Find An Employee With Such Id");
                await _repos.UpdateAsync(_mapper.Map<Employee>(model));
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
