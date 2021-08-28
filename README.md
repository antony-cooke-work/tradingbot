# tradingbot
## Adding SSL to containers (inital experiement)
1. dotnet dev-certs https --clean
2. dotnet dev-certs https -ep .aspnet\https\aspnetapp.pfx -p {YourPassword}
3. dotnet dev-certs https --trust

## Uploading to repo
1. docker image build . --tag dockerhubname/market:latest
2. docker image push dockerhubname/market:latest
