# escape=`
# see https://hub.docker.com/_/microsoft-dotnet-sdk
# see https://github.com/dotnet/dotnet-docker/blob/master/src/sdk/6.0/nanoserver-1809/amd64/Dockerfile
FROM mcr.microsoft.com/dotnet/sdk:6.0
WORKDIR c:/build
COPY *.csproj .
RUN dotnet restore
COPY . .
RUN dotnet build --no-restore --configuration Release
ENTRYPOINT ["c:/build/bin/Release/net6.0/HelloSeleniumWebDriver.exe"]
