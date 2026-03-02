using System.Collections;
using System.Reflection;
using SduWhy.Attributes;

namespace SduWhy.Parsers;

public class SduiFullContractParser : ISduiContractParser
{
    public Dictionary<string, object> GenerateContract<T>(T obj)
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
        elements ??= [];

        if (obj is null)
        {
            return elements;
        }
        
        foreach (var elementProperty in obj.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.Public))
        {
            if (IsCollection(elementProperty.PropertyType))
            {
                var node = GetElementProperties(obj, elementProperty);

                if (node != null)
                {
                    elements.Add(node);
                }
                
                continue;
            }
            
            if (!IsScalarLike(elementProperty.PropertyType))
            {
                var value = elementProperty.GetValue(obj);
                
                GetElementNodes(value, elements);
                
                continue;
            }
            
            var elementProperties = GetElementProperties(obj, elementProperty);
            
            if (elementProperties != null)
            {
                elements.Add(elementProperties);
            }
        }
        
        return elements;
    }

    private static Dictionary<string, object>? GetElementProperties<T>(T obj, PropertyInfo prop)
    {
        var result = new Dictionary<string, object?>();

        var attribute = prop.GetCustomAttribute<SduiContractElementAttribute>(inherit: true);
        var customAttribute = prop.GetCustomAttribute<SduiContractCustomElement>(inherit: true);

        if (attribute is null)
        {
            return null;
        }
        
        var value = prop.GetValue(obj);
        
        if (value == null && attribute.HideIfNull)
        {
            return null;
        }
        
        var customElements = customAttribute?.GetType()
            ?.GetProperties(BindingFlags.Instance | BindingFlags.Public) ?? [];
        
        result.Add("key", attribute.Key);
        result.Add("data", prop.GetValue(obj));
        result.Add("size", attribute.Size);
        result.Add("type", attribute.ComponentType);
        
        foreach (var elementProperty in customElements)
        {
            if (IsScalarLike(elementProperty.PropertyType))
            {
                result.Add(elementProperty.Name, elementProperty.GetValue(customAttribute));
            }
        }

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
    
    private static bool IsCollection(Type type)
    {
        type = Nullable.GetUnderlyingType(type) ?? type;

        return type != typeof(string) && typeof(IEnumerable).IsAssignableFrom(type);
    }
}