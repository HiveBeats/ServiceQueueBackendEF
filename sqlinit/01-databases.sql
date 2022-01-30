-- # create databases
-- CREATE DATABASE IF NOT EXISTS '${MYSQL_DATABASE}';

-- # create root user and grant rights
-- CREATE USER 'root'@'localhost' IDENTIFIED BY '${MYSQL_ROOT_PASSWORD}';
-- GRANT ALL PRIVILEGES ON *.* TO 'root'@'%';

-- CREATE USER 'john'@'localhost' IDENTIFIED BY '${MYSQL_JOHN_PASSWORD}';
-- GRANT ALL PRIVILEGES ON *.* TO 'john'@'localhost' WITH GRANT OPTION;
-- CREATE USER 'john'@'%' IDENTIFIED BY '${MYSQL_JOHN_PASSWORD}';
-- GRANT ALL PRIVILEGES ON *.* TO 'john'@'%' WITH GRANT OPTION;
