using System.Diagnostics.CodeAnalysis;

namespace Speakeasy.Server.Common.Exceptions;

public static class ExceptionUtil
{
    /// <summary>
    /// Throws an exception if the provided obj is null
    /// </summary>
    /// <param name="obj">The variable to tuse</param>
    /// <param name="provider">How to build the exception</param>
    /// <typeparam name="TException">The exception to throw</typeparam>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="TException">If obj is null, then throws TException</exception>
    public static void ThrowIfNull<TException>([NotNull] object? obj, Func<TException> provider)
        where TException : Exception
    {
        ArgumentNullException.ThrowIfNull(provider);
        
        if (obj is null)
        {
            throw provider();
        }
    }

    public static void ThrowIfNull<TException>([NotNull] object? obj)
        where TException : Exception, new()
    {
        ThrowIfNull(obj, () => new TException());
    }
}