using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.SQSEvents;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;

namespace NetCore3Lambda.Detector
{
    /// <summary>
    /// Representation of the Lambda invocation event returned by the Lambda HTTP API.
    /// </summary>
    public sealed class LambdaInvocationEvent
    {
        /// <summary>
        /// The invocation's payload. This is typically the JSON serialized event object such
        /// as a <see cref="APIGatewayProxyRequest"/> or <see cref="SQSEvent"/>.
        /// </summary>
        public string Payload { get; private set; }

        /// <summary>
        /// The request ID, which identifies the request that triggered the function invocation.
        /// For example, <c>8476a536-e9f4-11e8-9739-2dfe598c3fcd</c>.
        /// </summary>
        public string AwsRequestId { get; private set; }

        /// <summary>
        /// The date that the function times out in Unix time milliseconds. 
        /// For example, <c>1542409706888</c>.
        /// </summary>
        public string DeadlineTimestamp { get; private set; }

        /// <summary>
        /// The ARN of the Lambda function, version, or alias that's specified in the invocation.
        /// For example, <c>arn:aws:lambda:us-east-2:123456789012:function:custom-runtime</c>.
        /// </summary>
        public string InvokedFunctionArn { get; private set; }

        /// <summary>
        /// The AWS X-Ray tracing header.
        /// For example, <c>Root=1-5bef4de7-ad49b0e87f6ef6c87fc2e700;Parent=9a9197af755a6419;Sampled=1</c>.
        /// </summary>
        public string TraceId { get; private set; }

        /// <summary>
        /// For invocations from the AWS Mobile SDK, data about the client application and device.
        /// </summary>
        public string ClientContext { get; private set; }

        /// <summary>
        /// For invocations from the AWS Mobile SDK, data about the Amazon Cognito identity provider.
        /// </summary>
        public string CognitoIdentity { get; private set; }

        /// <summary>
        /// Builds the <see cref="LambdaInvocationEvent"/> from a <see cref="InvocationUrls.NextInvocation"/> response.
        /// </summary>
        /// <param name="response"></param>
        public static LambdaInvocationEvent Build(HttpResponseMessage response)
        {
            LambdaInvocationEvent evnt = new LambdaInvocationEvent();

            var responseContentTask = response.Content.ReadAsStringAsync();
            responseContentTask.Wait();

            evnt.Payload = responseContentTask.Result;
            evnt.AwsRequestId = GetHeaderValue(response.Headers, "Lambda-Runtime-Aws-Request-Id");
            evnt.DeadlineTimestamp = GetHeaderValue(response.Headers, "Lambda-Runtime-Deadline-Ms");
            evnt.InvokedFunctionArn = GetHeaderValue(response.Headers, "Lambda-Runtime-Invoked-Function-Arn");
            evnt.TraceId = GetHeaderValue(response.Headers, "Lambda-Runtime-Aws-Trace-Id");
            evnt.ClientContext = GetHeaderValue(response.Headers, "Lambda-Runtime-Aws-Client-Context");
            evnt.CognitoIdentity = GetHeaderValue(response.Headers, "Lambda-Runtime-Aws-Cognito-Identity");

            return evnt;
        }

        /// <summary>
        /// Attempts to retrieve the header value at the specified key.
        /// </summary>
        /// <param name="headers"></param>
        /// <param name="key"></param>
        /// <returns>The first value associated with the key if present, otherwise returns null.</returns>
        private static string GetHeaderValue(HttpResponseHeaders headers, string key)
        {
            if (headers.Contains(key))
            {
                return (new List<string>(headers.GetValues(key))[0]);
            }

            return null;
        }
    }
}
