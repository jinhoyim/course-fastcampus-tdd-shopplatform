#!/bin/bash

set -e
set -u

# PostgreSQL 서버 준비 대기
echo "Waiting for PostgreSQL to be ready..."
until pg_isready -h localhost -U "$POSTGRES_USER" --dbname "postgres"; do
  sleep 2
done

PGPASSWORD="${POSTGRES_PASSWORD}"
function create_user_and_database() {
	local db=$(echo $1 | tr ':' ' ' | awk  '{print $1}')
	local owner=$(echo $1 | tr ':' ' ' | awk  '{print $2}')
    if ! psql -U "$POSTGRES_USER" --dbname "postgres" -tc "SELECT 1 FROM pg_database WHERE datname = '$db';" | grep -q 1; then
        if ! psql -U "$POSTGRES_USER" --dbname "postgres" -tc "SELECT 1 FROM pg_catalog.pg_user WHERE usename = '$owner';" | grep -q 1; then
            echo "  Create user '$owner'"
            psql -v ON_ERROR_STOP=1 --username "$POSTGRES_USER" --dbname "postgres" <<-EOSQL
                CREATE USER "$owner" WITH PASSWORD '$POSTGRES_MULTIPLE_INIT_PASSWORD';
EOSQL
        fi
        echo "  Creating database '$db'"
        psql -v ON_ERROR_STOP=1 --username "$POSTGRES_USER" --dbname "postgres" <<-EOSQL
            CREATE DATABASE "$db";
            GRANT ALL PRIVILEGES ON DATABASE "$db" TO "$owner";
            ALTER DATABASE "$db" OWNER TO "$owner";
EOSQL
    else
        echo "Database '$db' already exists, skipping."
    fi
}

if [ -n "${POSTGRES_MULTIPLE_DATABASES:-}" ]; then
	echo "Multiple database creation requested: $POSTGRES_MULTIPLE_DATABASES"
	for db in $(echo $POSTGRES_MULTIPLE_DATABASES | tr ',' ' '); do
		create_user_and_database $db
	done
	echo "Multiple databases created"
fi