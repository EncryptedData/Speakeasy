namespace Speakeasy.Server.Tests.Attributes;

public class UnitTestCategoryAttribute : CategoryAttribute
{
    public UnitTestCategoryAttribute() : base("UnitTest")
    {
    }
}