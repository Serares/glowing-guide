using Microsoft.AspNetCore.Mvc; // to use decorators like [Route], [ApiController], etc.
using Northwind.EntityModels;
using Northwind.WebApi.Repositories;

namespace Northwind.WebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CustomersController : ControllerBase
{
    private readonly ICustomerRepository _repo;

    // constructor injects repository instance
    public CustomersController(ICustomerRepository repo)
    {
        _repo = repo;
    }

    // GET: api/customers
    [HttpGet]
    [ProducesResponseType(200, Type = typeof(IEnumerable<Customer>))]
    public async Task<IEnumerable<Customer>> GetCustomers(string? country)
    {
        if (string.IsNullOrWhiteSpace(country))
        {
            return await _repo.RetrieveAllAsync();
        }
        else
        {
            return (await _repo.RetrieveAllAsync()).Where(customer => customer.Country.ToLower() == country.ToLower());
        }
    }

    [HttpGet("{id}", Name = nameof(GetCustomer))]
    [ProducesResponseType(200, Type = typeof(Customer))]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetCustomer(string id)
    {
        Customer? c = await _repo.RetrieveAsync(id);

        if (c == null)
        {
            return NotFound(); // 404 resource not found
        }

        return Ok(c);
    }


    // POST: api/customers
    [HttpPost]
    [ProducesResponseType(201, Type = typeof(Customer))]
    [ProducesResponseType(400)]
    public async Task<IActionResult> CreateCustomer([FromBody] Customer c)
    {
        if (c == null)
        {
            return BadRequest(); // 400 bad request
        }

        Customer? added = await _repo.CreateAsync(c);

        if (added == null)
        {
            return BadRequest("Repository failed to create customer");
        }
        else
        {
            return CreatedAtRoute(
                routeName: nameof(GetCustomer),
                routeValues: new { id = added.CustomerId.ToLower() },
                value: added
            );
        }
    }

    // PUT: api/customers/id
    [HttpPut("{id}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> Update(string id, [FromBody] Customer c)
    {
        id = id.ToUpper();

        c.CustomerId = c.CustomerId.ToUpper();
        if (c == null || c.CustomerId != id)
        {
            return BadRequest(); // 400 bad request
        }

        Customer? existing = await _repo.RetrieveAsync(id);

        if (existing == null)
        {
            return NotFound();
        }

        await _repo.UpdateAsync(c);

        return new NoContentResult(); // 204 no content
    }

    // DELETE: api/customers/id
    [HttpDelete("{id}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> Delete(string id)
    {
        if (id == "bad")
        {
            ProblemDetails pd = new()
            {
                Status = StatusCodes.Status400BadRequest,
                Type = "https://localhost:5151/customers/failed-to-delete",
                Title = $"Customer delete failed id {id}",
                Detail = "More details like...",
                Instance = HttpContext.Request.Path,
            };

            return BadRequest(pd);
        }

        bool? deleted = await _repo.DeleteAsync(id);

        if (deleted.HasValue && deleted.Value) // i.e. if deleted successfully
        {
            return new NoContentResult(); // 204 no content
        }
        else if (!deleted.HasValue) // i.e. if not found
        {
            return NotFound(); // 404 not found
        }
        else
        {
            return BadRequest($"Repository failed to delete customer with id {id}");
        }
    }
}