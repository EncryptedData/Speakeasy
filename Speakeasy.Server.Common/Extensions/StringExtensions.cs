namespace Speakeasy.Server.Common.Extensions;

public static class StringExtensions
{
    /// <summary>
    /// Determines whether the specified string is null or empty.
    /// </summary>
    /// <param name="str">The string to validate.</param>
    /// <returns>true if the string is null or empty; otherwise, false.</returns>
    public static bool IsNullOrEmpty(this string? str)
    {
        return string.IsNullOrEmpty(str);
    }
    
    /// <summary>
    /// Determines whether the specified string is null, empty, or consists only of white-space characters.
    /// </summary>
    /// <param name="str">The string to validate.</param>
    /// <returns>true if the string is null, empty, or contains only white-space; otherwise, false.</returns>
    public static bool IsNullOrWhiteSpace(this string? str)
    {
        return string.IsNullOrWhiteSpace(str);
    }
    
    /// <summary>
    /// Determines whether the specified string is not null and not empty.
    /// </summary>
    /// <param name="str">The string to validate.</param>
    /// <returns>true if the string is not null and not empty; otherwise, false.</returns>
    public static bool IsNotNullOrEmpty(this string? str)
    {
        return !string.IsNullOrEmpty(str);
    }
    
    /// <summary>
    /// Determines whether the specified string is not null, not empty, and not consisting only of white-space characters.
    /// </summary>
    /// <param name="str">The string to validate.</param>
    /// <returns>true if the string is not null, not empty, and contains non-white-space characters; otherwise, false.</returns>
    public static bool IsNotNullOrWhiteSpace(this string? str)
    {
        return !string.IsNullOrWhiteSpace(str);
    }
}