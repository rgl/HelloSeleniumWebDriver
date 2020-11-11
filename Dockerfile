# escape=`
# see https://hub.docker.com/_/microsoft-dotnet-core-sdk
# see https://github.com/dotnet/dotnet-docker/blob/master/src/sdk/3.1/nanoserver-1809/amd64/Dockerfile
FROM mcr.microsoft.com/dotnet/core/sdk:3.1
WORKDIR c:/build
COPY *.csproj .
RUN dotnet restore
COPY . .
RUN dotnet build --no-restore --configuration Release
ENTRYPOINT ["c:/build/bin/Release/netcoreapp3.1/HelloSeleniumWebDriver.exe"]
