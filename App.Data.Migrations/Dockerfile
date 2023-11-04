FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY *.sln .
COPY App.Data/*.csproj ./App.Data/
COPY App.Data.Migrations/*.csproj ./App.Data.Migrations/

RUN dotnet restore "App.Data.Migrations/App.Data.Migrations.csproj"
COPY . .

# Build app
RUN dotnet build "App.Data.Migrations/App.Data.Migrations.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "App.Data.Migrations/App.Data.Migrations.csproj" -c Release -o /app/publish


FROM base AS final
WORKDIR /app
COPY --from=build /root/.nuget/packages/microsoft.entityframeworkcore.tools/*/tools/*/any/ef.dll .
COPY --from=publish /app/publish .

ENTRYPOINT [ "/app/migrate.sh" ]
CMD [ "update-assembly" ]