#!/usr/bin/env bash
dotnet restore src/FH.ParcelLogistics.Services/ && \
    dotnet build src/FH.ParcelLogistics.Services/ && \
    echo "Starting FH.ParcelLogistics.Services on localhost:8080" && \
    dotnet run --project src/FH.ParcelLogistics.Services/FH.ParcelLogistics.Services.csproj
