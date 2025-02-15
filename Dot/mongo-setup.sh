#!/bin/bash

# Usage: setup-mongo.sh <admin_password> <dot_password>

# Read passwords from arguments
ADMIN_USER="$1"
ADMIN_PASS="$2"
DOT_PASS="$3"

# Define connection parameters
MONGO_CONTAINER="dot-mongo-db"
MONGO_DB="dot"
MONGO_USER="dot-app"

echo "⏳ Waiting for MongoDB to be ready..."

# Wait for MongoDB to start accepting connections
until docker exec -it "$MONGO_CONTAINER" mongosh --quiet --eval "db.adminCommand('ping')" > /dev/null 2>&1; do
    echo "🔄 MongoDB is not ready yet. Retrying..."
    sleep 3
done

echo "✅ MongoDB is up and running!"

# Run MongoDB commands inside the running container
docker exec -it "$MONGO_CONTAINER" mongosh -u root -p "$ADMIN_PASS" --authenticationDatabase admin --eval "
  db = db.getSiblingDB('$MONGO_DB');
  
  // Check if the user already exists
  if (db.getUser('$MONGO_USER') === null) {
      db.createUser({
          user: '$MONGO_USER',
          pwd: '$DOT_PASS',
          roles: [{ role: 'readWrite', db: '$MONGO_DB' }]
      });
      print('✅ MongoDB user \'$MONGO_USER\' created successfully.');
  } else {
      print('ℹ️ MongoDB user \'$MONGO_USER\' already exists. Skipping creation.');
  }
"

echo "✅ Database and user setup completed!"
