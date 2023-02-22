using Amazon.DynamoDBv2.DataModel;
using Microsoft.AspNetCore.Mvc;
using hoge0220createkey.Entities;
using hoge0220createkey.Repositories;

namespace hoge0220createkey.Controllers;

[Route("api/[controller]")]
[Produces("application/json")]
public class ApiKeyItemsController : ControllerBase
{
    private readonly ILogger<ApiKeyItemsController> logger;
    private readonly IApiKeyItemRepository apiKeyRepository;

    public ApiKeyItemsController(ILogger<ApiKeyItemsController> logger, IApiKeyItemRepository apiKeyRepository)
    {
        this.logger = logger;
        this.apiKeyRepository = apiKeyRepository;
    }

    // GET api/apiKeys
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ApiKeyItem>>> Get([FromQuery] int limit = 10)
    {
        if (limit <= 0 || limit > 100) return BadRequest("The limit should been between [1-100]");

        return Ok(await apiKeyRepository.GetApiKeyItemsAsync(limit));
    }

    // GET api/apiKeys/5
    [HttpGet("{apiKeyId}")]
    public async Task<ActionResult<ApiKeyItem>> Get(string apiKeyId)
    {
        var result = await apiKeyRepository.GetByApiKeyItemIdAsync(apiKeyId);

        if (result == null)
        {
            return NotFound();
        }

        return Ok(result);
    }

    // POST api/apiKeys
    [HttpPost("{usagePlanId}")]
    public async Task<IActionResult> Post(string usagePlanId)
    {
        var result = await apiKeyRepository.CreateAsync(usagePlanId);

        if (result)
        {
            return Ok(result);
        }
        else
        {
            return BadRequest("Fail to persist");
        }

    }

    // DELETE api/apiKeys/5
    [HttpDelete("{apiKeyId}")]
    public async Task<IActionResult> Delete(string apiKeyId)
    {
        if (apiKeyId == string.Empty) return ValidationProblem("Invalid request payload");

        var apiKeyRetrieved = await apiKeyRepository.GetByApiKeyItemIdAsync(apiKeyId);

        if (apiKeyRetrieved == null)
        {
            var errorMsg = $"Invalid input! No apiKey found with id:{apiKeyId}";
            return NotFound(errorMsg);
        }

        await apiKeyRepository.DeleteAsync(apiKeyId);
        return Ok();
    }
}
