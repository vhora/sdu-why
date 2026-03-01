using System.Reflection;
using SduWhy.Attributes;

namespace SduWhy;

public static class SduiParser
{
    public static object GenerateContract<T>(T obj)
    {
        var result = new Dictionary<string, object>();

        var contractAttribute = typeof(T).GetCustomAttributes(false).OfType<SduiContractAttribute>().FirstOrDefault();

        if (contractAttribute != null)
        {
            result.Add("version", contractAttribute?.Version);
            result.Add("contractType", contractAttribute?.Type);
        }

        var elements = GetElementNodes(obj);

        result.Add("elements", elements);

        return result;
    }

    private static List<Dictionary<string, object>> GetElementNodes<T>(T obj, List<Dictionary<string, object>> elements = default)
    {
        if (elements == null)
        {
            elements = new List<Dictionary<string, object>>();
        }
        
        foreach (var elementProperty in typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public))
        {
            if (!IsScalarLike(elementProperty.PropertyType))
            {
                var value = elementProperty.GetValue(obj);
                
                return GetElementNodes(value, elements);
            }
            
            var element = GetElement(obj, elementProperty);
            
            if (element != null)
            {
                elements.Add(element);
            }
        }
        
        return elements;
    }

    private static Dictionary<string, object?>? GetElement<T>(T obj, PropertyInfo prop, Dictionary<string, object?> elementsDictionary = default)
    {
        var result = new Dictionary<string, object?>();

        var attribute = prop.GetCustomAttribute<SduiContractElementAttribute>(inherit: true);
        var customAttribute = prop.GetCustomAttribute<SduiContractCustomElement>(inherit: true);

        if (attribute is null)
        {
            return null;
        }
        
        var customElements = customAttribute?.GetType()
            ?.GetProperties(BindingFlags.Instance | BindingFlags.Public) ?? Array.Empty<PropertyInfo>();
        
        foreach (var elementProperty in customElements)
        {
            if (elementProperty.Name != "TypeId")
            {
                var value = elementProperty.GetValue(customAttribute);

                if (value != null || (value == null && !attribute.HideIfNull))
                {
                    result.Add(elementProperty.Name, elementProperty.GetValue(customAttribute));
                }
            }
        }

        result.Add("key", attribute.Key);
        result.Add("value", prop.GetValue(obj));
        result.Add("size", attribute.Size);
        result.Add("type", attribute.ComponentType);

        return result;
    }
    
    private static bool IsScalarLike(Type t)
    {
        t = Nullable.GetUnderlyingType(t) ?? t;

        if (t.IsPrimitive || t.IsEnum) return true;
        return t == typeof(string) ||
               t == typeof(decimal) ||
               t == typeof(DateTime) ||
               t == typeof(DateTimeOffset) ||
               t == typeof(TimeSpan) ||
               t == typeof(Guid);
    }
}