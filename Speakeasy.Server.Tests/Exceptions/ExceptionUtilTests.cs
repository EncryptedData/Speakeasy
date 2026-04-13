using Speakeasy.Server.Common.Exceptions;

namespace Speakeasy.Server.Tests.Exceptions;

[TestFixture]
[UnitTestCategory]
public class ExceptionUtilTests
{
    [Test]
    public void ThrowIfNull_WithNullObject_ThrowsException()
    {
        object? nullObj = null;

        var act = () => ExceptionUtil.ThrowIfNull(nullObj, () => new InvalidOperationException("Test error"));

        act.Should().Throw<InvalidOperationException>().WithMessage("Test error");
    }

    [Test]
    public void ThrowIfNull_WithNonNullObject_DoesNotThrow()
    {
        var obj = new object();

        var act = () => ExceptionUtil.ThrowIfNull(obj, () => new InvalidOperationException("Test error"));

        act.Should().NotThrow();
    }

    [Test]
    public void ThrowIfNull_WithNullObjectAndNoProvider_ThrowsException()
    {
        object? nullObj = null;

        var act = () => ExceptionUtil.ThrowIfNull<CustomTestException>(nullObj);

        act.Should().Throw<CustomTestException>();
    }

    [Test]
    public void ThrowIfNull_WithNonNullObjectAndNoProvider_DoesNotThrow()
    {
        var obj = new object();

        var act = () => ExceptionUtil.ThrowIfNull<CustomTestException>(obj);

        act.Should().NotThrow();
    }

    [Test]
    public void ThrowIfNull_WithNullString_ThrowsException()
    {
        string? nullString = null;

        var act = () => ExceptionUtil.ThrowIfNull(nullString, () => new ArgumentNullException(nameof(nullString)));

        act.Should().Throw<ArgumentNullException>();
    }

    [Test]
    public void ThrowIfNull_WithEmptyString_DoesNotThrow()
    {
        var emptyString = string.Empty;

        var act = () => ExceptionUtil.ThrowIfNull<ArgumentNullException>(emptyString);

        act.Should().NotThrow();
    }

    [Test]
    public void ThrowIfNull_WithDifferentExceptionTypes_ThrowsCorrectType()
    {
        object? nullObj = null;

        var act = () => ExceptionUtil.ThrowIfNull<NotImplementedException>(nullObj);

        act.Should().Throw<NotImplementedException>();
    }

    [Test]
    public void ThrowIfFalse_WithFalseExpression_ThrowsException()
    {
        var expression = false;
        
        var act = () => ExceptionUtil.ThrowIfFalse(expression, () => new InvalidOperationException("Condition failed"));

        act.Should().Throw<InvalidOperationException>().WithMessage("Condition failed");
    }

    [Test]
    public void ThrowIfFalse_WithTrueExpression_DoesNotThrow()
    {
        var expression = true;
        
        var act = () => ExceptionUtil.ThrowIfFalse(expression, () => new InvalidOperationException("Condition failed"));

        act.Should().NotThrow();
    }

    [Test]
    public void ThrowIfFalse_WithFalseExpressionAndNoProvider_ThrowsException()
    {
        var expression = false;
        
        var act = () => ExceptionUtil.ThrowIfFalse<CustomTestException>(expression);

        act.Should().Throw<CustomTestException>();
    }

    [Test]
    public void ThrowIfFalse_WithTrueExpressionAndNoProvider_DoesNotThrow()
    {
        var expression = true;
        
        var act = () => ExceptionUtil.ThrowIfFalse<CustomTestException>(expression);

        act.Should().NotThrow();
    }

    [Test]
    public void ThrowIfFalse_WithComplexCondition_ThrowsWhenConditionIsFalse()
    {
        var value = 5;
        
        var act = () =>
            ExceptionUtil.ThrowIfFalse(value > 10, () => new ArgumentException("Value must be greater than 10"));

        act.Should().Throw<ArgumentException>().WithMessage("Value must be greater than 10");
    }

