#!/usr/bin/env bash
dotnet restore src/services/FH.ParcelLogistics.Services/ && \
    dotnet build src/services/FH.ParcelLogistics.Services/ && \
    dotnet build src/logic/FH.ParcelLogistics.BusinessLogic/ && \
    echo "Starting FH.ParcelLogistics.Services on localhost:8080" && \
    dotnet run --project src/services/FH.ParcelLogistics.Services/FH.ParcelLogistics.Services.csproj
