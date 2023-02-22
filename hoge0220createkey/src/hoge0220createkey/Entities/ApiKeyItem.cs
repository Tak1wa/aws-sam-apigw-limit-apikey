using Amazon.DynamoDBv2.DataModel;

namespace hoge0220createkey.Entities;

// <summary>
/// Map the Book Class to DynamoDb Table
/// To learn more visit https://docs.aws.amazon.com/amazondynamodb/latest/developerguide/DeclarativeTagsList.html
/// </summary>
[DynamoDBTable("ApiKeyTable")]
public class ApiKeyItem
{
    ///<summary>
    /// Map c# types to DynamoDb Columns 
    /// to learn more visit https://docs.aws.amazon.com/amazondynamodb/latest/developerguide/MidLevelAPILimitations.SupportedTypes.html
    /// <summary>
    [DynamoDBHashKey] //Partition key
    public string ApiKeyId { get; set; } = string.Empty;

    [DynamoDBProperty]
    public string ApiKeyValue { get; set; } = string.Empty;

    [DynamoDBProperty]
    public int CreationTime { get; set; } = 0;

    [DynamoDBProperty]
    public int? ExpirationTime { get; set; } = 0;
}