    [Test]
    public void ThrowIfFalse_WithComplexCondition_DoesNotThrowWhenConditionIsTrue()
    {
        var value = 15;
        
        var act = () => ExceptionUtil.ThrowIfFalse<ArgumentException>(value > 10);

        act.Should().NotThrow();
    }

    [Test]
    public void ThrowIfTrue_WithTrueExpression_ThrowsException()
    {
        var expression = true;

        var act = () => ExceptionUtil.ThrowIfTrue(expression, () => new InvalidOperationException("Condition met"));

        act.Should().Throw<InvalidOperationException>().WithMessage("Condition met");
    }

    [Test]
    public void ThrowIfTrue_WithFalseExpression_DoesNotThrow()
    {
        var expression = false;

        var act = () => ExceptionUtil.ThrowIfTrue(expression, () => new InvalidOperationException("Condition met"));

        act.Should().NotThrow();
    }

    [Test]
    public void ThrowIfTrue_WithTrueExpressionAndNoProvider_ThrowsException()
    {
        var expression = true;
        
        var act = () => ExceptionUtil.ThrowIfTrue<CustomTestException>(expression);

        act.Should().Throw<CustomTestException>();
    }

    [Test]
    public void ThrowIfTrue_WithFalseExpressionAndNoProvider_DoesNotThrow()
    {
        var expression = false;
        
        var act = () => ExceptionUtil.ThrowIfTrue<CustomTestException>(expression);

        act.Should().NotThrow();
    }

    [Test]
    public void ThrowIfTrue_WithComplexCondition_ThrowsWhenConditionIsTrue()
    {
        var value = -5;
        
        var act = () => ExceptionUtil.ThrowIfTrue(value < 0, () => new ArgumentException("Value must not be negative"));

        act.Should().Throw<ArgumentException>().WithMessage("Value must not be negative");
    }

    [Test]
    public void ThrowIfTrue_WithComplexCondition_DoesNotThrowWhenConditionIsFalse()
    {
        var value = 5;
        
        var act = () => ExceptionUtil.ThrowIfTrue<ArgumentException>(value < 0);

        act.Should().NotThrow();
    }

    [Test]
    public void ThrowIfNull_WithZeroAsObject_DoesNotThrow()
    {
        object? zeroBoxed = 0;
        
        var act = () => ExceptionUtil.ThrowIfNull<InvalidOperationException>(zeroBoxed);

        act.Should().NotThrow();
    }

    [Test]
    public void ThrowIfNull_WithFalseAsObject_DoesNotThrow()
    {
        object? falseBoxed = false;
        
        var act = () => ExceptionUtil.ThrowIfNull<InvalidOperationException>(falseBoxed);

        act.Should().NotThrow();
    }

    [Test]
    public void ThrowIfFalse_WithZeroAsExpression_ThrowsException()
    {
        var zeroValue = 0;
        
        var act = () =>
            ExceptionUtil.ThrowIfFalse(zeroValue != 0, () => new InvalidOperationException("Value is zero"));

        act.Should().Throw<InvalidOperationException>().WithMessage("Value is zero");
    }

    [Test]
    public void ExceptionProvider_IsCalledOnlyWhenNeeded()
    {
        var providerCalled = false;
        
        ExceptionUtil.ThrowIfNull(new object(), () =>
        {
            providerCalled = true;
            return new CustomTestException();
        });
        
        providerCalled.Should().BeFalse();
    }

    [Test]
    public void ExceptionProvider_IsCalledWhenConditionMetForThrowIfNull()
    {
        var providerCalled = false;
        object? nullObj = null;


        var act = () => ExceptionUtil.ThrowIfNull(nullObj, () =>
        {
            providerCalled = true;
            return new CustomTestException();
        });

        act.Should().Throw<CustomTestException>();
        providerCalled.Should().BeTrue();
    }
}

/// <summary>
///     Custom exception for testing purposes
/// </summary>
internal class CustomTestException : Exception
{
    public CustomTestException() : base("This is a test exception")
    {
    }
}