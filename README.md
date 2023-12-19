#Docker file can be run by building

docker build -t testapi .
then running
docker run -p 5007:5007  testapi

I have used Functional Tests i this project by using two different WebApplicationFactories. One use Docker image of SQL (docker compose run in test folder) the other uses EF's InMemory Mock. This Mock
Isnt very good though which is disappointing. It appears to not enforec ref integrity for example.

I use Functional tests over standard Unit tests for most cases as testing internal implentations doesnt really give me much idea if something actually works

This is only very basic. I have made loads of assumptions about how this should work, and this is just an MVP. It is missing a lot. Like pagination logging etc


