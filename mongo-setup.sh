#!/bin/bash

# Usage: setup-mongo.sh <admin_password> <dot_password>

# Read passwords from arguments
ADMIN_USER="$1"
ADMIN_PASS="$2"
DOT_USER_NAME="$3"
DOT_PASS="$4"
MONGO_DB="$5"
MONGO_CONTAINER="$6"

echo "⏳ Waiting for MongoDB to start..."

# Wait for MongoDB to start accepting connections
until docker exec -it "$MONGO_CONTAINER" mongosh --quiet --eval "db.adminCommand('ping')" > /dev/null 2>&1; do
    echo "🔄 MongoDB is not ready yet. Retrying..."
    sleep 3
done

echo "✅ MongoDB is up and running!"

# Run MongoDB commands inside the running container
docker exec -it "$MONGO_CONTAINER" mongosh -u root -p "$ADMIN_PASS" --authenticationDatabase admin --eval "
  db = db.getSiblingDB('$MONGO_DB');
  
  print('⏳ Checking Dot MongoDB credentials...');
  if (db.getUser('$DOT_USER_NAME') === null) {
      print('👤 Creating MongoDB user \'$DOT_USER_NAME\'...')
      db.createUser({
          user: '$DOT_USER_NAME',
          pwd: '$DOT_PASS',
          roles: [{ role: 'readWrite', db: '$MONGO_DB' }]
      });
      print('✅ Dot MongoDB credentials created');
  } else {
      print('ℹ️ Dot MongoDB credentials \'$DOT_USER_NAME\' already exists.');
  }
    print('✅ Dot MongoDB credentials are ready!')

  print('⏳ Checking Dot\'s system settings...');
  if (db.settings.countDocuments({}) === 0) {
    print('ℹ️ Setting Dot\'s system settings...');
    db.settings.insertOne({
      Type: 'apisettings',
	  Value: {
        SystemPrompts: [
	    'Your name is DOT, which stands for Domestic Optimization Tool. You were built by Tan Nguyen.  You will answer questions directly and not provide   additional information unless it was asked for.'
	    ]
      },
      CreatedAt: new Date() 
    });
  }
print('✅ Dot\'s system settings ready');
"

echo "✅ Database and user setup completed!"
