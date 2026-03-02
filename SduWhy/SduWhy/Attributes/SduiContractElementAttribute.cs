namespace SduWhy.Attributes;

[AttributeUsage(AttributeTargets.Property)]
public class SduiContractElementAttribute(
    string key,
    string size = default,
    string componentType = default,
    bool hideIfNull = false,
    bool preserveHierarchy = false)
    : Attribute
{
    public string Key { get; set; } = key;

    public string Size { get; set; } = size;

    public string ComponentType { get; set; } = componentType;

    public bool HideIfNull { get; set; } = hideIfNull;
    
    public bool PreserveHierarchy { get; set; } = preserveHierarchy;
}