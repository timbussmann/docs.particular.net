---
title: Measuring system throughput using Azure Service Bus
summary: Use the Particular throughput tool to measure the throughput of an NServiceBus system.
reviewed: 2022-11-09
related:
  - nservicebus/throughput-tool
---

The Particular throughput tool can be installed locally and run against a production system to discover the throughput of each endpoint in a system over a period of time.

This article details how to collect endpoint and throughput data when the system uses the [Azure Service Bus transport](/transports/azure-service-bus/). Refer to the [throughput counter main page](./) for information how to install/uninstall the tool or for other data collection options.

## Prerequisites

Collecting metrics from Azure Service Bus relies upon an existing set of Azure credentials set using the Azure Command Line Interface (CLI), which must be installed first:

1. Install the [Azure CLI](https://docs.microsoft.com/en-us/cli/azure/install-azure-cli).
2. From a command line, execute `az login`, which will open a browser to complete the authentication to Azure. The Azure login must have access to view metrics data for the Azure Service Bus namespace.
3. Execute `az account set --subscription {SubscriptionId}`, where `{SubscriptionId}` is a Guid matching the subscription id that contains the Azure Service Bus namespace.

Completing these steps stores credentials that can be used by the tool.

## Running the tool

To run the tool, the resource ID for the Azure Service Bus namespace is needed.

In the Azure Portal, go to the Azure Service Bus namespace, click **Properties** in the side navigtation (as shown in the screenshot below) and then copy the `Id` value, which will be needed to run the tool. The `Id` value should have a format similar to `/subscriptions/{Guid}/resourceGroups/{rsrcGroupName}/providers/Microsoft.ServiceBus/namespaces/{namespaceName}`.

This screenshot shows how to copy the Service Bus Namespace's `Id` value:

![How to collect the Service Bus Namespace Id](azure-service-bus.png)

Execute the tool with the resource ID of the Azure Service Bus namespace, as in this example:

```shell
throughput-counter azureservicebus --resourceId /subscriptions/xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx/resourceGroups/my-resource-group/providers/Microsoft.ServiceBus/namespaces/my-asb-namespace
```

Using Azure Service Bus metrics allows the tool to capture the last 30 days worth of data at once. Although the tool collects 30 days worth of data, only the highest daily throughput is included in the report.

## Options

| Option | Description |
|-|-|
| <nobr>`--resourceId`</nobr> | **Required** – The resource ID of the Azure Service Bus namespace, which can be found in the Azure Portal as described above. |
| <nobr>`--serviceBusDomain`</nobr> | The Service Bus domain. Defaults to `servicebus.windows.net`. Only necessary for Azure customers using a [non-public/government cloud](https://learn.microsoft.com/en-us/rest/api/servicebus/). |
include: throughput-tool-global-options