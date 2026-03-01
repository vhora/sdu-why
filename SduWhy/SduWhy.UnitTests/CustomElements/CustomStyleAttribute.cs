using SduWhy.Attributes;

namespace SduWhy.UnitTests.CustomElements;

public class CustomStyleAttribute : SduiContractCustomElement
{
    public string Class { get; set; }
    public string Color { get; set; }

    public CustomStyleAttribute(string @class, string color)
    {
        Class = @class;
        Color = color;
    }
}