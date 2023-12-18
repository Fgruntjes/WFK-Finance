FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY *.sln .
COPY App.Lib.Data/*.csproj ./App.Lib.Data/
COPY App.Lib.Configuration/*.csproj ./App.Lib.Configuration/
COPY App.Lib.InstitutionConnection/*.csproj ./App.Lib.InstitutionConnection/
COPY App.Backend/*.csproj ./App.Backend/

RUN dotnet restore "App.Backend/App.Backend.csproj"
COPY . .

# Build app
RUN dotnet build "App.Backend/App.Backend.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "App.Backend/App.Backend.csproj" -c Release -o /app/publish


FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "App.Backend.dll"]
