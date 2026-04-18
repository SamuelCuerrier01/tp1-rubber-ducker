CREATE OR REPLACE DATABASE BDuckDB;
USE BDuckDB;
CREATE OR REPLACE TABLE users (
    id_user INT AUTO_INCREMENT PRIMARY KEY,
    nom VARCHAR(100) NOT NULL,
    courriel VARCHAR(255) NOT NULL UNIQUE,
    mot_de_passe VARCHAR(255) NOT NULL
);
CREATE OR REPLACE TABLE types (
    id_type INT AUTO_INCREMENT PRIMARY KEY,
    nom VARCHAR(100) NOT NULL UNIQUE
);
CREATE OR REPLACE TABLE modules (
    id_module INT AUTO_INCREMENT PRIMARY KEY,
    nom VARCHAR(100) NOT NULL,
    description TEXT,
    id_type INT,
    id_user INT,
    CONSTRAINT fk_type FOREIGN KEY(id_type) REFERENCES types(id_type) ON DELETE SET NULL,
    CONSTRAINT fk_user FOREIGN KEY(id_user) REFERENCES users(id_user) ON DELETE CASCADE
);
CREATE OR REPLACE TABLE taches (
    id_tache INT AUTO_INCREMENT PRIMARY KEY,
    nom VARCHAR(100) NOT NULL,
    description TEXT,
    date_debut TIMESTAMP,
    date_fin TIMESTAMP,
    id_module INT,
    CONSTRAINT fk_module FOREIGN KEY(id_module) REFERENCES modules(id_module) ON DELETE CASCADE
);
INSERT INTO types (nom) VALUES ('Travail');
INSERT INTO users (nom, courriel, mot_de_passe) 
VALUES ('admin', 'admin@admin.admin', 'admin');
INSERT INTO modules (nom, description, id_type, id_user)
VALUES ('Taches journalieres', 'Taches essentielles', 1, 1);
INSERT INTO taches (nom, description, date_debut, date_fin, id_module)
VALUES ('Manger un bon plat', 'Sante avant tout', '2026-05-01 12:00:00', '2026-05-01 13:00:00', 1);
