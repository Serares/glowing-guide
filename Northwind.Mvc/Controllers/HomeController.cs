using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Northwind.Mvc.Models;
using Microsoft.EntityFrameworkCore;
using Northwind.EntityModels;
using Microsoft.AspNetCore.Authorization;

namespace Northwind.Mvc.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly NorthwindContext _db;
    private readonly IHttpClientFactory _httpClientFactory;

    public HomeController(
        ILogger<HomeController> logger,
        NorthwindContext db,
        IHttpClientFactory httpClientFactory)
    {
        _logger = logger;
        _db = db;
        _httpClientFactory = httpClientFactory;
    }

    [ResponseCache(Duration = 10 /* seconds */, Location = ResponseCacheLocation.Any)]
    public async Task<IActionResult> Index()
    {
        HomeIndexViewModel model = new(
            VisitorCount: Random.Shared.Next(1, 1000),
            Categories: await _db.Categories.ToListAsync(),
            Products: await _db.Products.ToListAsync()
        );
        return View(model);
    }

    [Route("private")]
    [Authorize(Roles = "Administrators")]
    public IActionResult Privacy()
    {
        return View();
    }

    public IActionResult ModelBinding()
    {
        return View();
    }

    [HttpPost]
    public IActionResult ModelBinding(Thing thing)
    {
        _logger.LogInformation($"Request received: {Request.Path}");
        _logger.LogInformation($"Request parameters: {string.Join(", ", Request.Query.Select(x => $"{x.Key}={x.Value}"))}");
        HomeModelBindingViewModel model = new(
            Thing: thing,
            HasErrors: !ModelState.IsValid,
            ValidationErrors: ModelState.Values
            .SelectMany(state => state.Errors)
            .Select(error => error.ErrorMessage)
        );

        return View(model); // Show the model bound
    }

    public async Task<IActionResult> ProductDetail(int? id, string alertStyle = "success")
    {
        if (!id.HasValue)
        {
            return BadRequest($"Invalid product ID: {id}");
        }

        ViewData["alertStyle"] = alertStyle;
        Product? model = await _db.Products.Include(p => p.Category)
        .SingleOrDefaultAsync(p => p.ProductId == id);

        if (model is null)
        {
            return NotFound($"Product with ID: {id} not found");
        }

        return View(model);
    }

    public IActionResult ProductsThatCostMoreThan(decimal? price)
    {
        if (!price.HasValue)
        {
            return BadRequest("Price not provided");
        }

        IEnumerable<Product> model = _db.Products
        .Include(p => p.Category)
        .Include(p => p.Supplier)
        .Where(p => p.UnitPrice > price);


        if (!model.Any())
        {
            return NotFound($"No product costs more than {price:C}");
        }

        ViewData["MaxPrice"] = price.Value.ToString("C");

        return View(model);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    public async Task<IActionResult> Customers(string country)
    {
        string uri;

        if (string.IsNullOrEmpty(country))
        {
            ViewData["Title"] = "All Customers Worldwide";
            uri = "api/customers";
        }
        else
        {
            ViewData["Title"] = $"Customers in {country}";
            uri = $"api/customers?country={country}";
        }

        HttpClient client = _httpClientFactory.CreateClient(name: "Northwind.WebApi");

        HttpRequestMessage request = new(
            method: HttpMethod.Get, requestUri: uri
        );

        HttpResponseMessage response = await client.SendAsync(request);

        IEnumerable<Customer>? model = await response.Content.ReadFromJsonAsync<IEnumerable<Customer>>();

        return View(model);
    }
}
