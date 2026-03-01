using System.Text.Json;
using SduWhy.Enums;
using SduWhy.Parsers;
using SduWhy.UnitTests.Contracts;

namespace SduWhy.UnitTests;

public class SduiFullContractParserTests
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
        var contract = new SduiFullContractParser().GenerateContract(sampleProduct);

        //then
        Assert.That(contract["version"], Is.EqualTo("1"));
        Assert.That(contract["contractType"], Is.EqualTo(SduiContractType.ContractOnly));

        var elements = (List<Dictionary<string, object>>)contract["elements"];

        Assert.That(elements, Has.Count.EqualTo(4));
        
        Assert.That(elements, Has.Exactly(1)
            .Matches<Dictionary<string, object>>(e =>
                (string)e["key"] == "product-id" &&
                (string)e["value"] == "test#12321" &&
                (string)e["size"] == "large" &&
                (string)e["type"] == "field"));
        
        Assert.That(elements, Has.Exactly(1)
            .Matches<Dictionary<string, object>>(e =>
                (string)e["key"] == "product-price" &&
                (decimal)e["value"] == 100 &&
                (string)e["size"] == "small" &&
                (string)e["type"] == "field"));
        
        Assert.That(elements, Has.Exactly(1)
            .Matches<Dictionary<string, object>>(e =>
                (string)e["key"] == "item-id" &&
                (string)e["value"] == "1999312" &&
                (string)e["size"] == "small" &&
                (string)e["type"] == "field"));
        
        Assert.That(elements, Has.Exactly(1)
            .Matches<Dictionary<string, object>>(e =>
                (string)e["key"] == "tags" &&
                (string)e["value"] == "discount" &&
                (string)e["size"] == "small" &&
                (string)e["type"] == "chip"));
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
        var contract = new SduiFullContractParser().GenerateContract(sampleProduct);

        //then
        Assert.That(contract["version"], Is.EqualTo("1"));
        Assert.That(contract["contractType"], Is.EqualTo(SduiContractType.ContractOnly));

        var elements = (List<Dictionary<string, object>>)contract["elements"];

        Assert.That(elements, Has.Count.EqualTo(3));
        
        Assert.That(elements, Has.Exactly(1)
            .Matches<Dictionary<string, object>>(e =>
                (string)e["key"] == "product-id" &&
                (string)e["value"] == "test#12321" &&
                (string)e["size"] == "large" &&
                (string)e["type"] == "field"));
        
        Assert.That(elements, Has.Exactly(1)
            .Matches<Dictionary<string, object>>(e =>
                (string)e["key"] == "product-price" &&
                (decimal)e["value"] == 100 &&
                (string)e["size"] == "small" &&
                (string)e["type"] == "field"));
        
        Assert.That(elements, Has.Exactly(1)
            .Matches<Dictionary<string, object>>(e =>
                (string)e["key"] == "item-id" &&
                (string)e["value"] == "1999312" &&
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
        var contract = new SduiFullContractParser().GenerateContract(sampleProduct);

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
}