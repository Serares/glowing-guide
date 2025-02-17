// 💡for view models usually we use Record
using Northwind.EntityModels;

namespace Northwind.Mvc;

public record HomeIndexViewModel(int VisitorCount, IList<Category> Categories, IList<Product> Products);
