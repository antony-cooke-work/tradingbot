echo on

set repo_name=****

cd ../
cd ./Market
 docker image build . --tag %repo_name%/market:latest
 docker image push  %repo_name%/market:latest

cd ../
cd ./Strategy
 docker image build . --tag  %repo_name%/strategy:latest
 docker image push  %repo_name%/strategy:latest

