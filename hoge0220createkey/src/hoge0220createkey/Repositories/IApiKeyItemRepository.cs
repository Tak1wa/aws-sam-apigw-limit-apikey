using hoge0220createkey.Entities;

namespace hoge0220createkey.Repositories
{
    public interface IApiKeyItemRepository
    {
        Task<bool> CreateAsync(string usagePlanId);
        
        Task<bool> DeleteAsync(string apiKeyId);

        Task<IList<ApiKeyItem>> GetApiKeyItemsAsync(int limit = 10);

        Task<ApiKeyItem?> GetByApiKeyItemIdAsync(string apiKeyId);
    }
}