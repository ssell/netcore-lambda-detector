using Amazon.Lambda.Core;
using Amazon.Lambda.SQSEvents;

namespace NetCore3Lambda
{
    /// <summary>
    /// Entry point for Lambdas triggered by an SQS.
    /// </summary>
    public class SqsLambdaFunction
    {
        public void FunctionHandler(SQSEvent evnt, ILambdaContext context)
        {
            foreach (var record in evnt.Records)
            {
                // ... Process each message ...
            }
        }
    }
}
