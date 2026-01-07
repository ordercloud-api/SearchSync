
# 🔄 SearchSync Azure Function (Kafka Trigger) for Sitecore Search + OrderCloud  
**Kafka-triggered Azure Function for Reliable Data Sync**

> ⚡ **This Azure Function listens to Kafka topics and syncs product data to Sitecore Search** for fast, scalable, and production-ready indexing—ideal when your messaging backbone is Apache Kafka or Azure Event Hubs with Kafka protocol.

---

## 📌 Table of Contents
- [✨ Overview](#-overview)
- [⚙️ Requirements](#️-requirements)
- [🗂️ Configuration](#️-configuration)
- [🚀 Running Locally](#-running-locally)
- [🔗 OrderCloud Setup](#-ordercloud-setup)

---

## ✨ Overview

This project hosts **SearchSync** as an **Azure Function** with a **Kafka trigger**, designed for:

| Capability | Description |
|------------|-------------|
| 🔁 Syncing | Pushes product data from OrderCloud to Sitecore Search |
| ⚡ Scalable | Handles high-volume events via Kafka topics |
| 🛠️ Extensible | Add custom mappings for XP and other properties |

**Important:** The provided code syncs only the minimum properties required by OrderCloud’s search proxy. Extend the mappers in [SearchSync.Common/Mappers](../SearchSync.Common/Mappers/) to include additional fields or XP as needed for your Sitecore Search configuration.

---

## ⚙️ Requirements

- Azure Subscription with:  
  - **Azure Functions**  
  - **Azure Event Hubs (Kafka protocol enabled)** *or* **Managed Kafka Cluster**  
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
    "FUNCTIONS_WORKER_RUNTIME": "dotnet-isolated",
    "APPLICATIONINSIGHTS_CONNECTION_STRING": "",

    // Search settings
    "SearchSettings__IngestionEndpointUrl": "",
    "SearchSettings__IngestionApiKey": "",
    "SearchSettings__DomainID": "",
    "SearchSettings__SourceID": "",
    "SearchSettings__EntityProduct": "",

    // Kafka connection settings
    "Kafka__BootstrapServers": "localhost:9092",
    "Kafka__SaslUsername": "",
    "Kafka__SaslPassword": "",
    "Kafka__Topic": "products",
    "Kafka__ConsumerGroup": "searchsync-consumer-group"
  }
}
```

### 🔑 Setting Reference

| Key                          | Purpose                                              | Where To Find / Configure                           | Example                                   |
|------------------------------|------------------------------------------------------|-----------------------------------------------------|-------------------------------------------|
| `AzureWebJobsStorage`        | Azure Storage connection string                      | Azure Portal → Storage Accounts → Access Keys       | `DefaultEndpointsProtocol=https;...`      |
| `APPLICATIONINSIGHTS_CONNECTION_STRING` | (Optional) App Insights connection string     | Azure Portal → Application Insights → Overview      | `InstrumentationKey=...;IngestionEndpoint=...` |
| `SearchSettings__DomainID`   | Sitecore Search domain ID                            | CEC → Administration → Domain Settings              | `111111111`                                |
| `SearchSettings__SourceID`   | ID of Push API source                                | CEC → Sources → [Source] → Source Information       | `1111111`                                  |
| `SearchSettings__IngestionEndpointUrl` | Ingestion endpoint URL                          | CEC → Sources → [Source] → Endpoint URL             | `https://discover.sitecorecloud.io/ingestion/v1` |
| `SearchSettings__IngestionApiKey` | API key for ingestion                            | CEC → Sources → [Source] → API Key                  | `01-xxxxxxxx...`                           |
| `SearchSettings__EntityProduct` | Name of product entity                            | CEC → Administration → Domain Settings → Entities   | `product`                                  |
| `FUNCTIONS_WORKER_RUNTIME`   | Azure Functions runtime identifier                   | Not applicable                                      | `dotnet-isolated`                          |
| `Kafka__BootstrapServers`    | Kafka broker list                                    | Your Kafka/Event Hubs (Kafka) configuration         | `localhost:9092` or `broker1:9092,broker2:9092` |
| `Kafka__SaslUsername`        | SASL username (if required)                          | Your Kafka security configuration                   | `myUser`                                   |
| `Kafka__SaslPassword`        | SASL password (if required)                          | Your Kafka security configuration                   | `myPassword`                               |
| `Kafka__Topic`               | Topic to consume                                     | Kafka topic configuration                           | `products`                                 |
| `Kafka__ConsumerGroup`       | Consumer group name                                  | Kafka consumer configuration                         | `searchsync-consumer-group`                |

> 💡 **Function binding note:** In your trigger, reference these settings via configuration placeholders, for example:
>
> ```csharp
> [KafkaTrigger("%Kafka__BootstrapServers%", "%Kafka__Topic%", ConsumerGroup = "%Kafka__ConsumerGroup%")]
> ```

---

## 🚀 Running Locally

1. Ensure `local.settings.json` exists at the project root and includes the configuration shown above.  
2. Start the function:
   - **IDE**: Use Run / Debug in Visual Studio or VS Code  
   - **Core Tools**: From the project root, run:
     ```
     func start
     ```
> ℹ️ When using Azure Functions Core Tools, ensure `local.settings.json` exists and is not excluded from your local environment.

---

## 🔗 OrderCloud Setup

To publish OrderCloud events to Kafka and have this function consume them:

### 1️⃣ Create a Delivery Configuration

[POST /integrations/deliveryconfig](https://ordercloud.io/api-reference/integrations/delivery-configurations/create)

```json
{
  "ID": "sitecore-search-kafka",
  "Name": "Sitecore Search Kafka",
  "Enabled": true,
  "DeliveryTargets": {
    "Kafka": {
      "BootstrapServers": "YOUR_SERVERS",
      "SaslUsername": "YOUR_SASL_USERNAME",
      "SaslPassword": "YOUR_SASL_PASSWORD",
      "Topic": "YOUR_TOPIC_NAME"
    }
  }
}
```

- `BootstrapServers` should match your Kafka cluster brokers (or Azure Event Hubs Kafka endpoint).  
- `SaslUsername` / `SaslPassword` only if your Kafka cluster requires SASL auth.  
- `Topic` must match `Kafka__Topic` in your function settings.

### 2️⃣ Associate with a Sync

[PUT /integrations/productsync](https://api-docs.sitecore.com/ordercloud/product-synchronization/productsyncs.save)

```json
{
  "SyncProductDeleted": true,
  "SyncProductChanged": true,
  "DeliveryConfigID": "sitecore-search-kafka"
}
```

This ensures product events are delivered to your Kafka topic for the function to process

