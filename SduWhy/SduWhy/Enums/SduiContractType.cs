namespace SduWhy.Enums;

public enum SduiContractType
{
    Unknown = 0,
    /// <summary>
    /// Full contract + data on the payload
    /// </summary>
    Full = 1,
    
    /// <summary>
    /// SDUI contract only, data in a separate call
    /// </summary>
    ContractOnly = 2,
    
    /// <summary>
    /// Full contract with tokens for data parsing. Needs to specify token format
    /// </summary>
    Tokenized = 3
}