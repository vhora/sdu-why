using SduWhy.Enums;

namespace SduWhy.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class SduiContractAttribute : Attribute
{
    public string Version { get; set; }

    public SduiContractType Type { get; set; }
    
    public SduiContractAttribute(string version, SduiContractType type)
    {
        Version = version;
        Type = type;
    }
}