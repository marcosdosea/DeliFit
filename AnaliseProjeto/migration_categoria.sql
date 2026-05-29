-- Migration: Criar tabela categoria (CSU11)
-- Execute este script no banco de dados MySQL antes de rodar a aplicação.

CREATE TABLE IF NOT EXISTS categoria (
    id   INT UNSIGNED NOT NULL AUTO_INCREMENT,
    nome VARCHAR(100)  NOT NULL,
    PRIMARY KEY (id),
    UNIQUE KEY nome_UNIQUE (nome)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- Seed: categorias padrão do sistema
INSERT IGNORE INTO categoria (nome) VALUES
    ('Vegetariano'),
    ('Vegano'),
    ('Sem Glúten'),
    ('Sem Lactose'),
    ('Fitness'),
    ('Low Carb'),
    ('Zero Lactose'),
    ('Proteico');
