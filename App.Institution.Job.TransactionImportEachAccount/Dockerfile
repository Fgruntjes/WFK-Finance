FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY *.sln .
COPY App.Lib.Data/*.csproj ./App.Lib.Data/
COPY App.Lib.Configuration/*.csproj ./App.Lib.Configuration/
COPY App.Lib.ServiceBus/*.csproj ./App.Lib.ServiceBus/
COPY App.Institution.Job.TransactionImportEachAccount/*.csproj ./App.Institution.Job.TransactionImportEachAccount/

RUN dotnet restore "App.Institution.Job.TransactionImportEachAccount/App.Institution.Job.TransactionImportEachAccount.csproj"
COPY . .

# Build app
RUN dotnet build "App.Institution.Job.TransactionImportEachAccount/App.Institution.Job.TransactionImportEachAccount.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "App.Institution.Job.TransactionImportEachAccount/App.Institution.Job.TransactionImportEachAccount.csproj" -c Release -o /app/publish


FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
COPY appsettings.json ./
ENTRYPOINT ["dotnet", "App.Institution.Job.TransactionImportEachAccount.dll"]
