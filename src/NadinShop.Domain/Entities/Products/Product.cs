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
    private Product() { }//For Ef
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
        DateTime produceDate,
        bool isAvailable)
    {
        return new Product(
            ProductName.Create(name),
            ProduceDate.Create(produceDate),
            ManufacturePhone.Create(manufacturePhone),
            ManufactureEmail.Create(manufactureEmail),
            isAvailable);
    }

    //Methods
    public void Rename(string name)
    {
        var newName = ProductName.Create(name);
        if (Name.Equals(newName)) return;
        Name = newName;
    }

    public void ChangeManufacturePhone(string phone)
    {
        var newPhone = ManufacturePhone.Create(phone);
        if (ManufacturePhone.Equals(newPhone)) return;
        ManufacturePhone = newPhone;
    }

    public void ChangeManufactureEmail(string email)
    {
        var newEmail = ManufactureEmail.Create(email);
        if (ManufactureEmail.Equals(newEmail)) return;
        ManufactureEmail = newEmail;
    }

    public void ChangeManufactureContact(string phone, string email)
    {
        // Validate both first so the aggregate never ends up half-updated
        var newPhone = ManufacturePhone.Create(phone);
        var newEmail = ManufactureEmail.Create(email);

        ManufacturePhone = newPhone;
        ManufactureEmail = newEmail;
    }

    public void MarkAsAvailable()
    {
        if (IsAvailable) return;
        IsAvailable = true;
    }

    public void MarkAsUnavailable()
    {
        if (!IsAvailable) return;
        IsAvailable = false;
    }
}
