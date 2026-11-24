using System.Reflection;

namespace NadinShop.Application;

public class ApplicationAssemblyReference
{
    public static Assembly Assembly => typeof(ApplicationAssemblyReference).Assembly;
}
