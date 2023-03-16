# Facial Recognition

## Introduction

This repository demonstrates a microservices architecture backed by dapr to utilize 
AWS Rekogntion Services for facial detection and prediction of ages.

## Prerequisites

- [dapr](https://docs.dapr.io/getting-started/install-dapr-cli/)
- [docker](https://docs.docker.com/get-docker/)
- [make](https://gnuwin32.sourceforge.net/packages/make.htm)
- [.NET CLI](https://learn.microsoft.com/en-us/dotnet/core/tools/)
- [Entity Framework Tools](https://learn.microsoft.com/en-us/ef/core/cli/dotnet)

## Build and Run the Facial Recognition Application

1. Run `dapr init` to initialize dapr as well as the redis and zipkin docker containers.
2. Run `docker run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=password1!" -p 1455:1433 --name DaprOrderSql -d mcr.microsoft.com/mssql/server:2019-latest` to spin up the MSSQL container.
3. Run `docker run -p 4000:1080 -p 4025:1025 --name fds-maildev maildev/maildev:latest` to spin up the maildev container.
4. Run `dotnet ef database update -c OrdersContext -p OrdersApi -s OrdersApi` to update the MSSQL database schema.
5. Run `make build` to run all microservices with dapr as well as the dapr dashboard.

## Additional Information

- The main application runs on `localhost:5002`
- The dapr dashboard runs on `localhost:8080`
- The zipkin dashboard runs on `localhost:9411`
- The maildev dashboard runs on `localhost:4000`