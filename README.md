# About

[![Build status](https://github.com/rgl/HelloSeleniumWebDriver/workflows/Build/badge.svg)](https://github.com/rgl/HelloSeleniumWebDriver/actions?query=workflow%3ABuild)

This is a C# example that runs Chrome/Selenium inside a Windows container.

Also see:

* [rgl/HelloSpecFlowSeleniumWebDriver](https://github.com/rgl/HelloSpecFlowSeleniumWebDriver).
* [rgl/hello-puppeteer-windows-container](https://github.com/rgl/hello-puppeteer-windows-container).

## Caveats

* Chrome only runs inside the [Windows base container image](https://hub.docker.com/_/microsoft-windows) (13.3GB).
  * For more information see the [Container Base Images documentation](https://docs.microsoft.com/en-us/virtualization/windowscontainers/manage-containers/container-base-images).
* Chrome only runs in headless mode.

## Usage

Install docker and docker-compose.

Execute `run.ps1` inside a PowerShell session.

See the contents of the `tmp` directory.

**NB** This was tested in a Windows Server 2019 host. If you are using a different Windows version, you must modify the used container tag inside the [tests](Dockerfile) and [chrome](chrome/Dockerfile) respective `Dockerfile`.

## Troubleshooting

Start the browser container:

```powershell
# see https://chromedevtools.github.io/devtools-protocol/
docker run `
  --rm `
  --publish 9222:9222 `
  --entrypoint 'C:\Program Files\Chromium\Application\chrome.exe' `
  helloseleniumwebdriver_chromium `
  --verbose `
  --headless `
  --disable-gpu `
  --remote-debugging-address=0.0.0.0 `
  --remote-debugging-port=9222
```

In another shell, open the remote devtool UI in a browser:

```powershell
# XXX for some reason, this opens https://chrome-devtools-frontend.appspot.com/
# instead of http://localhost:9222/devtools/inspector.html... why?
start http://localhost:9222 
start http://localhost:9222/json/new?https://en.m.wikipedia.org/wiki/Main_Page
start http://localhost:9222/devtools/inspector.html
```

## Alternatives

* [cypress](https://www.cypress.io/)
* [Playwright](https://playwright.dev/)
* [Puppeteer](https://pptr.dev/)
