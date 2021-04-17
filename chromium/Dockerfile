# escape=`
#FROM mcr.microsoft.com/windows/servercore:1809 # NB does not work in this base image.
FROM mcr.microsoft.com/windows:1809
SHELL ["powershell.exe", "-Command", "$ErrorActionPreference = 'Stop'; $ProgressPreference = 'SilentlyContinue';"]
WORKDIR c:/app
RUN Set-ExecutionPolicy -ExecutionPolicy Bypass -Force; `
    Invoke-Expression (New-Object System.Net.WebClient).DownloadString('https://chocolatey.org/install.ps1'); `
    choco feature disable -name showDownloadProgress
# TODO maybe use an ungoogled version.
#      see https://github.com/Hibbiki/chromium-win32
RUN choco install -y chromium --version 86.0.4240.198
RUN choco install -y chromedriver --version 86.0.4240.220
EXPOSE 9515
ENTRYPOINT ["chromedriver.exe", "--port=9515", "--whitelisted-ips"]
