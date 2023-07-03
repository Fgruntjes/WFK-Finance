FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /source

# copy csproj and restore as distinct layers
COPY *.csproj .
RUN dotnet restore --use-current-runtime

# copy and publish app and libraries
COPY . .
RUN dotnet publish --use-current-runtime --self-contained false --no-restore -o /app


# final stage/image
FROM mcr.microsoft.com/dotnet/aspnet:7.0
WORKDIR /app
RUN addgroup --gid 1000 app && \
    adduser --uid 1000 --ingroup app --home /app --shell /bin/sh --disabled-password --gecos "" app
USER app:app
COPY --from=build /app .
EXPOSE 8080
ENTRYPOINT ["dotnet", "App.Backend.dll"]