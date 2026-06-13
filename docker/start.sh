#!/bin/sh
set -e

# Start nginx in foreground
nginx -g "daemon off;" &
NGINX_PID=$!

# Start dotnet backend
cd /app
dotnet ZJAdmin.Api.dll &
DOTNET_PID=$!

# Handle shutdown
trap "kill $NGINX_PID $DOTNET_PID; exit" SIGTERM SIGINT

# Wait for either process to exit
wait $DOTNET_PID
