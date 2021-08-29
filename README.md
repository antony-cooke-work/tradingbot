# tradingbot
## Adding SSL to containers (inital experiement)
1. dotnet dev-certs https --clean
2. dotnet dev-certs https -ep .aspnet\https\aspnetapp.pfx -p {YourPassword}
3. dotnet dev-certs https --trust

## Uploading to repo
1. docker image build . --tag dockerhubname/market:latest
2. docker image push dockerhubname/market:latest

## Useful cnds for Pi
- docker-compose pull
- docker-compose -f docker-compose.yaml up -d
- docker-compose ps
- docker-compose logs servicename
- docker-compose logs --tail="all"
- curl http://localhost:5101/markets/BTCGBP

