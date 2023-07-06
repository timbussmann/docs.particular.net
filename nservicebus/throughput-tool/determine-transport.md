---
title: Determining message transport
summary: How to determine what message transport a system uses to run the throughput tool
reviewed: 2023-07-06
---

This article will help to determine what message transport an NServiceBus system uses so that the [system throughput tool](/nservicebus/throughput-tool/#running-the-tool) can be run to generate a report to send to Particular Software for licensing purposes.

Often, the original developer will know what message transport is in use, but this article makes the assumption that the original developer(s) of the system are not available.

## Find NServiceBus processes

An NServiceBus system is made of multiple physical processes that all communicate using messages. Often, this will include one or more web or front-end applications that serve as the user interface plus one or more background services that process messages generated by the front-end applications. The various processes may be located on multiple hosts or virtual machines.

Each NServiceBus application will contain a `NServiceBus.dll` and/or `NServiceBus.Core.dll` in its runtime directory. There may also be other DLLs with names starting with `NServiceBus.`. These can be collectively called **NServiceBus assemblies**.

There are various ways to find NServiceBus processes:

1. Look for NServiceBus assemblies in the `bin` directory of any running web applications configured in IIS.
2. Use <kbd>Windows</kbd> + <kbd>R</kbd> to run `services.msc` and look for services configured to **Log On As** either `Network Service` or a domain account. Ignore the typical services that ship with Windows.
  1. Right-click the service and select **Properties**.
  2. Copy the folder part of the **Path to executable** and look in that path for NServiceBus assemblies.
3. Search the hard drive for `NServiceBus.Core.dll`. Any location where this is found is usually the runtime directory of an NServiceBus application.

It's generally sufficient to find one NServiceBus application, as in most cases, the throughput tool will reveal how many exist in total.

## Look for transport DLLs

Examine the executable directory of the NServiceBus services. The presence of any of the DLLs listed below will determine what message transport is used, which in turn dictates the data collection mechansim.

| DLL Name | Message Transport | Collection Method |
|-|-|-|
| `NServiceBus.Transport.AzureServiceBus.dll` | Azure Service Bus | [Azure Service Bus](azure-service-bus.md) |
| `NServiceBus.Azure.Transports.WindowsAzureServiceBus.dll` | Azure Service Bus | [Azure Service Bus](azure-service-bus.md) |
| `NServiceBus.Transport.SQS.dll` | Amazon SQS | [Amazon SQS](amazon-sqs.md) |
| `NServiceBus.AmazonSQS.dll` | Amazon SQS | [Amazon SQS](amazon-sqs.md) |
| `NServiceBus.Transport.RabbitMQ.dll` | RabbitMQ | [RabbitMQ](rabbitmq.md) |
| `NServiceBus.Transports.RabbitMQ.dll` | RabbitMQ | [RabbitMQ](rabbitmq.md) |
| `NServiceBus.Transport.SQLServer.dll` | SQL Server Transport | [SQL Server Transport](sql-transport.md) |
| `NServiceBus.Transports.SQLServer.dll` | SQL Server Transport | [SQL Server Transport](sql-transport.md) |
| `NServiceBus.Transport.Msmq.dll` | MSMQ | See [For MSMQ or Azure Storage Queues](#for-msmq-or-azure-storage-queues) below |
| `NServiceBus.Azure.Transports.WindowsAzureStorageQueues.dll` | Azure Storage Queues | See [For MSMQ or Azure Storage Queues](#for-msmq-or-azure-storage-queues) below |
| `NServiceBus.Transport.AzureStorageQueues.dll` | Azure Storage Queues | See [For MSMQ or Azure Storage Queues](#for-msmq-or-azure-storage-queues) below |

If one of the DLLs in the first column exists, follow the link in the third column to use that data collection mechanism.

### If no transport DLLs exist

If none of the DLLs above appears in the service's runtime directory, but an `NServiceBus.Core.dll` does exist, the system probably uses MSMQ as its message transport, as that transport was built into the `NServiceBus.Core.dll` until NServiceBus version 7.0.

It is possible to validate that MSMQ is the message transport by checking the status of the MSMQ service:

1. On the Windows server hosting the service, open **Computer Management** by pressing <kbd>Windows</kbd> + <kbd>R</kbd> and running `compmgmt.msc`.
2. In the left pane, expand **Services and Applications** > **Message Queuing**.
3. For a server that processes backend requests, clicking on **Private Queues** will likely show a number of queues.
4. If there are no private queues, especially in the case of a load-balanced web server, clicking on **Outgoing Queues** will likely show connections where messages are sent to other servers. These server names are good places to look for additional NServiceBus services.

If these steps aren't possible, such as if the Message Queuing service doesn't exist, email contact@particular.net for help identifying the message transport.

## For MSMQ or Azure Storage Queues

When using the MSMQ or Azure Storage Queues transport, the only way to collect throughput data using the throughput tool is if the system also has an instance of [ServiceControl](/servicecontrol/) installed. ServiceControl is an optional tool, so it's possible that a system might not have it.

Note that ServiceControl is a form of a database, and is commonly installed on its own virtual machine.

To find where a ServiceControl instance might be:

1. Find an instance on the current server by using <kbd>Windows</kbd> + <kbd>R</kbd> to run `services.msc` and look for services that contain the word `ServiceControl` in either the **Name** or **Description** column.
2. If the system has been determined to use the MSMQ transport, it might be possible to find the ServiceControl server by following the steps under the [If no transport DLLs exist](#look-for-transport-dlls-if-no-transport-dlls-exist) to find the Outgoing Queues. An outgoing queue for `error` or `audit` (or a queue name containing one of those words) will likely point to the ServivceControl server.

If a ServiceControl instance can't be found, email contact@particular.net for instructions on how to estimate the number of endpoints and system throughput.