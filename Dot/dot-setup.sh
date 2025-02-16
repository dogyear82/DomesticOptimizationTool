#!/bin/bash

# Prompt the user for MongoDB credentials
read -s -p "Enter a password for Dot to dependent resources: " DOT_PASS
echo ""
read -s -p "Enter an administrator password: " ADMIN_PASS
echo ""

# Define user variables
ADMIN_USER_NAME="root"

echo "🔒 Passwords are temporarily stored in environment variables."

# Run Docker Compose with the necessary environment variables
export MONGO_ADMIN_USER="$ADMIN_USER_NAME" 
export MONGO_ADMIN_PASS="$ADMIN_PASS" 
docker compose up -d

# Call setup-mongo.sh and pass the passwords as arguments
bash mongo-setup.sh "$ADMIN_USER_NAME" "$ADMIN_PASS" "$DOT_PASS"