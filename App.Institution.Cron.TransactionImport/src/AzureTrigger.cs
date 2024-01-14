using Microsoft.Azure.WebJobs;
using Microsoft.Azure.Functions.Worker;

namespace App.Institution.Cron.TransactionImport;

public static class QueueTransactionImportFunction
{
    [Function(nameof(QueueTransactionImportFunction))]
    public static async void Run([TimerTrigger("0 */5 * * * *")] TimerInfo timerInfo, FunctionContext context, Service service)
    {
        await service.QueueImports();
    }
}
