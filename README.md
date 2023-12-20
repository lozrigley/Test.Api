Docker file can be run by building

docker build -t testapi .

then running

docker run -p 5007:5007 -e "DOTNET_RUNNING_IN_CONTAINER=true"  testapi

This env variable is required so it used the correct connection string for docker which has Server=host.docker.internal instead of localhost


I have used Functional Tests in this project by using two different WebApplicationFactories. One uses Docker image of SQL (docker compose run in Test.Api/test folder) the other uses EF's InMemory Mock. This Mock
Isnt very good though which is disappointing. It appears to not enforce ref integrity for example.

I use Functional tests over standard Unit tests for most cases as testing internal implentations doesnt really give me much idea if something actually works

This is only very basic. I have made loads of assumptions about how this should work, and this is just an MVP. It is missing a lot. Like pagination logging etc


