using System;
using System.IO;
using System.Text;

using Amazon.Lambda.Core;
using Amazon.Lambda.DynamoDBEvents;

using Amazon.APIGateway;
using Amazon.APIGateway.Model;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace hoge0220removekey
{
    public class Function
    {   
        /// <summary>
        /// A simple function that takes a string and does a ToUpper
        /// </summary>
        /// <param name="input"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task FunctionHandler(DynamoDBEvent dynamoEvent, ILambdaContext context)
        {
            Console.WriteLine($"Beginning to process {dynamoEvent.Records.Count} records...");

            foreach (var record in dynamoEvent.Records)
            {
                var apiKeyId = record.Dynamodb.Keys["ApiKeyId"].S;
                Console.WriteLine($"ApiKeyId: {apiKeyId}");

                var apigw = new AmazonAPIGatewayClient();
                var resultDeleteApiKey = await apigw.DeleteApiKeyAsync(new DeleteApiKeyRequest
                {
                    ApiKey = apiKeyId
                });

                Console.WriteLine($"DeleteApiKey ({apiKeyId}) From API Gateway Result: {resultDeleteApiKey.HttpStatusCode}");
            }

            Console.WriteLine("Stream processing complete.");

            return;
        }
    }
}
