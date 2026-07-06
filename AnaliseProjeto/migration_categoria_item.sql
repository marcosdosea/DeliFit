-- Cria a relação muitos-para-muitos entre item e categoria.
-- Execute este script no banco `DeliFit` antes de publicar o código atualizado.

CREATE TABLE IF NOT EXISTS `categoria` (
  `id` int unsigned NOT NULL AUTO_INCREMENT,
  `nome` varchar(50) NOT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `nome_UNIQUE` (`nome`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3;

CREATE TABLE IF NOT EXISTS `item_categoria` (
  `idItem` int unsigned NOT NULL,
  `idCategoria` int unsigned NOT NULL,
  PRIMARY KEY (`idItem`, `idCategoria`),
  KEY `fk_ItemCategoria_Categoria1_idx` (`idCategoria`),
  CONSTRAINT `fk_ItemCategoria_Item1` FOREIGN KEY (`idItem`) REFERENCES `item` (`id`) ON DELETE CASCADE ON UPDATE RESTRICT,
  CONSTRAINT `fk_ItemCategoria_Categoria1` FOREIGN KEY (`idCategoria`) REFERENCES `categoria` (`id`) ON DELETE CASCADE ON UPDATE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3;

-- Categorias padrão do sistema (mesmas 8 que já existiam como lista fixa no código)
INSERT IGNORE INTO `categoria` (`nome`) VALUES
  ('Vegetariano'),
  ('Vegano'),
  ('Sem Glúten'),
  ('Sem Lactose'),
  ('Fitness'),
  ('Low Carb'),
  ('Zero Lactose'),
  ('Proteico');

-- Migra os dados existentes da antiga coluna livre `item.restricao` (quando o valor bate com uma categoria padrão)
INSERT IGNORE INTO `item_categoria` (`idItem`, `idCategoria`)
SELECT i.id, c.id
FROM `item` i
JOIN `categoria` c ON c.nome = i.restricao;

-- A coluna antiga não é mais usada pela aplicação; remova depois de validar a migração de dados acima.
-- ALTER TABLE `item` DROP COLUMN `restricao`;
