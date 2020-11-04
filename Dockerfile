# escape=`
# see https://hub.docker.com/_/microsoft-dotnet-core-sdk
# see https://github.com/dotnet/dotnet-docker/blob/master/src/sdk/3.1/nanoserver-1809/amd64/Dockerfile
FROM mcr.microsoft.com/dotnet/core/sdk:3.1 as build
WORKDIR c:/build
COPY *.csproj .
RUN dotnet restore `
        --runtime win10-x64
COPY . .
RUN dotnet publish `
        --runtime win10-x64 `
        --configuration Release `
        --no-restore `
        --self-contained `
        --output bin/publish && `
    xcopy bin\Release\netcoreapp3.1\win10-x64\chromedriver.exe bin\publish

#FROM mcr.microsoft.com/windows/servercore:1809 # NB does not work in this base image.
FROM mcr.microsoft.com/windows:1809
SHELL ["powershell.exe", "-Command", "$ErrorActionPreference = 'Stop'; $ProgressPreference = 'SilentlyContinue';"]
WORKDIR c:/app
RUN Set-ExecutionPolicy -ExecutionPolicy Bypass -Force; `
    Invoke-Expression (New-Object System.Net.WebClient).DownloadString('https://chocolatey.org/install.ps1'); `
    choco feature disable -name showDownloadProgress
# NB there's a big caveat with this package, it will always install the
#    latest version regardless of the package version because google chrome
#    does not have a version specific download address.
# TODO switch to chromium, maybe an ungoogled version.
#      see https://github.com/Hibbiki/chromium-win32
#      see https://chocolatey.org/packages/chromium/86.0.4240.111#files
RUN choco install -y GoogleChrome
COPY --from=build c:/build/bin/publish .
ENTRYPOINT ["HelloSeleniumWebDriver.exe"]
