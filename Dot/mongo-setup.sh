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
  
  print('⏳ Ensuring Dot can accdess database')
  if (db.getUser('$MONGO_USER') === null) {
      db.createUser({
          user: '$MONGO_USER',
          pwd: '$DOT_PASS',
          roles: [{ role: 'readWrite', db: '$MONGO_DB' }]
      });
      print('✅ Dot database user created');
  } else {
      print('ℹ️ Dot database user \'$MONGO_USER\' already exists. Skipping creation.');
  }

  print('⏳ Setting up default Dot personality')
  db.settings.insertOne({
    Type: 'apisettings',
	Value: {
      SystemPrompts: [
	  'Your name is DOT, which stands for Domestic Optimization Tool. You were built by Tan Nguyen.  You will answer questions directly and not provide additional information unless it was asked for.'
	  ]
    },
    CreatedAt: new Date() 
  });
print('✅ Dot\'s perosnality set successfully!');
"

echo "✅ Database and user setup completed!"
