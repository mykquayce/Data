#! /bin/bash

docker image ls | tail --line +2 | awk '{system("docker pull " $1 ":" $2)}'

if [ $? -ne 0 ]; then
    exit 1
fi

docker build --tag eassbhhtgu/data:latest .

if [ $? -ne 0 ]; then
    exit 1
fi

docker push eassbhhtgu/data:latest

if [ $? -ne 0 ]; then
    exit 1
fi

docker stack ls | tail --line +2 | grep 'data'

if [ $? -ne 0 ]; then
    docker stack deploy --compose-file docker-compose.yml data
else
    docker service ls | tail --line +2 | grep 'data_data' | awk '{system("docker service update --image " $5 " " $2)}'
fi

if [ $? -ne 0 ]; then
    exit 1
fi

docker container ls -a | tail --line +2 | grep 'eassbhhtgu/data:latest' | grep 'Exited (0)' | awk '{system("docker rm " $1)}'

if [ $? -ne 0 ]; then
    exit 1
fi

docker image ls | tail --line +2 | grep '<none>' | awk '{system("docker rmi " $3)}'
