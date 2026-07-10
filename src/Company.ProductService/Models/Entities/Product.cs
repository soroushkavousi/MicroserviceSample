namespace Company.ProductService.Models.Entities;

public class Product
{
    private Product() { }

    public Product(long id, string name, double price, string description)
    {
        Id = id;
        Name = name;
        Price = price;
        Description = description;
    }

    public long Id { get; }
    public string Name { get; private set; }
    public double Price { get; private set; }
    public string Description { get; private set; }

    public void Modify(string name, double price, string description)
    {
        Name = name;
        Price = price;
        Description = description;
    }
}