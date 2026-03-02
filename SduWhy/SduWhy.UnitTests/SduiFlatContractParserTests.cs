using System.Text.Json;
using SduWhy.Enums;
using SduWhy.Parsers;
using SduWhy.UnitTests.Contracts;

namespace SduWhy.UnitTests;

public class SduiFlatContractParserTests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void ParseSduiContract_SampleProductContract_NullTags_ShouldPass()
    {
        //given
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

        //when
        var contract = new SduiFlatContractParser().GenerateContract(sampleProduct);

        //then
        Assert.That(contract["version"], Is.EqualTo("1"));
        Assert.That(contract["contractType"], Is.EqualTo(SduiContractType.Flat));

        var elements = (ICollection<Dictionary<string, object>>)contract["elements"];

        Assert.That(elements, Has.Count.EqualTo(4));
        
        Assert.That(elements, Has.Exactly(1)
            .Matches<Dictionary<string, object>>(e =>
                (string)e["key"] == "product-id" &&
                (string)e["data"] == "test#12321" &&
                (string)e["size"] == "large" &&
                (string)e["type"] == "field"));
        
        Assert.That(elements, Has.Exactly(1)
            .Matches<Dictionary<string, object>>(e =>
                (string)e["key"] == "product-price" &&
                (decimal)e["data"] == 100 &&
                (string)e["size"] == "small" &&
                (string)e["type"] == "field"));
        
        Assert.That(elements, Has.Exactly(1)
            .Matches<Dictionary<string, object>>(e =>
                (string)e["key"] == "item-id" &&
                (string)e["data"] == "1999312" &&
                (string)e["size"] == "small" &&
                (string)e["type"] == "field"));
        
        Assert.That(elements, Has.Exactly(1)
            .Matches<Dictionary<string, object>>(e =>
                (string)e["key"] == "tags" &&
                (string)e["data"] == "discount" &&
                (string)e["size"] == "small" &&
                (string)e["type"] == "chip"));
    }
    
    [Test]
    public void ParseSduiContract_SampleProductContract_WithList_ShouldPass()
    {
        //given
        var sampleProduct = new SampleProductContract
        {
            ProductId = "test#12321",
            Price = 100,
            Metadata = new SampleProductComplexItem
            {
                ItemId = "1999312",
                Tags = "discount",
                IsActive = true
            },
            Categories =
            [
                "Utilities",
                "Kitchen"
            ]
        };

        //when
        var contract = new SduiFlatContractParser().GenerateContract(sampleProduct);

        //then
        Assert.That(contract["version"], Is.EqualTo("1"));
        Assert.That(contract["contractType"], Is.EqualTo(SduiContractType.Flat));

        var elements = (ICollection<Dictionary<string, object>>)contract["elements"];

        Assert.That(elements, Has.Count.EqualTo(5));
        
        Assert.That(elements, Has.Exactly(1)
            .Matches<Dictionary<string, object>>(e =>
                (string)e["key"] == "product-id" &&
                (string)e["data"] == "test#12321" &&
                (string)e["size"] == "large" &&
                (string)e["type"] == "field"));
        
        Assert.That(elements, Has.Exactly(1)
            .Matches<Dictionary<string, object>>(e =>
                (string)e["key"] == "product-price" &&
                (decimal)e["data"] == 100 &&
                (string)e["size"] == "small" &&
                (string)e["type"] == "field"));
        
        Assert.That(elements, Has.Exactly(1)
            .Matches<Dictionary<string, object>>(e =>
                (string)e["key"] == "item-id" &&
                (string)e["data"] == "1999312" &&
                (string)e["size"] == "small" &&
                (string)e["type"] == "field"));
        
        Assert.That(elements, Has.Exactly(1)
            .Matches<Dictionary<string, object>>(e =>
                (string)e["key"] == "tags" &&
                (string)e["data"] == "discount" &&
                (string)e["size"] == "small" &&
                (string)e["type"] == "chip"));
        
        Assert.That(elements, Has.Exactly(1)
            .Matches<Dictionary<string, object>>(e =>
                (string)e["key"] == "categories" &&
                ((IEnumerable<string>)e["data"]).SequenceEqual(["Utilities", "Kitchen"]) &&
                (string)e["size"] == "large" &&
                (string)e["type"] == "table"));
    }

    [Test]
    public void ParseSduiContract_SampleProductContract_HideIfNull_ShouldPass()
    {
        //given
        var sampleProduct = new SampleProductContract
        {
            ProductId = "test#12321",
            Price = 100,
            Metadata = new SampleProductComplexItem
            {
                ItemId = "1999312",
                IsActive = true
            }
        };
        
        //when
        var contract = new SduiFlatContractParser().GenerateContract(sampleProduct);

        //then
        Assert.That(contract["version"], Is.EqualTo("1"));
        Assert.That(contract["contractType"], Is.EqualTo(SduiContractType.Flat));

        var elements = (ICollection<Dictionary<string, object>>)contract["elements"];

        Assert.That(elements, Has.Count.EqualTo(3));
        
        Assert.That(elements, Has.Exactly(1)
            .Matches<Dictionary<string, object>>(e =>
                (string)e["key"] == "product-id" &&
                (string)e["data"] == "test#12321" &&
                (string)e["size"] == "large" &&
                (string)e["type"] == "field"));
        
        Assert.That(elements, Has.Exactly(1)
            .Matches<Dictionary<string, object>>(e =>
                (string)e["key"] == "product-price" &&
                (decimal)e["data"] == 100 &&
                (string)e["size"] == "small" &&
                (string)e["type"] == "field"));
        
        Assert.That(elements, Has.Exactly(1)
            .Matches<Dictionary<string, object>>(e =>
                (string)e["key"] == "item-id" &&
                (string)e["data"] == "1999312" &&
                (string)e["size"] == "small" &&
                (string)e["type"] == "field"));
    }

    [Test]
    public void ParseSdui_ToJson_SampleProductContract_ShouldPass()
    {
        //given
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
        
        //when
        var contract = new SduiFlatContractParser().GenerateContract(sampleProduct);

        //then
        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        var actualJson = JsonSerializer.Serialize(contract, options);
        
        var expectedJson = File.ReadAllText(
            Path.Combine(TestContext.CurrentContext.TestDirectory, "TestData/ExpectedJsonPayloads", "sampleProductCategory_full.json")
        );

        using var actualDoc = JsonDocument.Parse(actualJson);
        using var expectedDoc = JsonDocument.Parse(expectedJson);

        Assert.That(
            JsonElement.DeepEquals(actualDoc.RootElement, expectedDoc.RootElement),
            Is.True
        );
    }
    
    [Test]
    public void ParseSdui_ToJson_SampleProductContract_SimpleCollection_ShouldPass()
    {
        //given
        var sampleProduct = new SampleProductContract
        {
            ProductId = "test#12321",
            Price = 100,
            Metadata = new SampleProductComplexItem
            {
                ItemId = "1999312",
                Tags = "discount",
                IsActive = true
            },
            Categories =
            [
                "Utilities",
                "Kitchen"
            ]
        };

        //when
        var contract = new SduiFlatContractParser().GenerateContract(sampleProduct);

        //then
        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        var actualJson = JsonSerializer.Serialize(contract, options);
        
        var expectedJson = File.ReadAllText(
            Path.Combine(TestContext.CurrentContext.TestDirectory, "TestData/ExpectedJsonPayloads", "sampleProductCategory_simpleCollection_full.json")
        );

        using var actualDoc = JsonDocument.Parse(actualJson);
        using var expectedDoc = JsonDocument.Parse(expectedJson);

        Assert.That(
            JsonElement.DeepEquals(actualDoc.RootElement, expectedDoc.RootElement),
            Is.True
        );
    }
    
    [Test]
    public void ParseSdui_ToJson_SampleProductContract_ComplexCollection_ShouldPass()
    {
        //given
        var sampleProduct = new SampleProductContract
        {
            ProductId = "test#12321",
            Price = 100,
            Metadata = new SampleProductComplexItem
            {
                ItemId = "1999312",
                Tags = "discount",
                IsActive = true
            },
            Categories =
            [
                "Utilities",
                "Kitchen"
            ],
            Comments = 
            [
                new SampleCollectionItem
                {
                    Row = 1,
                    Description = "Comment"
                }
            ]
        };

        //when
        var contract = new SduiFlatContractParser().GenerateContract(sampleProduct);

        //then
        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        var actualJson = JsonSerializer.Serialize(contract, options);
        
        var expectedJson = File.ReadAllText(
            Path.Combine(TestContext.CurrentContext.TestDirectory, "TestData/ExpectedJsonPayloads", "sampleProductCategory_complexCollection_full.json")
        );

        using var actualDoc = JsonDocument.Parse(actualJson);
        using var expectedDoc = JsonDocument.Parse(expectedJson);

        Assert.That(
            JsonElement.DeepEquals(actualDoc.RootElement, expectedDoc.RootElement),
            Is.True
        );
    }
}