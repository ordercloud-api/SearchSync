
# 🔄 SearchSync ASP.NET Core Web API (Event Hub Consumer) for Sitecore Search + OrderCloud
**Event Hub–backed Web API for Reliable Data Sync**

> ⚡ Continuously consumes Azure Event Hub events from OrderCloud and syncs product data to Sitecore Search.

---


## 📌 Table of Contents
- [✨ Overview](#overview)
- [⚙️ Requirements](#requirements)
- [🗂️ Configuration](#configuration)
- [🚀 Running Locally](#running-locally)
- [🔗 OrderCloud Setup](#ordercloud-setup)

---

## ✨ Overview

This project hosts **SearchSync** as an **ASP.NET Core Web API** with a **background service** that listens to **Azure Event Hub** and ingests events into Sitecore Search.

| Capability | Description |
|------------|-------------|
| 🔁 Syncing | Pushes product data from OrderCloud to Sitecore Search |
| ⚙️ Reliable | Checkpointed processing for resilience and at-least-once delivery |
| 🛠️ Extensible | Extend mappings for XP and other properties |

**Important:** The code syncs only the minimum properties required by OrderCloud’s search proxy. Extend the mappers in [SearchSync.Common/Mappers](../SearchSync.Common/Mappers/) for your Sitecore Search needs.

---

## ⚙️ Requirements

- Azure Subscription with:
  - **Event Hub Namespace** and **Event Hub instance**
  - **Azure Storage Account** (for checkpoint management)
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

  "EventHub": {
    "ConnectionString": "Endpoint=sb://YOUR_NAMESPACE.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=YOUR_KEY",
    "EventHubName": "ordercloud-events",
    "ConsumerGroup": "$Default",
    "StorageConnectionString": "DefaultEndpointsProtocol=https;AccountName=YOUR_ACCOUNT;AccountKey=YOUR_KEY;EndpointSuffix=core.windows.net",
    "StorageContainerName": "eventhub-checkpoints"
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

- **EventHub.ConnectionString** — Event Hub namespace connection string  
- **EventHub.EventHubName** — Event Hub instance name (e.g., `ordercloud-events`)  
- **EventHub.ConsumerGroup** — Consumer group (e.g., `$Default`)  
- **EventHub.StorageConnectionString** — Storage account connection string for checkpoints  
- **EventHub.StorageContainerName** — Container for checkpoints (e.g., `eventhub-checkpoints`)

---

## 🚀 Running Locally

1. Ensure `appsettings.json` has the required values (Search + Event Hub + Storage).  
2. Start the Web API:
   - **Visual Studio / VS Code**: Run/Debug the project  
   - **CLI**: From the project root:
     ```
     dotnet run
     ```

---

## 🔗 OrderCloud Setup

Publish OrderCloud events to **Azure Event Hub** so this Web API can consume them.

### 1️⃣ Create a Delivery Configuration

[POST /integrations/deliveryconfig](https://ordercloud.io/api-reference/integrations/delivery-configurations/create)

```json
{
  "ID": "sitecore-search-eventhub",
  "Name": "Sitecore Search Event Hub",
  "Enabled": true,
  "DeliveryTargets": {
    "EventHub": {
      "EventHubName": "ordercloud-events",
      "ConnectionString": "Endpoint=sb://YOUR_NAMESPACE.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=YOUR_KEY"
    }
  }
}
```

- `EventHubName`: Must match **EventHub.EventHubName** in your app settings.  
- `ConnectionString`: Must include the **Send** claim.

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
