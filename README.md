# SDU-WHY
NOTE: WIP - Expected initial release by End of March 2026

Simple C# Server-Driven UI (SDUI) contract parser from user-defined entities to avoid manual payloads creation and mapping with flexibility and standardized structure.

SDU-WHY contract structure:
- Version
- ContractType
- Elements
  - Key
  - Value
  - Size
  - HideIfNull
  - Custom metadata (via extending attribute)

Why SDU-WHY?
- Standard payload definition
- Simple and extentable library for flexibility
- No more complex mapping between data models/DTOs and SDUI contracts
- Consistency: changing your models will automatically apply to the payload
- Versioning: Contract versioning possible to avoid breaking changes

Capabilities roadmap for v1:
-  [x] SduiContract class-level attribute for basic SDUI properties (key, value, size, contract type, version, and hideIfNull)
-  [x] Allow extending SduiElements attribute for custom metadata/style classes
-  [x] Handle primitives
-  [x] Handle complex objects
-  [ ] Handle collections
-  [ ] Implement parser factory
-  [ ] Implement contractType = full (contract + data in payload)
-  [ ] Implement contractOnly = contract without data
-  [ ] Implement tokenized contract = contract + tokenized data for dynamic parsing and flexibility in the UI
-  [ ] Optimize code and explore approach for caching contracts (compile-time models, should be feasible)
-  [ ] Implement complex versioning -> Contract version + elements version
-  [ ] Publish v1 to NuGet

How to use it:

Define the base contract using SduiContract attribute
- Version
- Contract type

Define the properties that would be part of the "elements" array in the SDUI payload output using SduiContractElement attribute
- Key: Field key, must be unique in the "elements" array
- Size: User defined sizes, no rules. Ex: medium, m, md, 2, size-md, grid-4
- Type: Any user defined type to implement in the UI. Examples: card, password, number, field, header, block
- HideIfNull: Defaulted to false. If true, the field will not be exported to the output payload

```
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
}
```

Extending custom properties for "elements" array

Define your custom metadata to extend "elements" properties from the basic built-in in the library.
- Extend the library attribute class 'SduiContractCustomElement'
- Create any properties as you wish, any class name
- Add extra attribute to the property you marked with 'SduiContractElement'
Example:

```
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
```

## How to parse the contract
Just instantiate your SDUI model class:
```
var sampleProduct = new SampleProductContract
{
    ProductId = "test#12321",
    Price = 100,
    Metadata = new SampleProductComplexItem
    {
        ItemId = "1999312",
        Tags = "discount",
        IsActive = true
    }
};
```

Expected payload:
```
{
  "version": "1",
  "contractType": 2,
  "elements": [
    {
      "key": "product-id",
      "value": "test#12321",
      "size": "large",
      "type": "field"
    },
    {
      "Class": "card-small",
      "Color": "blue",
      "key": "product-price",
      "value": 100,
      "size": "small",
      "type": "field"
    },
    {
      "key": "item-id",
      "value": "1999312",
      "size": "small",
      "type": "field"
    },
    {
      "key": "tags",
      "value": "discount",
      "size": "small",
      "type": "chip"
    }
  ]
}
```
