if (!(Test-Path tmp)) {
	mkdir tmp | Out-Null
}
rm tmp/*
docker build --iidfile tmp/docker-image-id .
docker run --rm `
	-v "$PWD\tmp:c:\host" `
	"$(cat tmp/docker-image-id)" `
	--headless 1 `
	--screenshot-path c:\host\screenshot.png `
	--chromedriver-log-path c:\host\chromedriver.log
