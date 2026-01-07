
# 🔄 SearchSync Azure Function for Sitecore Search + OrderCloud  
**EventHub-triggered Azure Function for Reliable Data Sync**

> ⚡ **This Azure Function listens to OrderCloud events via EventHub and syncs product data to Sitecore Search** for fast, scalable, and production-ready indexing.

---

## 📌 Table of Contents
- [✨ Overview](#-overview)
- [⚙️ Requirements](#️-requirements)
- [🗂️ Configuration](#️-configuration)
- [🚀 Running Locally](#-running-locally)
- [🔗 OrderCloud Setup](#-ordercloud-setup)

---

## ✨ Overview

This project hosts **SearchSync** as an **Azure Function** with an **EventHub trigger**, designed for:

| Capability | Description |
|------------|-------------|
| 🔁 Syncing | Pushes product data from OrderCloud to Sitecore Search |
| ⚡ Scalable | Handles high-volume events reliably |
| 🛠️ Extensible | Add custom mappings for XP and other properties |

> ✅ Recommended for **production environments** as it offers greater reliability and scalability than the [HTTP method](../SearchSync.AzureFunctionsHttp/README.md).

**Important:** The provided code syncs only the minimum properties required by OrderCloud’s search proxy. Extend the mappers in [SearchSync.Common/Mappers](../SearchSync.Common/Mappers/) to include additional fields or XP as needed for your Sitecore Search configuration.

---

## ⚙️ Requirements

- Azure Subscription with:  
  - **Azure Functions**  
  - **Azure Event Hub**  
  - **Azure Storage Account**  
- OrderCloud Marketplace  
- Sitecore Search instance with **Push API source**  

---

## 🗂️ Configuration

Create a `local.settings.json` at the project root:

> ⚠️ Do **not** commit secrets to source control.

```json
{
  "IsEncrypted": false,
  "Values": {
    "AzureWebJobsStorage": "",
    "EventHubConnectionString": "",
    "APPLICATIONINSIGHTS_CONNECTION_STRING": "",
    "SearchSettings__IngestionEndpointUrl": "",
    "SearchSettings__IngestionApiKey": "",
    "SearchSettings__DomainID": "",
    "SearchSettings__SourceID": "",
    "SearchSettings__EntityProduct": "",
    "FUNCTIONS_WORKER_RUNTIME": "dotnet-isolated"
  }
}
```

### 🔑 Setting Reference

| Key                                         | Purpose                                                      | Where To Find | Example |
|---------------------------------------------|--------------------------------------------------------------|---------------|---------|
| `AzureWebJobsStorage`                      | Connection string for Azure Storage                         | Azure Portal → Storage Accounts → [Account] → Access Keys | `DefaultEndpointsProtocol=https;AccountName=youraccount;AccountKey=***;EndpointSuffix=core.windows.net` |
| `EventHubConnectionString`                 | Connection string for EventHub                              | Azure Portal → Event Hubs → [Namespace] → Shared Access Policies | `Endpoint=sb://your-namespace.servicebus.windows.net/...` |
| `APPLICATIONINSIGHTS_CONNECTION_STRING`    | (Optional) App Insights connection string                   | Azure Portal → Application Insights → Overview | `InstrumentationKey=xxxx;IngestionEndpoint=https://yourregion.applicationinsights.azure.com/` |
| `SearchSettings__DomainID`                 | Sitecore Search domain ID                                   | CEC → Administration → Domain Settings | `111111111` |
| `SearchSettings__SourceID`                 | ID of Push API source                                       | CEC → Sources → [Source] → Source Information | `1111111` |
| `SearchSettings__IngestionEndpointUrl`     | Ingestion endpoint URL                                      | CEC → Sources → [Source] → Endpoint URL | `https://discover.sitecorecloud.io/ingestion/v1` |
| `SearchSettings__IngestionApiKey`          | API key for ingestion                                       | CEC → Sources → [Source] → API Key | `01-xxxxxxxx...` |
| `SearchSettings__EntityProduct`            | Name of product entity                                      | CEC → Administration → Domain Settings → Entities | `product` |
| `FUNCTIONS_WORKER_RUNTIME`                 | Azure Functions runtime identifier                          | Not applicable | `dotnet-isolated` |

---

## 🚀 Running Locally

1. Ensure `local.settings.json` exists at the project root and includes the configuration shown above.
2. Run the project:
   - **IDE**: Use Run / Debug in Visual Studio or VS Code
   - **Core Tools**: From the project root, run:
     ```
     func start
     ```
> ℹ️ When using Azure Functions Core Tools, ensure `local.settings.json` exists and is not excluded from your local environment.
---

## 🔗 OrderCloud Setup

To receive events from OrderCloud, configure **Delivery Configurations** and associate them with sync jobs:

### 1️⃣ Create a Delivery Configuration

[POST /integrations/deliveryconfig](https://ordercloud.io/api-reference/integrations/delivery-configurations/create)

```json
{
  "ID": "sitecore-search-eventhub",
  "Name": "Sitecore Search EventHub",
  "Enabled": true,
  "DeliveryTargets": {
    "EventHub": {
      "EventHubName": "YOUR_EVENTHUB_NAME",
      "ConnectionString": "YOUR_CONNECTION_STRING"
    }
  }
}
```

- `EventHubName` must match the one in [SearchSyncFunction.cs](./SearchSyncFunction.cs)
- `ConnectionString` must include the **Send** claim

### 2️⃣ Associate with a Sync

[PUT /integrations/productsync](https://api-docs.sitecore.com/ordercloud/product-synchronization/productsyncs.save)

```json
{
  "SyncProductDeleted": true,
  "SyncProductChanged": true,
  "DeliveryConfigID": "sitecore-search-eventhub"
}
```

This ensures product events are delivered to your EventHub for processing.
