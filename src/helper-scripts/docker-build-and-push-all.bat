echo on

set repo_name=###

cd ../Market
 docker image build --file Dockerfile.arm -t %repo_name%/market:latest .
 docker image push  %repo_name%/market:latest

cd ../Strategy
 docker image build --file Dockerfile.arm -t %repo_name%/strategy:latest .
 docker image push  %repo_name%/strategy:latest

 pause >nul

