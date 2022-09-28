#!/usr/bin/env bash

# check if jq is installed for formatting
if ! [ -x "$(command -v jq)" ]; then
  echo 'Error: jq is not installed. It is used to properly format reponses of the API requests.' >&2
  echo '    sudo apt install jq' >&2
  exit 1
fi

echo "Testing LogicticsPartnerAPI, should return trackingID PYJRB4HZ6"
echo "POST /parcel/{trackingId}"
curl -X 'POST' \
  'http://localhost:8080/parcel/PYJRB4HZ6' \
  -H 'accept: text/plain' \
  -H 'Content-Type: application/json' \
  -d '{
        "weight": 0,
        "recipient": {
            "name": "string",
            "street": "string",
            "postalCode": "string",
            "city": "string",
            "country": "string"
        },
        "sender": {
            "name": "string",
            "street": "string",
            "postalCode": "string",
            "city": "string",
            "country": "string"
        }
    }' | jq
echo ""

echo "Testing LogisticsPartner API, should fail due to empty request body"
echo "POST /parcel/{trackingId}"
curl -X 'POST' \
  'http://localhost:8080/parcel/PYJRB4HZ6' \
  -H 'accept: text/plain' \
  -H 'Content-Type: application/json' \
  -d '{}' | jq
echo 

echo "Testing RecipientAPI, should return tracking information"
echo "GET /parcel/{trackingId}"
curl -X 'GET' \
  'http://localhost:8080/parcel/PYJRB4HZ6' \
  -H 'accept: text/plain' | jq
echo 

echo "Testing SenderAPI, should return trackingID of new parcel"
echo "POST /parcel"
curl -X 'POST' \
  'http://localhost:8080/parcel' \
  -H 'accept: text/plain' \
  -H 'Content-Type: application/json' \
  -d '{
  "weight": 0,
  "recipient": {
    "name": "string",
    "street": "string",
    "postalCode": "string",
    "city": "string",
    "country": "string"
  },
  "sender": {
    "name": "string",
    "street": "string",
    "postalCode": "string",
    "city": "string",
    "country": "string"
  }
}' | jq
echo

echo "Testing SenderAPI, should fail due to missing sender information"
echo "POST /parcel"
curl -X 'POST' \
  'http://localhost:8080/parcel' \
  -H 'accept: text/plain' \
  -H 'Content-Type: application/json' \
  -d '{
  "weight": 0,
  "recipient": {
    "name": "string",
    "street": "string",
    "postalCode": "string",
    "city": "string",
    "country": "string"
  }
}' | jq
echo

echo "Testing StaffAPI, should return successful response"
echo "POST /parcel/{trackingId}/reportDelivery"
curl -X 'POST' \
  'http://localhost:8080/parcel/PYJRB4HZ6/reportDelivery' \
  -H 'accept: */*' \
  -d '' | jq
echo 

echo "Testing StaffAPI, should return successful response"
echo "POST /parcel/{trackingId}/reportHop/{code}"
curl -X 'POST' \
  'http://localhost:8080/parcel/PYJRB4HZ6/reportHop/CODE1' \
  -H 'accept: */*' \
  -d '' | jq
echo

echo "Testing WarehouseManagementAPI, should load successfully"
curl -X 'POST' \
    'http://localhost:8080/warehouse' \
    -H 'accept: */*' -H 'Content-Type: application/json' \
    --data-binary @data-full.json | jq
echo

echo "Testing WarehouseManagementAPI, should return warehouse information"
echo "GET /warehouse"
curl -X 'GET' \
  'http://localhost:8080/warehouse' \
  -H 'accept: text/plain' | jq
echo

echo "Testing WarehouseManagementAPI, should return warehouse information"
echo "GET /warehouse/{code}"
curl -X 'GET' \
  'http://localhost:8080/warehouse/CODE1' \
  -H 'accept: text/plain' | jq
echo 