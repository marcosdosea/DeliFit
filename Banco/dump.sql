-- =============================================================
-- DeliFit - Dump de Dados para Testes
-- =============================================================
-- Credenciais dos usuários criados por este dump:
--
-- Admin:
--   Email:  admin@delifit.com
--   Senha:  Admin@123
--
-- Cliente:
--   Email:  cliente@delifit.com
--   Senha:  Senha@123
--
-- Restaurante (GerenteRestaurante):
--   Email:  restaurante@delifit.com
--   Senha:  Senha@123
-- =============================================================

-- =============================================================
-- Banco de dados: IdentityUsers
-- =============================================================
USE IdentityUsers;

-- Roles
INSERT IGNORE INTO `AspNetRoles` (`Id`, `Name`, `NormalizedName`, `ConcurrencyStamp`) VALUES
('1a2b3c4d-0001-0000-0000-000000000001', 'Admin',              'ADMIN',              NULL),
('1a2b3c4d-0002-0000-0000-000000000002', 'GerenteRestaurante', 'GERENTERESTAURANTE', NULL),
('1a2b3c4d-0003-0000-0000-000000000003', 'Cliente',            'CLIENTE',            NULL);

-- Usuários (senhas pré-geradas com ASP.NET Identity PBKDF2-HMACSHA256)
-- Admin@123
-- Senha@123
INSERT IGNORE INTO `AspNetUsers`
    (`Id`, `UserName`, `NormalizedUserName`, `Email`, `NormalizedEmail`,
     `EmailConfirmed`, `PasswordHash`, `SecurityStamp`, `ConcurrencyStamp`,
     `PhoneNumber`, `PhoneNumberConfirmed`, `TwoFactorEnabled`,
     `LockoutEnd`, `LockoutEnabled`, `AccessFailedCount`)
VALUES
(
    'aaaaaaaa-0001-0000-0000-000000000001',
    'admin@delifit.com', 'ADMIN@DELIFIT.COM',
    'admin@delifit.com', 'ADMIN@DELIFIT.COM',
    1,
    'AQAAAAIAAYagAAAAEE64hcliTySCUrvJofyFfxqsc3EusbeBluLiB8lxSGPy9Od+RNn5rKc3LeVPolhEag==',
    'STATIC-SECURITY-STAMP-ADMIN-000001',
    'STATIC-CONCURRENCY-STAMP-ADMIN-001',
    NULL, 0, 0, NULL, 1, 0
),
(
    'aaaaaaaa-0002-0000-0000-000000000002',
    'cliente@delifit.com', 'CLIENTE@DELIFIT.COM',
    'cliente@delifit.com', 'CLIENTE@DELIFIT.COM',
    1,
    'AQAAAAIAAYagAAAAENkhcFTYr3aDyrfCrU+v1Gtz2Xx2HMikFE9Bn+hP3BzRqYYcghiap/0+HvwmgSADdg==',
    'STATIC-SECURITY-STAMP-CLIENTE-0002',
    'STATIC-CONCURRENCY-STAMP-CLIENTE02',
    '62987654321', 0, 0, NULL, 1, 0
),
(
    'aaaaaaaa-0003-0000-0000-000000000003',
    'restaurante@delifit.com', 'RESTAURANTE@DELIFIT.COM',
    'restaurante@delifit.com', 'RESTAURANTE@DELIFIT.COM',
    1,
    'AQAAAAIAAYagAAAAENkhcFTYr3aDyrfCrU+v1Gtz2Xx2HMikFE9Bn+hP3BzRqYYcghiap/0+HvwmgSADdg==',
    'STATIC-SECURITY-STAMP-REST-000003',
    'STATIC-CONCURRENCY-STAMP-REST-003',
    '62912345678', 0, 0, NULL, 1, 0
);

-- Atribuição de roles aos usuários
INSERT IGNORE INTO `AspNetUserRoles` (`UserId`, `RoleId`) VALUES
('aaaaaaaa-0001-0000-0000-000000000001', '1a2b3c4d-0001-0000-0000-000000000001'), -- Admin
('aaaaaaaa-0002-0000-0000-000000000002', '1a2b3c4d-0003-0000-0000-000000000003'), -- Cliente
('aaaaaaaa-0003-0000-0000-000000000003', '1a2b3c4d-0002-0000-0000-000000000002'); -- GerenteRestaurante


-- =============================================================
-- Banco de dados: DeliFit
-- =============================================================
USE DeliFit;

-- Cliente
-- CPF válido: 529.982.247-25
INSERT IGNORE INTO `cliente` (`nome`, `cpf`, `email`, `telefone`, `dataNascimento`) VALUES
('João Silva', '52998224725', 'cliente@delifit.com', '62987654321', '1990-05-15');

-- Restaurante
-- CPF proprietário válido: 111.444.777-35
-- CNPJ válido: 11.222.333/0001-81
INSERT IGNORE INTO `restaurante`
    (`nomeRestaurante`, `nomeProprietario`, `cpfProprietario`, `cnpj`,
     `descricao`, `telefoneProprietario`, `telefoneRestaurante`,
     `email`, `validado`, `rua`, `numero`, `bairro`, `cep`, `cidade`, `estado`)
VALUES
(
    'Sabor Natural',
    'Maria Santos',
    '11144477735',
    '11222333000181',
    'Restaurante saudável com pratos naturais e nutritivos.',
    '62912345678',
    '6232109876',
    'restaurante@delifit.com',
    1,
    'Rua das Flores',
    '123',
    'Jardim América',
    '74000000',
    'Goiânia',
    'GO'
);
