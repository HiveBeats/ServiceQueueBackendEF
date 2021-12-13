#!/bin/bash

#stop old compose
echo "Stopping docker" 
docker compose down

#update version
echo "Updating packages"
sh ./update-full.sh

#build frontend
echo "Building frontend"
sh ./Frontend/build.sh

#up compose
echo "Running docker"
docker compose up

