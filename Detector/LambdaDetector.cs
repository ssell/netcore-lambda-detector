using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.SQSEvents;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;

namespace NetCore3Lambda.Detector
{
    public enum LambdaSource
    {
        Unknown,
        ApiGateway,
        Sqs
    }

    /// <summary>
    /// Utility class which uses the Lambda HTTP API to retrieve the next invocation event
    /// and determines what triggered the currently executing Lambda - an API Gateway or SQS.
    /// </summary>
    public static class LambdaDetector
    {
        /// <summary>
        /// The Lambda API URL as described at https://docs.aws.amazon.com/lambda/latest/dg/runtimes-api.html
        /// </summary>
        private static readonly string NextInvocationUrl = $"http://{Environment.GetEnvironmentVariable("AWS_LAMBDA_RUNTIME_API")}/2018-06-01/runtime/invocation/next";

        /// <summary>
        /// Detects the triggering source for the currently executing Lambda.<para/>
        /// 
        /// Returns a tuple of (<see cref="APIGatewayProxyRequest"/>, <see cref="SQSEvent"/>) where
        /// the source is represented by the returned object that is not null. For example, if the <see cref="SQSEvent"/> 
        /// is not null then the Lambda was triggered by a SQS message event.
        /// </summary>
        /// <returns></returns>
        public static LambdaSource DetectSource()
        {
            APIGatewayProxyRequest api = null;
            SQSEvent sqs = null;

            var invocationEventTask = GetInvocationEvent();
            (api, sqs) = GetEventBody(invocationEventTask);

            return (api != null) ? LambdaSource.ApiGateway : LambdaSource.Sqs;
        }

        /// <summary>
        /// Retrieves the <see cref="LambdaInvocationEvent"/> associated with the currently executing Lambda.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        private static LambdaInvocationEvent GetInvocationEvent(CancellationToken token = default)
        {
            using (var client = new HttpClient())
            using (var request = new HttpRequestMessage())
            {
                request.Method = new HttpMethod("GET");
                request.Headers.Accept.Add(MediaTypeWithQualityHeaderValue.Parse("application/json"));
                request.RequestUri = new Uri(NextInvocationUrl, UriKind.RelativeOrAbsolute);

                var responseTask = client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, token);
                responseTask.Wait();

                var response = responseTask.Result;

                if ((response.StatusCode != System.Net.HttpStatusCode.OK) &&
                    (response.StatusCode != System.Net.HttpStatusCode.NoContent))
                {
                    throw new Exception($"API request '{request.RequestUri}' returned error status code {response.StatusCode}");
                }

                return LambdaInvocationEvent.Build(response);
            }
        }

        /// <summary>
        /// Attempts to deserialize the <see cref="LambdaInvocationEvent.Payload"/> into one of the expected trigger event objects.
        /// </summary>
        /// <param name="invocationEvent"></param>
        /// <returns></returns>
        private static (APIGatewayProxyRequest api, SQSEvent sqs) GetEventBody(LambdaInvocationEvent invocationEvent)
        {
            var api = JsonConvert.DeserializeObject<APIGatewayProxyRequest>(invocationEvent.Payload);
            var sqs = JsonConvert.DeserializeObject<SQSEvent>(invocationEvent.Payload);

            // The serializer will create the default object regardless of whether any of the fields deserialized.
            // So if the object is missing a key field then set it to null to indicate the JSON was not for the event.
            return ((api?.Resource == null ? null : api),
                    (sqs?.Records == null ? null : sqs));
        }
    }
}