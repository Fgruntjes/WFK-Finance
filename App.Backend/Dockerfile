FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["App.Backend.csproj", "."]
RUN dotnet restore "App.Backend.csproj"
COPY . .
RUN dotnet build "App.Backend.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "App.Backend.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "App.Backend.dll"]
