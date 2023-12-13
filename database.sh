#!/bin/bash

cd "$(dirname "$0")"

set +x
set -e

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
    if [[ -z "${MIGRATION_NAME}" ]]; then
        echo "Usage: database-tools.sh new-migration <migration-name>"
        exit 1
    fi
    echo "### Creating new migrations"
    dotnet ef migrations add \
        "${MIGRATION_NAME}" \
        --project App.DataMigrations \
        --output-dir src/Migrations
}

function updateDatabase {
    echo "### Updating database"
    dotnet ef database update \
        --project App.DataMigrations
}

function updateFromAssembly {
    echo "### Updating database from assembly"
    dotnet exec \
        --runtimeconfig "./App.DataMigrations.runtimeconfig.json" \
        --depsfile "./App.DataMigrations.deps.json" "ef.dll" \
        --verbose database update \
        --context "App.Lib.Data.DatabaseContext" \
        --assembly "./App.DataMigrations.dll" \
        --startup-assembly "./App.DataMigrations.dll" \
        --data-dir "./"
}

function updateDatabaseDocker {
    echo "### Updating database using docker container"
    # Command used for local testing of running database migrations from final assembly
    # Used in terraform/backend_database_migrations.tf
    docker run \
        --env "ConnectionStrings__DefaultConnection=Server=host.docker.internal,1433; Database=development; User Id=sa; Password=myLeet123Password!; Encrypt=False" \
        --add-host=host.docker.internal:host-gateway \
        app.datamigrations
}

ACTION=${1:-"update"}
if [[ "${ACTION}" == "cleanup" ]]; then
    cleanup
elif [[ "${ACTION}" == "new-migration" ]]; then
    newMigration "${@:2}"
elif [[ "${ACTION}" == "update" ]]; then
    updateDatabase "${@:2}"
elif [[ "${ACTION}" == "update-assembly" ]]; then
    updateFromAssembly "${@:2}"
elif [[ "${ACTION}" == "update-docker" ]]; then
    updateDatabaseDocker "${@:2}"
else
    echo "Supported actions: database-tools.sh {update, update-docker, cleanup, new-migration}"
fi
