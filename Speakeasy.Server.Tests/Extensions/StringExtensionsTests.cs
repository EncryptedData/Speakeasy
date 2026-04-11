using Speakeasy.Server.Common.Extensions;

namespace Speakeasy.Server.Tests.Extensions;

[TestFixture]
[UnitTestCategory]
public class StringExtensionsTests
{
    [Test]
    public void IsNullOrEmpty_WithNullString_ReturnsTrue()
    {
        string? nullString = null;
        var result = nullString.IsNullOrEmpty();
        result.Should().BeTrue();
    }

    [Test]
    public void IsNullOrEmpty_WithEmptyString_ReturnsTrue()
    {
        string emptyString = string.Empty;
        var result = emptyString.IsNullOrEmpty();
        result.Should().BeTrue();
    }

    [Test]
    public void IsNullOrEmpty_WithWhitespaceString_ReturnsFalse()
    {
        string whitespaceString = "   ";
        var result = whitespaceString.IsNullOrEmpty();
        result.Should().BeFalse();
    }

    [Test]
    public void IsNullOrEmpty_WithNormalString_ReturnsFalse()
    {
        string normalString = "Hello World";
        var result = normalString.IsNullOrEmpty();
        result.Should().BeFalse();
    }

    [Test]
    public void IsNullOrEmpty_WithSingleCharacter_ReturnsFalse()
    {
        string singleChar = "A";
        var result = singleChar.IsNullOrEmpty();
        result.Should().BeFalse();
    }

    [Test]
    public void IsNullOrWhiteSpace_WithNullString_ReturnsTrue()
    {
        string? nullString = null;
        var result = nullString.IsNullOrWhiteSpace();
        result.Should().BeTrue();
    }

    [Test]
    public void IsNullOrWhiteSpace_WithEmptyString_ReturnsTrue()
    {
        string emptyString = string.Empty;
        var result = emptyString.IsNullOrWhiteSpace();
        result.Should().BeTrue();
    }

    [Test]
    public void IsNullOrWhiteSpace_WithSpaces_ReturnsTrue()
    {
        string spacesString = "   ";
        var result = spacesString.IsNullOrWhiteSpace();
        result.Should().BeTrue();
    }

    [Test]
    public void IsNullOrWhiteSpace_WithTabs_ReturnsTrue()
    {
        string tabsString = "\t\t\t";
        var result = tabsString.IsNullOrWhiteSpace();
        result.Should().BeTrue();
    }

    [Test]
    public void IsNullOrWhiteSpace_WithNewlines_ReturnsTrue()
    {
        string newlineString = "\n\n\n";
        var result = newlineString.IsNullOrWhiteSpace();
        result.Should().BeTrue();
    }

    [Test]
    public void IsNullOrWhiteSpace_WithMixedWhitespace_ReturnsTrue()
    {
        string mixedWhitespace = " \t\n ";
        var result = mixedWhitespace.IsNullOrWhiteSpace();
        result.Should().BeTrue();
    }

    [Test]
    public void IsNullOrWhiteSpace_WithNormalString_ReturnsFalse()
    {
        string normalString = "Hello World";
        var result = normalString.IsNullOrWhiteSpace();
        result.Should().BeFalse();
    }

    [Test]
    public void IsNullOrWhiteSpace_WithStringContainingWhitespace_ReturnsFalse()
    {
        string stringWithWhitespace = "  Hello  ";
        var result = stringWithWhitespace.IsNullOrWhiteSpace();
        result.Should().BeFalse();
    }

    [Test]
    public void IsNullOrWhiteSpace_WithSingleCharacter_ReturnsFalse()
    {
        string singleChar = "X";
        var result = singleChar.IsNullOrWhiteSpace();
        result.Should().BeFalse();
    }

    [Test]
    public void IsNotNullOrEmpty_WithNullString_ReturnsFalse()
    {
        string? nullString = null;
        var result = nullString.IsNotNullOrEmpty();
        result.Should().BeFalse();
    }

    [Test]
    public void IsNotNullOrEmpty_WithEmptyString_ReturnsFalse()
    {
        string emptyString = string.Empty;
        var result = emptyString.IsNotNullOrEmpty();
        result.Should().BeFalse();
    }

    [Test]
    public void IsNotNullOrEmpty_WithWhitespaceString_ReturnsTrue()
    {
        string whitespaceString = "   ";
        var result = whitespaceString.IsNotNullOrEmpty();
        result.Should().BeTrue();
    }

    [Test]
    public void IsNotNullOrEmpty_WithNormalString_ReturnsTrue()
    {
        string normalString = "Hello World";
        var result = normalString.IsNotNullOrEmpty();
        result.Should().BeTrue();
    }

