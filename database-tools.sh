#!/bin/bash

cd "$(dirname "$0")"

# TODO: Should no longer clean all migrations, instead just run the latest once we have some production data.

function cleanup {
    echo "### Cleaning database"
    docker-compose down \
        --remove-orphans \
        --volumes

    echo "### Starting database"
    docker-compose up -d
}

function newMigration {
    MIGRATION_NAME=$1
    echo "### Creating new migrations"
    dotnet ef migrations add \
        "${MIGRATION_NAME}" \
        --project App.Backend \
        --output-dir src/Data/Migrations
}

function updateDatabase {
    echo "### Updating database"
    dotnet ef database update \
        --project App.Data.Migrations
}

function updateDatabaseDocker {
    echo "### Updating database using docker container"
    # Command used for local testing of running database migrations from final assembly
    # Used in terraform/backend_database_migrations.tf
    docker run --entrypoint="dotnet" \
        --env "ConnectionStrings__DefaultConnection=Server=host.docker.internal,1433; Database=development; User Id=sa; Password=myLeet123Password!; Encrypt=False" \
        --add-host=host.docker.internal:host-gateway \
        app.data.migrations \
        exec \
        --runtimeconfig ./App.Data.Migrations.runtimeconfig.json \
        --depsfile ./App.Data.Migrations.deps.json ef.dll \
        --verbose database update --context App.Data.DatabaseContext \
        --assembly ./App.Data.Migrations.dll  \
        --startup-assembly ./App.Data.Migrations.dll \
        --data-dir ./
}

updateDatabaseDocker