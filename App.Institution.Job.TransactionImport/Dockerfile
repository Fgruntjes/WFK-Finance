FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY *.sln .
COPY App.Lib.Data/*.csproj ./App.Lib.Data/
COPY App.Lib.Configuration/*.csproj ./App.Lib.Configuration/
COPY App.Lib.ServiceBus/*.csproj ./App.Lib.ServiceBus/
COPY App.Institution.Job.TransactionImport/*.csproj ./App.Institution.Job.TransactionImport/

RUN dotnet restore "App.Institution.Job.TransactionImport/App.Institution.Job.TransactionImport.csproj"
COPY . .

# Build app
RUN dotnet build "App.Institution.Job.TransactionImport/App.Institution.Job.TransactionImport.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "App.Institution.Job.TransactionImport/App.Institution.Job.TransactionImport.csproj" -c Release -o /app/publish


FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
COPY appsettings.json ./
ENTRYPOINT ["dotnet", "App.Institution.Job.TransactionImport.dll"]
