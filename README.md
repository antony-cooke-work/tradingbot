# tradingbot
## ADDING SSL TO CONTAINERS (inital experiement)
1. dotnet dev-certs https --clean
2. dotnet dev-certs https -ep .aspnet\https\aspnetapp.pfx -p {YourPassword}
3. dotnet dev-certs https --trust