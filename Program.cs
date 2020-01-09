using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Amazon.Lambda.RuntimeSupport;
using Amazon.Lambda.Serialization.Json;
using Amazon.Lambda.SQSEvents;
using NetCore3Lambda.Detector;
using System;
using System.Threading.Tasks;

namespace NetCore3Lambda
{
    public class Program
    {
        /// <summary>
        /// Entry point into the Lambda via the custom runtime.
        /// </summary>
        /// <param name="args"></param>
        public static void Main(string[] args)
        {
            LambdaSource triggerSource = LambdaDetector.DetectSource();

            switch (triggerSource)
            {
                case LambdaSource.ApiGateway:
                    TriggerFromApiGateway();
                    break;

                case LambdaSource.Sqs:
                    TriggerFromSqs();
                    break;

                default:
                    break;
            }
        }

        /// <summary>
        /// Follows the standard pattern for a ASP.NET-based Lambda fed from an API Gateway.
        /// See: https://github.com/aws/aws-lambda-dotnet/tree/master/Libraries/src/Amazon.Lambda.APIGatewayEvents
        /// </summary>
        public static void TriggerFromApiGateway()
        {
            var lambdaEntry = new ApiGatewayLambdaFunction();
            var functionHandler = (Func<APIGatewayProxyRequest, ILambdaContext, Task<APIGatewayProxyResponse>>)(lambdaEntry.FunctionHandlerAsync);

            using (var handlerWrapper = HandlerWrapper.GetHandlerWrapper(functionHandler, new JsonSerializer()))
            using (var bootstrap = new LambdaBootstrap(handlerWrapper))
            {
                bootstrap.RunAsync().Wait();
            }
        }

        /// <summary>
        /// Follows the standard pattern for a Lambda fed from a SQS.
        /// See: https://github.com/aws/aws-lambda-dotnet/tree/master/Libraries/src/Amazon.Lambda.SQSEvents
        /// </summary>
        public static void TriggerFromSqs()
        {
            var lambdaEntry = new SqsLambdaFunction();
            Action<SQSEvent, ILambdaContext> func = lambdaEntry.FunctionHandler;

            using (var handlerWrapper = HandlerWrapper.GetHandlerWrapper(func, new JsonSerializer()))
            using (var bootstrap = new LambdaBootstrap(handlerWrapper))
            {
                bootstrap.RunAsync().Wait();
            }
        }
    }
}