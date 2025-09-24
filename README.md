# StorageService Solution

## Overview

StorageService is a C# gRPC-based dockerized microservice for generating secure upload URLs and validating file uploads. It provides endpoints for clients to request upload URLs and verify file existence.

## Features

- Generate signed upload URLs for file uploads
- Validate if a file exists by its ID
  
## Technologies

- .NET 8 (C#)
- gRPC
- Docker

## Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- [Grpc.Tools](https://www.nuget.org/packages/Grpc.Tools/)

### Configuration

the default key "storage-service-secret-2024" is used.

```bash
