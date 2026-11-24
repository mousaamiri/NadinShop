using System.Reflection;

namespace NadinShop.Application;

internal class ApplicationAssemblyReference
{
    internal static Assembly Assembly => typeof(ApplicationAssemblyReference).Assembly;
}
