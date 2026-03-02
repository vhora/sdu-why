using SduWhy.Attributes;
using SduWhy.Enums;
using SduWhy.UnitTests.CustomElements;

namespace SduWhy.UnitTests.Contracts;

[SduiContract("1", SduiContractType.Flat)]
public class SampleProductContract
{
    [SduiContractElement("product-id", "large", "field")]
    public string ProductId { get; set; }

    [SduiContractElement("product-price", "small", "field")]
    [CustomStyle("card-small", "blue")]
    public decimal Price { get; set; }

    [SduiContractElement("metadata", "small", "field", true, false)]
    public SampleProductComplexItem Metadata { get; set; }
    
    [SduiContractElement("categories", "large", "table", true)]
    public List<string> Categories { get; set; }
    
    [SduiContractElement("comments", "large", "table", true)]
    public List<SampleCollectionItem> Comments { get; set; }
}

public class SampleProductComplexItem
{
    [SduiContractElement("item-id", "small", "field")]
    public string ItemId { get; set; }
    
    [SduiContractElement("tags", "small", "chip", true)]
    public string Tags { get; set; }

    public bool IsActive { get; set; }
}

public class SampleCollectionItem
{
    public int Row { get; set; }

    public string Description { get; set; }
}