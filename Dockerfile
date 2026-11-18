# Build
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY Mixtape/Mixtape.csproj Mixtape/
COPY Mixtape.Client/Mixtape.Client.csproj Mixtape.Client/
COPY Mixtape.Data/Mixtape.Data.csproj Mixtape.Data/
COPY Mixtape.Shared/Mixtape.Shared.csproj Mixtape.Shared/
COPY Mixtape.Application/Mixtape.Application.csproj Mixtape.Application/
RUN dotnet restore Mixtape/Mixtape.csproj

COPY . .
RUN dotnet publish Mixtape/Mixtape.csproj -c Release -o /app

# Run
FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /app .

# Folder for the keys that protect the session cookie. A volume on the server keeps it between deploys.
RUN mkdir -p /home/app/.aspnet/DataProtection-Keys && chown -R $APP_UID /home/app/.aspnet

USER $APP_UID
EXPOSE 8080
ENTRYPOINT ["dotnet", "Mixtape.dll"]
