# Build
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY Mixtape/Mixtape.csproj Mixtape/
COPY Mixtape.Client/Mixtape.Client.csproj Mixtape.Client/
RUN dotnet restore Mixtape/Mixtape.csproj

COPY . .
RUN dotnet publish Mixtape/Mixtape.csproj -c Release -o /app

# Run
FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /app .

USER $APP_UID
EXPOSE 8080
ENTRYPOINT ["dotnet", "Mixtape.dll"]
