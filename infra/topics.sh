#!/usr/bin/env bash
set -euo pipefail

# fix windows file paths when using MSYS2 to run bash scripts
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

create "userMessages" 1

docker exec -i broker bash -lc "/opt/kafka/bin/kafka-topics.sh --bootstrap-server '$BOOTSTRAP' --list"
