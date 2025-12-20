#!/usr/bin/env bash
set -euo pipefail

# Stop Git Bash from rewriting /opt/... into Windows paths for docker commands
export MSYS_NO_PATHCONV=1
export MSYS2_ARG_CONV_EXCL="*"

docker compose -f docker-compose.yml up -d

# Wait until broker responds (no race conditions)
echo "Waiting for Kafka broker..."
for i in {1..60}; do
  if docker exec -i broker bash -lc "/opt/kafka/bin/kafka-topics.sh --bootstrap-server localhost:9092 --list" >/dev/null 2>&1; then
    echo "Kafka is ready."
    break
  fi
  sleep 1
  if [ "$i" -eq 60 ]; then
    echo "Kafka did not become ready in time. Showing broker logs:"
    docker logs --tail 200 broker
    exit 1
  fi
done

bash topics.sh
echo "Kafka up on localhost:9092, UI on http://localhost:8080"
