FROM mysql:8.0

# Copy initialization scripts
COPY ./mysql-init/ /docker-entrypoint-initdb.d/

# Expose the MySQL port
EXPOSE 3306
