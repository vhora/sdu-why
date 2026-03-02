using SduWhy.Attributes;
using SduWhy.Enums;
using SduWhy.UnitTests.CustomElements;

namespace SduWhy.UnitTests.Contracts;

[SduiContract("1", SduiContractType.ContractOnly)]
public class SampleProductContract
{
    [SduiContractElement("product-id", "large", "field")]
    public string ProductId { get; set; }

    [SduiContractElement("product-price", "small", "field")]
    [CustomStyle("card-small", "blue")]
    public decimal Price { get; set; }

    [SduiContractElement("product-price", "small", "field", true)]
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
    [SduiContractElement("row", null, "table-row")]
    public int Row { get; set; }

    [SduiContractElement("description", null, "table-row", true)]
    public string Description { get; set; }
}