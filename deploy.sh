#!/bin/bash
#check for env
filename=.env
if [ -f "$filename" ]; then
    until $(curl https://raw.githubusercontent.com/HiveBeats/denvlate/main/release/denver --output denver); do
        echo "."
        sleep 5
    done
    chmod +x ./denver
    ./denver -e .env -t ./sqlinit/01-databases.sql
    if ! type "docker-compose -h" > /dev/null; then
        echo "new version compose not found, trying the old one";
        if echo "Stopping docker" && docker compose down; then
            #update version
            if echo "Updating packages" && sh ./update-full.sh; then
                #build frontend
                if echo "Building frontend" && cd ./Frontend && sh ./build-prod.sh && cd ../; then
                    #up compose
                    echo "Running docker" && docker compose up -d
                fi
            fi
        fi
    else
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
    fi
else
    echo "env file has not been found"
fi
