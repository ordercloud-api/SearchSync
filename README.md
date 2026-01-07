# SearchSync

Synchronize data from OrderCloud to Sitecore Search by leveraging OrderCloud's ProductSync and EntitySync features. Azure Functions ingest sync events, transform the payloads, and index them in Sitecore Search.

**Important Note:** The provided code synchronizes only the minimum set of properties required by OrderCloud's search proxy. It is expected that developers will extend the code ([SearchSync.Common/Mappers](./SearchSync.Common/Mappers/)) to include any additional relevant properties, including XP (extended properties), based on their specific Sitecore Search requirements. This practice helps to optimize indexing performance and avoid unnecessary data transfer.

## Architecture Diagram

![Architecture Diagram](/architecture-diagram.png)

## Examples 

We've included sample implementations to help you get started: 

### Azure Functions Variants

- [Azure EventHub](./SearchSync.AzureFunctionsEventHub/README.md) (recommended for production)
- [Apache Kafka](./SearchSync.AzureFunctionsKafka/README.md) (recommended for production)
- [HTTP](./SearchSync.AzureFunctionsHttp/README.md) (ideal for demos or proofs of concept)

### Web API Variants

- [EventHub](./SearchSync.WebApiEventHub/README.md) (recommended for production)
- [Apache Kafka](./SearchSync.WebApiKafka/README.md) (recommended for production)
- [HTTP](./SearchSync.WebApiHttp/README.md) (ideal for demos or proofs of concept)

Check out the README files in the example projects for instructions