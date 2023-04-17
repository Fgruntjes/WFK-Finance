using System;
using System.Diagnostics;
using System.Threading;
using App.Deploy.Stacks;
using Pulumi;

bool.TryParse(Environment.GetEnvironmentVariable("PULUMI_DEBUG"), out var debug);

if (debug)
{
    Console.WriteLine($"Waiting for debugger (pid: {Environment.ProcessId})");
    while (!Debugger.IsAttached)
    {
        Console.WriteLine($"Waiting for debugger (pid: {Environment.ProcessId})");
        Thread.Sleep(500);
    }
}

return await Deployment.RunAsync<AppStack>();
