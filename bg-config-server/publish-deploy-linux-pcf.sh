#!/bin/bash

cf delete bg-config-server -f -r
cf delete bg-config-server-v2-0-0 -f -r

dotnet publish -v d -o ./bin/linux-x64 --framework netcoreapp2.2 --runtime linux-x64 bg-config-server.csproj

cf push -f manifest-bg-config-server.yml
cf push -f manifest-bg-config-server-v2-0-0.yml
