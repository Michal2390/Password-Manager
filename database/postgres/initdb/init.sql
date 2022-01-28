CREATE DATABASE secure_password_manager_database WITH
    OWNER = postgres
    ENCODING = 'UTF8'
    TABLESPACE = pg_default
    LC_COLLATE = 'pl_PL.UTF-8'
    LC_CTYPE = 'pl_PL.UTF-8';

CREATE SCHEMA IF NOT EXISTS secure_password_manager;