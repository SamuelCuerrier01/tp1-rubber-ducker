#!/bin/bash
echo "Starting DuckLog local environment..."

cd ../../

dotnet tool update --global dotnet-ef

docker-compose down -v
docker-compose up -d --build

echo "Waiting 5 seconds for MySQL to wake up..."
sleep 5

cd src/BackendApi
rm -rf obj bin

dotnet clean
dotnet restore BackendApi.csproj --force

dotnet ef database update --project BackendApi.csproj

echo "Database is ready! You can now run the API."