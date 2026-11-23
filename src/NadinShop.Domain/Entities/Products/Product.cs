using NadinShop.Domain.Common;

namespace NadinShop.Domain.Entities.Products;

public class Product():AggregateRoot<Guid>(Guid.NewGuid())
{
    //Properties
    public string Name { get; private set; } = null!;
    public DateTime ProductDate { get; private set; }
    public string ManufacturePhone { get; private set; } = null!;
    public string ManufactureEmail { get; private set; } = null!;
    public bool IsAvailable { get; private set; }
    //Constructor
    private Product(string name,string manufacturePhone,string manufactureEmail,bool isAvailable):this()
    {
        this.Name = name;
        this.ProductDate = DateTime.Now;
        this.ManufacturePhone = manufacturePhone;
        this.ManufactureEmail = manufactureEmail;
        this.IsAvailable = isAvailable;
    }
    //Factory method
    public static Product Create(string name, string manufacturePhone, string manufactureEmail, bool isAvailable)
    {

    }
    //Methods

}
