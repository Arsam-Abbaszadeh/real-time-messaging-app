#!/usr/bin/env bash
set -euo pipefail

export MSYS_NO_PATHCONV=1
export MSYS2_ARG_CONV_EXCL="*"

BOOTSTRAP="localhost:9092"

create () {
  local topic="$1"
  local partitions="$2"

  docker exec -i broker bash -lc \
    "/opt/kafka/bin/kafka-topics.sh \
      --bootstrap-server '$BOOTSTRAP' \
      --create --if-not-exists \
      --replication-factor 1 \
      --partitions '$partitions' \
      --topic '$topic'"
}

create "myproject.orders-created" 3
create "myproject.email-send" 1

docker exec -i broker bash -lc "/opt/kafka/bin/kafka-topics.sh --bootstrap-server '$BOOTSTRAP' --list"
