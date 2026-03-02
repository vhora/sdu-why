using System.Collections;
using System.Reflection;
using SduWhy.Attributes;

namespace SduWhy.Parsers;

public class SduiFlatContractParser : ISduiContractParser
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

        result.Add("elements", elements.Values);

        return result;
    }

    private static Dictionary<string, Dictionary<string, object>> GetElementNodes<T>(T obj, Dictionary<string, Dictionary<string, object>>? elements = null, string? parentKey = null)
    {
        elements ??= [];

        if (obj is null)
        {
            return elements;
        }
        
        foreach (var elementProperty in obj.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.Public))
        {
            var elementKey = GetElementKey(elementProperty);
            
            if (IsCollection(elementProperty.PropertyType))
            {
                var node = GetElementProperties(obj, elementProperty, elementKey);

                if (node != null)
                {
                    elements.Add(elementKey, node);
                }
                
                continue;
            }
            
            if (!IsScalarLike(elementProperty.PropertyType))
            {
                var value = elementProperty.GetValue(obj);
                
                GetElementNodes(value, elements, GetElementKey(elementProperty));
                
                continue;
            }
            
            var elementProperties = GetElementProperties(obj, elementProperty, parentKey);
            
            if (elementProperties != null)
            {
                elements.Add(elementKey, elementProperties);
            }
        }
        
        return elements;
    }

    private static Dictionary<string, object>? GetElementProperties<T>(T obj, PropertyInfo prop, string? parentKey = null)
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
        
        result.Add("key",  BuildElementKey(attribute, parentKey));
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

    private static string? GetElementKey(PropertyInfo prop)
    {
        var attribute = prop.GetCustomAttribute<SduiContractElementAttribute>(inherit: true);
        
        return attribute?.Key;
    }
    
    private static string? BuildElementKey(SduiContractElementAttribute attribute, string? parentKey = null)
    {
        if (attribute.PreserveHierarchy && parentKey != null)
        {
            return $"{parentKey}.{attribute.Key}";
        }
        
        return attribute.Key;
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