
# 🌐 SearchSync ASP.NET Core Web API (Kafka Consumer) for Sitecore Search + OrderCloud
**Kafka-backed Web API for Reliable Data Sync**

> ⚡ Consumes Kafka topics carrying OrderCloud events and syncs product data to Sitecore Search. Use when your messaging backbone is Apache Kafka or Azure Event Hubs (Kafka protocol).

---

## 📌 Table of Contents
- [✨ Overview](#-overview)
- [⚙️ Requirements](#️-requirements)
- [🗂️ Configuration](#️-configuration)
- [🚀 Running Locally](#-running-locally)
- [🔗 OrderCloud Setup](#-ordercloud-setup)

---

## ✨ Overview

This project hosts **SearchSync** as an **ASP.NET Core Web API** with a **background service** that consumes **Kafka** messages and ingests them into Sitecore Search.

| Capability | Description |
|------------|-------------|
| 🔁 Syncing | Pushes product data from OrderCloud to Sitecore Search |
| ⚡ Scalable | Handles high-volume events via Kafka topics |
| 🛠️ Extensible | Add custom mappings for XP and other properties |

**Important:** The code syncs only the minimum properties required by OrderCloud’s search proxy. Extend the mappers in [SearchSync.Common/Mappers](../SearchSync.Common/Mappers/) for your Sitecore Search needs.

---

## ⚙️ Requirements

- Azure Subscription with:
  - **Azure Event Hubs (Kafka protocol enabled)** *or* **Managed Kafka Cluster**
  - **Azure Storage Account** (if you use checkpointing or telemetry)
  - **Application Insights** (optional)
- OrderCloud Marketplace  
- Sitecore Search instance with **Push API source**  
- **.NET 8 SDK** (or later)

---

## 🗂️ Configuration

Use `appsettings.json` (do **not** commit secrets):

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },

  "SearchSettings": {
    "IngestionEndpointUrl": "https://discover.sitecorecloud.io/ingestion/v1",
    "IngestionApiKey": "",
    "DomainID": "111111111",
    "SourceID": "1111111",
    "EntityProduct": "product"
  },

  "Kafka": {
    "BootstrapServers": "your-namespace.servicebus.windows.net:9093",
    "SaslUsername": "$ConnectionString",
    "SaslPassword": "Endpoint=sb://your-namespace.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=***REDACTED***",
    "Topic": "ordercloud-events",
    "ConsumerGroup": "searchsync-consumer-group"
  },

  "ApplicationInsights": {
    "ConnectionString": ""
  }
}
```

### 🔑 Setting Reference

- **SearchSettings.IngestionEndpointUrl** — Sitecore Search Push API endpoint  
- **SearchSettings.IngestionApiKey** — API key for Push API  
- **SearchSettings.DomainID** — Sitecore Search domain ID  
- **SearchSettings.SourceID** — Push source ID  
- **SearchSettings.EntityProduct** — Product entity name in Search (`product`)

- **Kafka.BootstrapServers** — Kafka brokers or **Azure Event Hubs Kafka endpoint**  
- **Kafka.SaslUsername / Kafka.SaslPassword** — Required if your Kafka endpoint enforces SASL (Event Hubs uses `$ConnectionString`)  
- **Kafka.Topic** — Topic to consume (e.g., `ordercloud-events`)  
- **Kafka.ConsumerGroup** — Consumer group name (e.g., `searchsync-consumer-group`)

---

## 🚀 Running Locally

1. Ensure `appsettings.json` has the required values (Search + Kafka).  
2. Start the Web API:
   - **Visual Studio / VS Code**: Run/Debug the project  
   - **CLI**: From the project root:
     ```
     dotnet run
     ```

---

## 🔗 OrderCloud Setup

Publish OrderCloud events to **Kafka** so this Web API can consume them.

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
- `Topic` must match `Kafka.Topic` in your app settings.

### 2️⃣ Associate with a Sync

[PUT /integrations/productsync](https://api-docs.sitecore.com/ordercloud/product-synchronization/productsyncs.save)

```json
{
  "SyncProductDeleted": true,
  "SyncProductChanged": true,
  "DeliveryConfigID": "sitecore-search-kafka"
}
```

This routes product events to your Kafka topic
