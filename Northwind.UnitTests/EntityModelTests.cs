using System.Security.Cryptography.X509Certificates;
using Northwind.EntityModels;

namespace Northwind.EntityModels;

public class EntityModelsTests
{
    [Fact]
    public void DatabaseContextTest()
    {
        using NorthwindContext db = new();

        Assert.True(db.Database.CanConnect());
    }

    [Fact]
    public void CategoryCountTest()
    {
        // Arrange
        using NorthwindContext db = new();
        // Act 
        int expected = 8;
        int actual = db.Categories.Count();
        // Assert
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void ProductId1IsChaiTest()
    {
        using NorthwindContext db = new();

        string expected = "Chai";

        Product? product = db.Products.Find(keyValues: 1);
        string actual = product?.ProductName ?? string.Empty;

        Assert.Equal(expected, actual);
    }

}
