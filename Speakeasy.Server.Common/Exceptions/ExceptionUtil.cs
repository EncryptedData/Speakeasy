using System.Diagnostics.CodeAnalysis;

namespace Speakeasy.Server.Common.Exceptions;

public static class ExceptionUtil
{
    /// <summary>
    /// Throws an exception if the provided obj is null
    /// </summary>
    /// <param name="obj">The variable to use</param>
    /// <param name="provider">How to build the exception</param>
    /// <typeparam name="TException">The exception to throw</typeparam>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="TException">If obj is null, then throws TException</exception>
    public static void ThrowIfNull<TException>([NotNull] object? obj, Func<TException> provider)
        where TException : Exception
    {
        if (obj is null)
        {
            throw provider();
        }
    }

    /// <summary>
    /// Throws an exception if the provided obj is null
    /// </summary>
    /// <param name="obj">The variable to use</param>
    /// <typeparam name="TException">The exception to throw</typeparam>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="TException">If obj is null, then throws TException</exception>
    public static void ThrowIfNull<TException>([NotNull] object? obj)
        where TException : Exception, new()
    {
        ThrowIfNull(obj, () => new TException());
    }

    /// <summary>
    /// Throws an exception if the provided expression is false.
    /// </summary>
    /// <param name="expression">The bool to check</param>
    /// <param name="provider">The builder for the exception</param>
    /// <typeparam name="TException">The exception to throw</typeparam>
    /// <exception cref="TException">Throws an exception if the expression is false</exception>
    public static void ThrowIfFalse<TException>([DoesNotReturnIf(false)] bool expression, Func<TException> provider)
        where TException : Exception
    {
        if (!expression)
        {
            throw provider();
        }
    }

    /// <summary>
    /// Throws an exception if the provided expression is false.
    /// </summary>
    /// <param name="expression">The bool to check</param>
    /// <param name="provider">The builder for the exception</param>
    /// <typeparam name="TException">The exception to throw</typeparam>
    /// <exception cref="TException">Throws an exception if the expression is false</exception>
    public static void ThrowIfFalse<TException>([DoesNotReturnIf(false)] bool expression) 
        where TException : Exception, new()
    {
        ThrowIfFalse(expression, () => new TException());
    }

    /// <summary>
    /// Throws an exception if the provided expression is true.
    /// </summary>
    /// <param name="expression">The bool to check</param>
    /// <param name="provider">The builder for the exception</param>
    /// <typeparam name="TException">The exception to throw</typeparam>
    /// <exception cref="TException">Throws an exception if the expression is true</exception>
    public static void ThrowIfTrue<TException>([DoesNotReturnIf(true)] bool expression, Func<TException> provider)
        where TException : Exception
    {
        if (expression)
        {
            throw provider();
        }
    }

    /// <summary>
    /// Throws an exception if the provided expression is true.
    /// </summary>
    /// <param name="expression">The bool to check</param>
    /// <typeparam name="TException">The exception to throw</typeparam>
    /// <exception cref="TException">Throws an exception if the expression is true</exception>
    public static void ThrowIfTrue<TException>([DoesNotReturnIf(true)] bool expression)
        where TException : Exception, new()
    {
        ThrowIfTrue(expression, () => new TException());
    }
}