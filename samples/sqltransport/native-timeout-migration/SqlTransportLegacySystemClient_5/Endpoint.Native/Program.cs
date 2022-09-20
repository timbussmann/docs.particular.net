﻿using NServiceBus;
using System;
using System.Threading.Tasks;

class Program
{
    const string ConnectionString = @"Data Source=.\SqlExpress;Database=NsbSamplesNativeTimeoutMigration;Integrated Security=True";

    static async Task Main()
    {
        Console.Title = "Samples.SqlServer.NativeTimeoutMigration";
        var endpointConfiguration = new EndpointConfiguration("Samples.SqlServer.NativeTimeoutMigration");
        endpointConfiguration.SendFailedMessagesTo("error");
        var transport = endpointConfiguration.UseTransport<SqlServerTransport>();
        transport.ConnectionString(ConnectionString);

        endpointConfiguration.UsePersistence<NonDurablePersistence>();
        endpointConfiguration.EnableInstallers();

        var endpointInstance = await Endpoint.Start(endpointConfiguration);

        Console.WriteLine("The endpoint has started. Run the script to migrate the timeouts.");
        Console.WriteLine("Press any key to exit");
        Console.ReadKey();

        await endpointInstance.Stop();
    }
}