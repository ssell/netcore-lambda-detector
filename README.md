# .NET Core 3.0 Lambda Detector

## About

This repository serves as an example of how to detect the trigger service (API Gateway or SQS) of a .NET Core 3.0 Lambda using a Custom Runtime. 

Using the `LambdaDetector` allows for the writing of Lambdas which use a single shared codebase but allow for triggering from multiple different services. Typically Lambdas are written for firing off of a single specific service using a provided Lambda bootstrap class.

There is an accompany ramble - [Triggering .NET Core Lambdas from Multiple Services]([123](https://www.vertexfragment.com/ramblings/multiple-lambda-triggers/)).

## Disclaimer

This repository is only an example/reference for the C# implementation and should not be considered a complete application. It is up to the reader to design, architect, and implement the appropriate AWS stack (ideally using CloudFormation and serverless deployment) to suit the needs of their project.