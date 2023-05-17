namespace App.Backend.Swagger;

using NSwag.Generation.Processors;
using NSwag.Generation.Processors.Contexts;

internal class OperationIdProcessor : IOperationProcessor
{
    public bool Process(OperationProcessorContext context)
    {
        context.OperationDescription.Operation.OperationId = context.MethodInfo.Name;
        return true;
    }
}