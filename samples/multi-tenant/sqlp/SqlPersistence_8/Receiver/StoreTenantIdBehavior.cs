using System;
using System.Threading.Tasks;

using NServiceBus.Pipeline;

public class StoreTenantIdBehavior :
    Behavior<IIncomingLogicalMessageContext>
{
    public override Task Invoke(IIncomingLogicalMessageContext context, Func<Task> next)
    {
        if (context.MessageHeaders.TryGetValue("tenant_id", out var tenant))
        {
            Console.WriteLine($"Setting tenent id to {tenant}");
            context.Extensions.Set("TenantId", tenant);
        }
        return next();

    }
}
