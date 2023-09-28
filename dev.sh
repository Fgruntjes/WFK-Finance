#!/bin/bash

cd "$(dirname "$0")"

# Cleanup when exit
function clean_up {
    echo "### Cleanup started"
    if [[ $count -eq 0 ]]; then
        pkill -P $$
        count=1
    elif [[ $count -eq 1 ]]; then
        pkill -P $$
        count=2
    else
        pkill -9 -P $$
        exit
    fi
    echo "### Cleanup finished"
}

count=0
trap clean_up SIGINT

# start docker containers
docker-compose up -d

function run_backend {
    echo "### Starting backend"
    dotnet watch run \
        --project App.Backend \
        --non-interactive \
        ./App.Backend/App.Backend.csproj \
        /property:GenerateFullPaths=true \
        /consoleloggerparameters:NoSummary
    echo "### Frontend backend"
}

function run_frontend {
    echo "### Starting frontend"
    cd frontend
    npm run dev
    echo "### Frontend closed"

    kill -TERM -$$
}

run_backend &
sleep 2
run_frontend &
wait