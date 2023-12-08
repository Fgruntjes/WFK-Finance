FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY *.sln .
COPY App.Lib.Data/*.csproj ./App.Lib.Data/
COPY App.DataMigrations/*.csproj ./App.DataMigrations/

RUN dotnet restore "App.DataMigrations/App.DataMigrations.csproj"
COPY . .

# Build app
RUN dotnet build "App.DataMigrations/App.DataMigrations.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "App.DataMigrations/App.DataMigrations.csproj" -c Release -o /app/publish


FROM base AS final
WORKDIR /app
COPY --from=build /root/.nuget/packages/microsoft.entityframeworkcore.tools/*/tools/*/any/ef.dll .
COPY --from=publish /app/publish .
COPY ./database.sh .

ENTRYPOINT [ "/app/database.sh" ]
CMD [ "update-assembly" ]