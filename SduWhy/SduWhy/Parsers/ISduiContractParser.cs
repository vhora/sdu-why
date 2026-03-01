namespace SduWhy.Parsers;

public interface ISduiContractParser
{
    public Dictionary<string, object> GenerateContract<T>(T obj);
}