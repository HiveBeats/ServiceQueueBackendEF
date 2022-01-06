#!/bin/bash
#check for env
filename=.env
if [ -f "$filename" ];
then
#stop old compose
if echo "Stopping docker" && docker-compose down; then
    #update version
    if echo "Updating packages" && sh ./update-full.sh; then
        #build frontend
        if echo "Building frontend" && cd ./Frontend && sh ./build-prod.sh && cd ../; then
            #up compose
            echo "Running docker" && docker-compose up -d
        fi
    fi
fi
else
echo "env file has not been found"
fi
