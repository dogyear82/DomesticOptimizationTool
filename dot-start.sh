#!/bin/bash

read -p "Enter a password for Dot to access dependent resources: " DOT_PASS
echo ""
export DOT_USER_NAME="dot-app"
export DOT_PASS

read -p "Enter an administrator password: " ADMIN_PASS
echo ""
export ADMIN_USER_NAME="root"
export ADMIN_PASS

#read -p "Enter your huggingface secret token: " HUGGINGFACE_TOKEN
#echo ""
#export HUGGINGFACE_TOKEN

export MONGO_DB_NAME="dot"
export MONGO_CONTAINER_NAME="dot-mongo-db"
export RABBIT_CONTAINER_NAME="dot-rabbitmq"
docker compose up -d

# Setup MongoDB
bash mongo-setup.sh "$ADMIN_USER_NAME" "$ADMIN_PASS" "$DOT_USER_NAME" "$DOT_PASS" "$MONGO_DB_NAME" "$MONGO_CONTAINER_NAME"

# Setup RabbitMQ
#bash rabbitmq-setup.sh "$ADMIN_USER_NAME" "$ADMIN_PASS" "$DOT_USER_NAME" "$DOT_PASS" "$RABBIT_CONTAINER_NAME"