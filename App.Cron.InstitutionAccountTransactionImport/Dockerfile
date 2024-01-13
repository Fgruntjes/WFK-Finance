FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY *.sln .
COPY App.Lib.Data/*.csproj ./App.Lib.Data/
COPY App.Lib.Configuration/*.csproj ./App.Lib.Configuration/
COPY App.Lib.ServiceBus/*.csproj ./App.Lib.ServiceBus/

RUN dotnet restore "App.Cron.InstitutionAccountTransactionImport/App.Cron.InstitutionAccountTransactionImport.csproj"
COPY . .

# Build app
RUN dotnet build "App.Cron.InstitutionAccountTransactionImport/App.Cron.InstitutionAccountTransactionImport.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "App.Cron.InstitutionAccountTransactionImport/App.Cron.InstitutionAccountTransactionImport.csproj" -c Release -o /app/publish


FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
COPY appsettings.json ./
ENTRYPOINT ["dotnet", "App.Cron.InstitutionAccountTransactionImport.dll"]
