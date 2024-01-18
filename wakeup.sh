#!/usr/bin/env bash

# Set the maximum duration for the attempts (120 seconds)
MAX_DURATION=120

# The starting time
START_TIME=$(date +%s)

# URL to be requested
URL=$1

# Loop until the time limit is reached
while :; do
    # Perform the curl command
    if curl -s --fail "$URL"; then
        echo ""
        echo "Request succeeded"
        break
    else
        echo "Request failed, trying again"
    fi

    # Check the elapsed time
    CURRENT_TIME=$(date +%s)
    ELAPSED_TIME=$(( CURRENT_TIME - START_TIME ))

    # Exit the loop if the maximum duration is exceeded
    if [ "$ELAPSED_TIME" -ge "$MAX_DURATION" ]; then
        echo "Time limit reached"
        break
    fi

    # Wait for a short period before trying again
    sleep 5
done