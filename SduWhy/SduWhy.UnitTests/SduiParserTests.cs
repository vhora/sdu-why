using System.Text.Json;
using SduWhy.UnitTests.Contracts;

namespace SduWhy.UnitTests;

public class SduiParserTests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void ParseSduiContract_SampleProductContract_ShouldPass()
    {
        var sampleProduct = new SampleProductContract
        {
            ProductId = "test#12321",
            Price = 100,
            Metadata = new SampleProductComplexItem
            {
                ItemId = "1999312",
                IsActive =  true
            }
        };

        var contract = SduiParser.GenerateContract(sampleProduct);
        
        var jsonContract = JsonSerializer.Serialize(contract, new JsonSerializerOptions()
        {
            PropertyNamingPolicy =  JsonNamingPolicy.CamelCase
        });
        
        Assert.Pass();
    }
}