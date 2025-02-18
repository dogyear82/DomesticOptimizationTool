#!/bin/bash

# Define credentials
RABBITMQ_ADMIN_USER="$1"
RABBITMQ_ADMIN_PASS="$2"
RABBITMQ_USER="$3"
RABBITMQ_PASS="$4"
RABBITMQ_CONTAINER="$5"

echo "⏳ Waiting for RabbitMQ to start..."

# Wait until RabbitMQ is ready
until docker exec "$RABBITMQ_CONTAINER" rabbitmqctl status > /dev/null 2>&1; do
    echo "🔄 Waiting for RabbitMQ to be available..."
    sleep 3
done

echo "✅ RabbitMQ is up and running!"

# Check if the user already exists
EXISTING_USER=$(docker exec "$RABBITMQ_CONTAINER" rabbitmqctl list_users | grep -w "$RABBITMQ_USER")

if [ -z "$EXISTING_USER" ]; then
    echo "👤 Creating RabbitMQ user '$RABBITMQ_USER'..."
    
    docker exec "$RABBITMQ_CONTAINER" rabbitmqctl add_user "$RABBITMQ_USER" "$RABBITMQ_PASS"
    docker exec "$RABBITMQ_CONTAINER" rabbitmqctl set_user_tags "$RABBITMQ_USER" management
    docker exec "$RABBITMQ_CONTAINER" rabbitmqctl set_permissions -p / "$RABBITMQ_USER" ".*" ".*" ".*"

    echo "✅ User '$RABBITMQ_USER' created successfully!"
else
    echo "ℹ️ User '$RABBITMQ_USER' already exists."
fi

echo "🚀 RabbitMQ setup complete!"
