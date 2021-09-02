echo on

set repo_name=antonycookework

cd ../
cd ./Market
 docker image build -f ./Dockerfile.arm -t %repo_name%/market:latest .
 docker image push  %repo_name%/market:latest

cd ../
cd ./Strategy
 docker image build -f ./Dockerfile.arm -t %repo_name%/strategy:latest .
 docker image push  %repo_name%/strategy:latest

 pause >nul

