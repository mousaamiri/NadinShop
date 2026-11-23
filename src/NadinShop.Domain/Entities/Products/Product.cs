using NadinShop.Domain.Common;
using NadinShop.Domain.Entities.Products.ValueObjects;

namespace NadinShop.Domain.Entities.Products;

public class Product:AggregateRoot<Guid>
{
    //Properties
    public ProductName Name { get; private set; } = null!;
    public ProduceDate ProduceDate { get; private set; }
    public ManufacturePhone ManufacturePhone { get; private set; } = null!;
    public ManufactureEmail ManufactureEmail { get; private set; } = null!;
    public bool IsAvailable { get; private set; }
    
    // Constructor
    public Product() { }//For Ef
    private Product(
        ProductName name,
        ProduceDate produceDate,
        ManufacturePhone manufacturePhone,
        ManufactureEmail manufactureEmail,
        bool isAvailable) : base(Guid.NewGuid())
    {
        Name = name;
        ProduceDate = produceDate;
        ManufacturePhone = manufacturePhone;
        ManufactureEmail = manufactureEmail;
        IsAvailable = isAvailable;
    }

    // Factory method
    public static Product Create(
        string name,
        string manufacturePhone,
        string manufactureEmail,
        bool isAvailable)
    {
        return new Product(
            ProductName.Create(name),
            ProduceDate.Create(DateTime.UtcNow),
            ManufacturePhone.Create(manufacturePhone),
            ManufactureEmail.Create(manufactureEmail),
            isAvailable);
    }
    //Methods
    //TODO: 
}
