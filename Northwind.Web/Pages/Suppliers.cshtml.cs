using Microsoft.AspNetCore.Mvc.RazorPages;
using Northwind.EntityModels;
using Microsoft.AspNetCore.Mvc; // To use [BindProperty], I ActionResult

namespace Northwind.Web.Pages;

public class SuppliersModel : PageModel
{
    [BindProperty]
    public Supplier? Supplier { get; set; }
    private NorthwindContext _db;
    public IEnumerable<Supplier>? Suppliers { get; set; }

    public SuppliersModel(NorthwindContext db)
    {
        _db = db;
    }

    public IActionResult OnPost()
    {
        if (Supplier is not null && ModelState.IsValid)
        {
            _db.Suppliers.Add(Supplier);
            _db.SaveChanges();

            return RedirectToPage("/suppliers");
        }
        else
        {
            return Page(); // return to original page;
        }
    }

    // When an HTTP GET request is made for this Razor Page, the OnGet method executes
    public void OnGet()
    {
        ViewData["Title"] = "Northwind B2B - Suppliers";

        Suppliers = _db.Suppliers.OrderBy(c => c.Country)
        .ThenBy(c => c.CompanyName);
    }
}