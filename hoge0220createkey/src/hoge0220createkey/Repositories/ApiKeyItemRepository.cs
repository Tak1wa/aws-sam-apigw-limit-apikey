using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using hoge0220createkey.Entities;

using Amazon.APIGateway;
using Amazon.APIGateway.Model;

namespace hoge0220createkey.Repositories
{


    public class ApiKeyItemRepository : IApiKeyItemRepository
    {
        private readonly IDynamoDBContext context;
        private readonly ILogger<ApiKeyItemRepository> logger;

        public ApiKeyItemRepository(IDynamoDBContext context, ILogger<ApiKeyItemRepository> logger)
        {
            this.context = context;
            this.logger = logger;
        }

        public async Task<bool> CreateAsync(string usagePlanId)
        {
            try
            {
                var apigw = new AmazonAPIGatewayClient();

                var createApiKeyResult = await apigw.CreateApiKeyAsync(new CreateApiKeyRequest
                {
                    Name = Guid.NewGuid().ToString(),
                    Enabled = true
                });

                var createUsagePlanKeyResult = await apigw.CreateUsagePlanKeyAsync(new CreateUsagePlanKeyRequest
                {
                    UsagePlanId = usagePlanId,
                    KeyId = createApiKeyResult.Id,
                    KeyType = "API_KEY"
                });

                var newKey = new ApiKeyItem();
                newKey.ApiKeyId = createApiKeyResult.Id;
                newKey.ApiKeyValue = createApiKeyResult.Value;
                newKey.CreationTime = GetUnixTimeSecondsFromDateTime(DateTime.Now);
                newKey.ExpirationTime = GetUnixTimeSecondsFromDateTime(DateTime.Now.AddMinutes(5));
                await context.SaveAsync(newKey);

                logger.LogInformation("ApiKey {} is added", newKey.ApiKeyId);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "fail to persist to DynamoDb Table");
                return false;
            }

            return true;
        }

        private int GetUnixTimeSecondsFromDateTime(DateTime beforeDateTime)
        {
            var dateTimeOffset = new DateTimeOffset(beforeDateTime.ToUniversalTime());
            return (int)dateTimeOffset.ToUnixTimeSeconds();
        }

        public async Task<bool> DeleteAsync(string apiKeyId)
        {
            bool result;
            try
            {
                var apigw = new AmazonAPIGatewayClient();
                var resultDeleteApiKey = await apigw.DeleteApiKeyAsync(new DeleteApiKeyRequest
                {
                    ApiKey = apiKeyId
                });

                await context.DeleteAsync<ApiKeyItem>(apiKeyId);
                ApiKeyItem deletedApiKey = await context.LoadAsync<ApiKeyItem>(apiKeyId, new DynamoDBContextConfig
                {
                    ConsistentRead = true
                });

                result = deletedApiKey == null;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "fail to delete ApiKey from DynamoDb Table");
                result = false;
            }

            if (result) logger.LogInformation("ApiKey {apiKeyId} is deleted", apiKeyId);

            return result;
        }

        public async Task<ApiKeyItem?> GetByApiKeyItemIdAsync(string apiKeyId)
        {
            try
            {
                return await context.LoadAsync<ApiKeyItem>(apiKeyId);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "fail to update apiKey from DynamoDb Table");
                return null;
            }
        }

        public async Task<IList<ApiKeyItem>> GetApiKeyItemsAsync(int limit = 10)
        {
            var result = new List<ApiKeyItem>();

            try
            {
                if (limit <= 0)
                {
                    return result;
                }

                var filter = new ScanFilter();
                filter.AddCondition("ApiKeyId", ScanOperator.IsNotNull);
                var scanConfig = new ScanOperationConfig()
                {
                    Limit = limit,
                    Filter = filter
                };
                var queryResult = context.FromScanAsync<ApiKeyItem>(scanConfig);

                do
                {
                    result.AddRange(await queryResult.GetNextSetAsync());
                }
                while (!queryResult.IsDone && result.Count < limit);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "fail to list apiKeys from DynamoDb Table");
                return new List<ApiKeyItem>();
            }

            return result;
        }
    }
}
