namespace SduWhy.Enums;

public enum SduiContractType
{
    Unknown = 0,
    /// <summary>
    /// Full contract + data on the payload
    /// </summary>
    Complex = 1,
    
    /// <summary>
    /// Full contract (flatten) + data on the payload
    /// </summary>
    Flat = 2,
    
    /// <summary>
    /// SDUI contract only, data in a separate call
    /// </summary>
    ContractOnly = 3,
    
    /// <summary>
    /// Full contract with tokens for data parsing. Needs to specify token format
    /// </summary>
    Tokenized = 4
}