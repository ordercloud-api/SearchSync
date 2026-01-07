
# 🌐 SearchSync Web API (HTTP) for Sitecore Search + OrderCloud  
**ASP.NET Core Web API for Demo & Proof-of-Concept Sync**

> ⚠️ **This Web API exposes HTTP endpoints to sync product data from OrderCloud to Sitecore Search.**  
Best suited for **demos or proofs of concept**. For production environments, use an EventHub or Kafka-based approach for improved resilience.

---

## 📌 Table of Contents
- [✨ Overview](#-overview)
- [⚙️ Requirements](#️-requirements)
- [🗂️ Configuration](#️-configuration)
- [🚀 Running Locally](#-running-locally)
- [📡 API Endpoints](#-api-endpoints)
- [🔗 OrderCloud Setup](#-ordercloud-setup)

---

## ✨ Overview

This project hosts **SearchSync** as an **ASP.NET Core Web API** with an **HTTP endpoint** for ingesting OrderCloud events into Sitecore Search.

| Capability | Description |
|------------|-------------|
| 🔁 Syncing | Pushes product data from OrderCloud to Sitecore Search |
| 🧪 Demo-Friendly | Simple HTTP endpoint for quick testing |
| 🛠️ Extensible | Add custom mappings for XP and other properties |

**Important:** The provided code syncs only the minimum properties required by OrderCloud’s search proxy. Extend the mappers in [SearchSync.Common/Mappers](../SearchSync.Common/Mappers/) to include additional fields or XP as needed for your Sitecore Search configuration.

---

## ⚙️ Requirements

- Azure Subscription with:  
  - **Azure Storage Account** (if you enable Application Insights or other Azure services)  
- OrderCloud Marketplace  
- Sitecore Search instance with **Push API source**  
- .NET 8 SDK

---

## 🗂️ Configuration

You can configure the app via `appsettings.json`, `appsettings.{Environment}.json`, `local.settings.json` (gitignored), or environment variables.

> ⚠️ Do **not** commit secrets to source control.

### Example `local.settings.json`

```json
{
  "IsEncrypted": false,
  "Values": {
    "SearchSettings__IngestionEndpointUrl": "https://discover.sitecorecloud.io/ingestion/v1",
    "SearchSettings__IngestionApiKey": "your-api-key",
    "SearchSettings__DomainId": "your-domain-id",
    "SearchSettings__SourceId": "your-source-id",

    "SearchSettings__EntityProduct": "Product",
    "SearchSettings__EntityCategory": "Category",
    "SearchSettings__EntityAdminUser": "AdminUser",
    "SearchSettings__EntityBuyer": "Buyer",
    "SearchSettings__EntityBuyerUserGroup": "BuyerUserGroup",
    "SearchSettings__EntityBuyerUser": "BuyerUser",
    "SearchSettings__EntityInventoryRecord": "InventoryRecord",
    "SearchSettings__EntitySupplier": "Supplier",
    "SearchSettings__EntitySupplierUser": "SupplierUser"
  }
}
```

### 🔑 Setting Reference

| Key                               | Purpose                                    | Where To Find | Example |
|-----------------------------------|--------------------------------------------|---------------|---------|
| `SearchSettings__DomainId`        | Sitecore Search domain ID                  | CEC → Administration → Domain Settings | `111111111` |
| `SearchSettings__SourceId`        | ID of Push API source                      | CEC → Sources → [Source] → Source Information | `1111111` |
| `SearchSettings__IngestionEndpointUrl` | Ingestion endpoint URL                    | CEC → Sources → [Source] → Endpoint URL | `https://discover.sitecorecloud.io/ingestion/v1` |
| `SearchSettings__IngestionApiKey` | API key for ingestion                      | CEC → Sources → [Source] → API Key | `01-xxxxxxxx...` |
| `SearchSettings__EntityProduct`   | Name of product entity                     | CEC → Administration → Domain Settings → Entities | `product` |

> ℹ️ Configuration sources are loaded in this order (later overrides earlier):  
> 1) `appsettings.json` → 2) `appsettings.{ENV}.json` → 3) `local.settings.json` → 4) environment variables.

---

## 🚀 Running Locally

1. Ensure configuration is set (e.g., `local.settings.json` or environment variables).  
2. Start the Web API:
   ```bash
   dotnet build
   dotnet run
   ```
3. Swagger UI is typically available at `/swagger` in Development.

---

## 📡 API Endpoints

### POST /api/sync/process

Processes an OrderCloud event and ingests it into Sitecore Search.

**Request Body (example):**
```json
{
  "Headers": {
    "type": "sitecore.ordercloud.messages.product.updated"
  },
  "Payload": {
    "ProductID": "PROD-001",
    "Name": "Product Name",
    "Description": "Product description",
    "Active": true,
    "Archived": false
  }
}
```

**Response (example):**
```json
{
  "message": "Event processed successfully",
  "eventType": "sitecore.ordercloud.messages.product.updated"
}
```

---

## 🔗 OrderCloud Setup

Configure **Delivery Configurations** and associate them with sync jobs to send events to this Web API.

### 1️⃣ Create a Delivery Configuration

https://ordercloud.io/api-reference/integrations/delivery-configurations/create

```json
{
  "ID": "sitecore-search-http",
  "Name": "Sitecore Search HTTP",
  "Enabled": true,
  "DeliveryTargets": {
    "Http": {
      "Endpoint": "https://your-hosted-webapi/api/sync/process",
      "CustomAuthHeaderName": "x-api-key",
      "CustomAuthHeaderValue": "YOUR_API_KEY",
      "Secret": "super-secret-key"
    }
  }
}
```

- `CustomAuthHeaderName` / `CustomAuthHeaderValue` should match what your Web API expects for authorization (e.g., API key header).  
- `Secret` is required by OrderCloud but is not validated by this project unless you add support.

### 2️⃣ Associate with a Sync

https://api-docs.sitecore.com/ordercloud/product-synchronization/productsyncs.save

```json
{
  "SyncProductDeleted": true,
  "SyncProductChanged": true,
  "DeliveryConfigID": "sitecore-search-http"
}
```

This ensures product events are delivered to your Web API
