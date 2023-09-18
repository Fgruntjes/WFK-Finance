#!/bin/bash

cd "$(dirname "$0")"

# TODO: Should no longer clean all migrations, instead just run the latest once we have some production data.

echo "### Cleaning migrations"
rm -Rvf App.Backend/src/Data/Migrations/*

echo "### Cleaning database"
docker-compose down \
    --remove-orphans \
    --volumes

echo "### Starting database"
docker-compose up -d

echo "### Creating new migrations"
dotnet ef migrations add \
    InitialCreate \
    --project App.Backend \
    --output-dir src/Data/Migrations
dotnet ef database update \
    --project App.Backend