# GPU RAG Benchmark

This benchmark compares a standard CPU/Cloud retrieval pipeline against a fully GPU-accelerated pipeline using NVIDIA NIM and NeMo Retriever.

## Tested Models

> [!IMPORTANT]
> This project uses paid Azure resources such as Azure AI Search, Azure OpenAI, Storage, Key Vault, and Document Intelligence.  
> For local development, the recommended workflow is: `terraform apply → test → delete the resource group`.

CPU
  - Document Intelligence (FormRecognizerService)
  - Azure OpenAI Embeddings (TextEmbeddingService)
  - Azure OpenAI Chat (IChatService implemented by GPT_4_Model)
  - Azure Azure AI Search (via SearchClient, index: documents-index)

GPU
  - NVIDIA NeMo Retriever
  - NVIDIA NIM microservices

## Prerequisites
- .NET SDK 10.
- Azure subscription with:
  - Azure Key Vault
  - Azure Storage Account (Blob)
  - Azure AI Document Intelligence (Form Recognizer)
  - Azure AI Search (Azure AI Search) with vector search enabled
  - Azure OpenAI resource with deployments for:
    - an Embeddings model
    - a Chat model (e.g., GPT-4)
  - (Optional) Application Insights
- Local Azure login for Key Vault access (e.g., via `az login`) or a managed identity in hosting.

> [!NOTE]
> The app uses `DefaultAzureCredential` to access Azure Key Vault.  
> Locally, run `az login` and make sure the correct subscription is selected before starting the API.

## Configuration

Environment variables:
- KEYVAULT_URI (required): URI of your Key Vault, e.g. https://your-kv.vault.azure.net/
- APPLICATIONINSIGHTS_CONNECTION_STRING (optional): to enable telemetry export.

Key Vault secrets used by the API (as referenced in code):
- Azure Azure AI Search:
  - Azure--search-endpoint: e.g. https://your-search.search.windows.net
  - Azure--search-key: admin/query key for the service
- Document Intelligence:
  - Azure--Form-Recognizer-Endpoint: e.g. https://your-fr.cognitiveservices.azure.com
  - Azure--Form-Recognizer-Key: the API key

> [!WARNING]
> Do not commit `.tfstate`, `.tfvars`, Azure keys, connection strings, or downloaded model files.  
> Secrets should be stored in Azure Key Vault or local user-secrets only.

Additional secrets required by other services (check their implementations for exact names):
- Azure OpenAI (endpoint, key, and deployment names for embeddings and chat)
- Blob Storage (connection information or credentials used by IBlobStorageService)

Authentication to Key Vault:
- The app uses DefaultAzureCredential. Locally, ensure you are logged in (`az login`) or have a suitable developer identity configured. In Azure, assign a managed identity with Key Vault Secret Get permissions.

## Models
---
> [!WARNING]
> Hugging Face / ONNX model artifacts are not committed to the repository.  
> Download them locally using the provided model download script.

---
### Azure OpenAI deployments (models)
!["AzureOpenAI"](docs/pictures/deployments.png)

### HuggingFace model
!["LocalModel"](docs/pictures/local-model.png)

Set environment variables:
```bash
# Key Vault
export KEYVAULT_URI="https://your-kv.vault.azure.net/"

# (Optional) App Insights
export APPLICATIONINSIGHTS_CONNECTION_STRING="InstrumentationKey=...;IngestionEndpoint=..."
```

Ensure required secrets exist in Key Vault:
- Azure--search-endpoint
- Azure--search-key
- Azure--Form-Recognizer-Endpoint
- Azure--Form-Recognizer-Key
- Plus required OpenAI and Blob Storage secrets used by your service classes.

Requirements:
- Azure AI Search index named documents-index with a vector field embedding matching your embedding dimensions.

### POST /ask
- Sends a JSON question, retrieves relevant chunks from Azure AI Search, optionally reranks them, and answers using the configured Azure OpenAI chat deployment.

Example:
```bash
curl -k -X POST "http://localhost:5292/ask" \
  -H "Content-Type: application/json" \
  -d '{"question":"What is the maximum upload file size?"}'
```

Response (200 OK):
```json
{
  "question": "What is the maximum upload file size?",
  "answer": "The maximum upload file size is **30 MB**."
}
```

## Azure setup checklist

> [!IMPORTANT]
> Not every Azure service is available in every region.  
> In this setup, most resources can run in `polandcentral`, while Document Intelligence may need a different region such as `swedencentral`.

1) Key Vault
- Create a Key Vault and grant your identity Secret Get permissions.
- Add the required secrets (search, form recognizer, and those used by OpenAI/Blob services).

2) Azure Azure AI Search
- Create the service and an index named documents-index.
- Include fields similar to:
  - id (Edm.String, key)
  - content (Edm.String, searchable, filterable=false, sortable=false)
  - embedding (collection of single/float numbers) with a vector profile configured to match your embedding model’s dimensionality and algorithm.
- Enable vector search on the service and on the embedding field.

3) Document Intelligence (Form Recognizer)
- Create a resource and retrieve endpoint and key.
- Use prebuilt-document model ID (already set in the code).

4) Azure OpenAI
- Create deployments for:
  - an Embeddings model (used by TextEmbeddingService)
  - a Chat model (e.g., GPT-4) used by IChatService (GPT_4_Model)
- Store endpoint, key, and deployment names in Key Vault under the names expected by your service classes.

5) Storage
- Create a Storage Account and container(s).
- Store connection details in Key Vault as expected by BlobStorageService.

## Telemetry
- Set APPLICATIONINSIGHTS_CONNECTION_STRING to send logs/metrics/exceptions to Application Insights.
- The app tracks events and request timings for key operations (uploads, processing, Q&A).

## Limitations 

> [!CAUTION]
> The local ONNX reranker increases latency because it performs additional inference over retrieved chunks.  
> Start with a small `VectorTopK`, such as `5`, before increasing it to `50`.

- The API expects an existing Azure AI Search index (documents-index) with a compatible vector field.
- Markdown and plain text files are read directly; binary document formats require Azure Document Intelligence support.- All secrets are resolved at runtime from Key Vault; ensure identities and RBAC are correctly configured.
- Embedding dimensionality must match your index configuration.
- Swagger UI is enabled only in the Development environment.
