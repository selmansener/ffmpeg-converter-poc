
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /App
ARG BUILD_CONFIGURATION=Release

# Copy everything
COPY . ./
# Restore as distinct layers
RUN dotnet restore
# Build and publish a release
RUN dotnet publish -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0

RUN apt update && apt install -y ffmpeg

EXPOSE 8080:8080

WORKDIR /App
COPY --from=build-env /App/out .
ENTRYPOINT ["dotnet", "SpawnProcessOnUbuntu.dll"]