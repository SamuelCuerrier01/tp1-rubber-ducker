CREATE OR REPLACE DATABASE BDuckDB;
USE BDuckDB;

CREATE OR REPLACE TABLE app_user (
  id INT PRIMARY KEY AUTO_INCREMENT,
  name VARCHAR(100) NOT NULL,
  email VARCHAR(255) NOT NULL UNIQUE,
  password_hash VARCHAR(255) NOT NULL
);

INSERT IGNORE INTO app_user (name, email, password)
VALUES ('admin', 'admin@admin.com', 'admin');
