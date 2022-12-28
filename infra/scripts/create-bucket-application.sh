#!/bin/bash

# Protects script from continuing with an error
set -eu -o pipefail

# Create bucket to store active application
influx bucket create \
    --name ${DOCKER_INFLUXDB_APPLICATION_BUCKET} \
    --org ${DOCKER_INFLUXDB_INIT_ORG} \
    --retention ${DOCKER_INFLUXDB_APPLICATION_RETENTION}