    [Test]
    public void IsNotNullOrEmpty_WithSingleCharacter_ReturnsTrue()
    {
        string singleChar = "A";
        var result = singleChar.IsNotNullOrEmpty();
        result.Should().BeTrue();
    }

    [Test]
    public void IsNotNullOrWhiteSpace_WithNullString_ReturnsFalse()
    {
        string? nullString = null;
        var result = nullString.IsNotNullOrWhiteSpace();
        result.Should().BeFalse();
    }

    [Test]
    public void IsNotNullOrWhiteSpace_WithEmptyString_ReturnsFalse()
    {
        string emptyString = string.Empty;
        var result = emptyString.IsNotNullOrWhiteSpace();
        result.Should().BeFalse();
    }

    [Test]
    public void IsNotNullOrWhiteSpace_WithSpaces_ReturnsFalse()
    {
        string spacesString = "   ";
        var result = spacesString.IsNotNullOrWhiteSpace();
        result.Should().BeFalse();
    }

    [Test]
    public void IsNotNullOrWhiteSpace_WithTabs_ReturnsFalse()
    {
        string tabsString = "\t\t\t";
        var result = tabsString.IsNotNullOrWhiteSpace();
        result.Should().BeFalse();
    }

    [Test]
    public void IsNotNullOrWhiteSpace_WithNewlines_ReturnsFalse()
    {
        string newlineString = "\n\n\n";
        var result = newlineString.IsNotNullOrWhiteSpace();
        result.Should().BeFalse();
    }

    [Test]
    public void IsNotNullOrWhiteSpace_WithMixedWhitespace_ReturnsFalse()
    {
        string mixedWhitespace = " \t\n ";
        var result = mixedWhitespace.IsNotNullOrWhiteSpace();
        result.Should().BeFalse();
    }

    [Test]
    public void IsNotNullOrWhiteSpace_WithNormalString_ReturnsTrue()
    {
        string normalString = "Hello World";
        var result = normalString.IsNotNullOrWhiteSpace();
        result.Should().BeTrue();
    }

    [Test]
    public void IsNotNullOrWhiteSpace_WithStringContainingWhitespace_ReturnsTrue()
    {
        string stringWithWhitespace = "  Hello  ";
        var result = stringWithWhitespace.IsNotNullOrWhiteSpace();
        result.Should().BeTrue();
    }

    [Test]
    public void IsNotNullOrWhiteSpace_WithSingleCharacter_ReturnsTrue()
    {
        string singleChar = "X";
        var result = singleChar.IsNotNullOrWhiteSpace();
        result.Should().BeTrue();
    }
    
    [TestCase(null, true, true)]
    [TestCase("", true, true)]
    [TestCase("   ", false, true)]
    [TestCase("Hello", false, false)]
    public void StringExtensions_AllMethods_WorkCorrectly(string? input, bool expectedIsNullOrEmpty, bool expectedIsNullOrWhiteSpace)
    {
        var isNullOrEmpty = input.IsNullOrEmpty();
        var isNullOrWhiteSpace = input.IsNullOrWhiteSpace();
        var isNotNullOrEmpty = input.IsNotNullOrEmpty();
        var isNotNullOrWhiteSpace = input.IsNotNullOrWhiteSpace();

        isNullOrEmpty.Should().Be(expectedIsNullOrEmpty);
        isNullOrWhiteSpace.Should().Be(expectedIsNullOrWhiteSpace);
        isNotNullOrEmpty.Should().Be(!expectedIsNullOrEmpty);
        isNotNullOrWhiteSpace.Should().Be(!expectedIsNullOrWhiteSpace);
    }

    [TestCase(null)]
    [TestCase("")]
    [TestCase("   ")]
    [TestCase("Hello")]
    public void IsNotNullOrEmpty_IsInverseOf_IsNullOrEmpty(string? testCase)
    {
        var isNullOrEmpty = testCase.IsNullOrEmpty();
        var isNotNullOrEmpty = testCase.IsNotNullOrEmpty();

        isNullOrEmpty.Should().NotBe(isNotNullOrEmpty);
    }

    [TestCase(null)]
    [TestCase("")]
    [TestCase("   ")]
    [TestCase("Hello")]
    public void IsNotNullOrWhiteSpace_IsInverseOf_IsNullOrWhiteSpace(string? testCase)
    {
        var isNullOrWhiteSpace = testCase.IsNullOrWhiteSpace();
        var isNotNullOrWhiteSpace = testCase.IsNotNullOrWhiteSpace();

        isNullOrWhiteSpace.Should().NotBe(isNotNullOrWhiteSpace);
    }
}


