using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Northwind.Mvc.Models;
using Microsoft.EntityFrameworkCore;
using Northwind.EntityModels;

namespace Northwind.Mvc.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly NorthwindContext _db;

    public HomeController(ILogger<HomeController> logger, NorthwindContext db)
    {
        _logger = logger;
        _db = db;
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
}
