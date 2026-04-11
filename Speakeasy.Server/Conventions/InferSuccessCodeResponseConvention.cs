using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace Speakeasy.Server.Conventions;

/// <summary>
/// Convention which automatically adds 200 response with return type to any controller endpoint
/// which does not have an explicitly defined [ProducesResponseType]
/// </summary>
public class InferSuccessCodeResponseConvention : IActionModelConvention
{
    public void Apply(ActionModel action)
    {
        
        var producesAttributes = action.Filters
            .Concat(action.Controller.Filters)
            .OfType<ProducesResponseTypeAttribute>().ToList();

        // If there are 0 attributes, we're using the default behavior which already does this, so skip
        // Otherwise, don't overwrite an explicit definition
        if (producesAttributes.Count == 0
            || producesAttributes.Any(a => a.StatusCode is > 200 and < 300))
        {
            return;
        }

        // Try to get the response type from ActionResult<T>
        var type = Unwrap(action.ActionMethod.ReturnType);
        if (type is null)
        {
            return;
        }

        action.Filters.Add(new ProducesResponseTypeAttribute(type, 200));
    }

    private static Type? Unwrap(Type t)
    {
        // Task<ActionResult<T>>
        if (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Task<>))
        {
            t = t.GetGenericArguments()[0];
        }

        // ActionResult<T>
        if (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(ActionResult<>))
        {
            t = t.GetGenericArguments()[0];
        }

        if (!typeof(IActionResult).IsAssignableFrom(t) && t != typeof(void) && t != typeof(Task))
            return t;

        return null;
    }
}