using Microsoft.AspNetCore.Mvc.RazorPages;
using Northwind.EntityModels;

namespace Northwind.Web.Pages;

public class SuppliersModel : PageModel
{
    private NorthwindContext _db;
    public IEnumerable<Supplier>? Suppliers { get; set; }

    public SuppliersModel(NorthwindContext db)
    {
        _db = db;
    }

    // When an HTTP GET request is made for this Razor Page, the OnGet method executes
    public void OnGet()
    {
        ViewData["Title"] = "Northwind B2B - Suppliers";

        Suppliers = _db.Suppliers.OrderBy(c => c.Country)
        .ThenBy(c => c.CompanyName);
    }
}