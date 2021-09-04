# tradingbot
## Adding SSL to containers (inital experiement)
1. dotnet dev-certs https --clean
2. dotnet dev-certs https -ep .aspnet\https\aspnetapp.pfx -p {YourPassword}
3. dotnet dev-certs https --trust

## Uploading to repo
1. docker image build . --tag dockerhubname/market:latest
2. docker image push dockerhubname/market:latest

## Useful cmds for Pi
- docker-compose pull
- docker-compose -f docker-compose.yaml up -d
- docker-compose ps
- docker-compose logs servicename
- docker-compose logs --tail="all"
- curl http://localhost:5101/prices/BTCGBP
- docker run --rm -it --entrypoint=/bin/bash dockerhubname/strategy
- docker-compose down
- docker-compose rm

### Delete all containers
- docker rm -f $(docker ps -a -q)

### Delete all volumes
- docker volume rm $(docker volume ls -q)

## Useful cmds for InfluxDB
- influx delete --org tradingbot --bucket market --start '1970-01-01T00:00:00Z'  --stop $(date +"%Y-%m-%dT%H:%M:%SZ")
