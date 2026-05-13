-- ==============================================================================
-- BACKUP DE ESTRUTURA DO BANCO DE DADOS - AERO CONCEPTS (SAD PRECIFICAÇÃO)
-- DATA: 05/05/2026
-- ==============================================================================

-- CREATE DATABASE IF NOT EXISTS sad_precificacao;
-- USE sad_precificacao;

-- ==============================================================================
-- TABELAS DO SISTEMA
-- ==============================================================================

-- TABELA: USUARIO
-- CREATE TABLE IF NOT EXISTS usuario (
--     id_usuario INT AUTO_INCREMENT PRIMARY KEY,
--     nome VARCHAR(255) NOT NULL,
--     email VARCHAR(255) NOT NULL UNIQUE,
--     senha VARCHAR(255) NOT NULL,
--     status VARCHAR(50) DEFAULT 'Ativo'
-- );

-- TABELA: CLIENTE
-- CREATE TABLE IF NOT EXISTS cliente (
--     id_cliente INT AUTO_INCREMENT PRIMARY KEY,
--     nome VARCHAR(255) NOT NULL,
--     tipo VARCHAR(50), -- PF/PJ
--     cpf_cnpj VARCHAR(20),
--     cidade VARCHAR(100),
--     estado VARCHAR(50),
--     contato VARCHAR(100)
-- );

-- TABELA: CARGO
-- CREATE TABLE IF NOT EXISTS cargo (
--     id_cargo INT AUTO_INCREMENT PRIMARY KEY,
--     nome VARCHAR(255) NOT NULL,
--     nivel VARCHAR(50), -- Jr/Pl/Sr
--     custo_medio_hora DECIMAL(18, 2) NOT NULL,
--     descricao TEXT
-- );

-- TABELA: FUNCIONARIO
-- CREATE TABLE IF NOT EXISTS funcionario (
--     id_funcionario INT AUTO_INCREMENT PRIMARY KEY,
--     id_cargo INT NOT NULL,
--     nome VARCHAR(255) NOT NULL,
--     custo_hora DECIMAL(18, 2) NOT NULL,
--     tipo_vinculo VARCHAR(50), -- CLT/PJ/Autônomo
--     status VARCHAR(50) DEFAULT 'Ativo',
--     FOREIGN KEY (id_cargo) REFERENCES cargo(id_cargo)
-- );

-- TABELA: PROJETO
-- CREATE TABLE IF NOT EXISTS projeto (
--     id_projeto INT AUTO_INCREMENT PRIMARY KEY,
--     id_usuario INT NOT NULL,
--     id_cliente INT NOT NULL,
--     nome VARCHAR(255) NOT NULL,
--     tipo VARCHAR(100), -- Produto/Serviço
--     status VARCHAR(50) DEFAULT 'Rascunho', -- Rascunho/Orçado/Aprovado/Executando/Concluído
--     data_criacao DATETIME DEFAULT CURRENT_TIMESTAMP,
--     data_conclusao_prevista DATE,
--     FOREIGN KEY (id_usuario) REFERENCES usuario(id_usuario),
--     FOREIGN KEY (id_cliente) REFERENCES cliente(id_cliente)
-- );

-- TABELA: TAREFA
-- CREATE TABLE IF NOT EXISTS tarefa (
--     id_tarefa INT AUTO_INCREMENT PRIMARY KEY,
--     id_projeto INT NOT NULL,
--     id_funcionario INT NOT NULL,
--     descricao VARCHAR(255),
--     horas_estimadas DECIMAL(18, 2) DEFAULT 0,
--     horas_reais DECIMAL(18, 2) DEFAULT 0,
--     custo_real DECIMAL(18, 2) DEFAULT 0,
--     status VARCHAR(50) DEFAULT 'Pendente', -- Pendente/Executando/Concluída
--     FOREIGN KEY (id_projeto) REFERENCES projeto(id_projeto) ON DELETE CASCADE,
--     FOREIGN KEY (id_funcionario) REFERENCES funcionario(id_funcionario)
-- );

-- TABELA: CUSTO
-- CREATE TABLE IF NOT EXISTS custo (
--     id_custo INT AUTO_INCREMENT PRIMARY KEY,
--     id_projeto INT NOT NULL,
--     nome VARCHAR(255) NOT NULL,
--     categoria VARCHAR(100),
--     tipo VARCHAR(50), -- Direto/Indireto
--     valor DECIMAL(18, 2) NOT NULL,
--     unidade VARCHAR(50), -- Unitário/Hora/Dia/Mês
--     data_cadastro DATETIME DEFAULT CURRENT_TIMESTAMP,
--     FOREIGN KEY (id_projeto) REFERENCES projeto(id_projeto) ON DELETE CASCADE
-- );

-- TABELA: ORCAMENTO
-- CREATE TABLE IF NOT EXISTS orcamento (
--     id_orcamento INT AUTO_INCREMENT PRIMARY KEY,
--     id_projeto INT NOT NULL UNIQUE, -- Relacionamento 1:1
--     custo_base DECIMAL(18, 2) DEFAULT 0,
--     percentual_impostos DECIMAL(18, 2) DEFAULT 0,
--     valor_impostos DECIMAL(18, 2) DEFAULT 0,
--     margem_percentual DECIMAL(18, 2) DEFAULT 0,
--     valor_margem DECIMAL(18, 2) DEFAULT 0,
--     valor_final DECIMAL(18, 2) DEFAULT 0,
--     data_criacao DATETIME DEFAULT CURRENT_TIMESTAMP,
--     FOREIGN KEY (id_projeto) REFERENCES projeto(id_projeto) ON DELETE CASCADE
-- );

-- ==============================================================================
-- DADOS INICIAIS DE CONFIGURAÇÃO (CARGOS, FUNCIONÁRIOS, USUÁRIOS, CLIENTES)
-- ==============================================================================

-- INSERT IGNORE INTO cargo (id_cargo, nome, nivel, custo_medio_hora) VALUES 
-- (1, 'Engenheiro Elétrico', 'Sr', 160.00), (2, 'Supervisor Eletroeletrônico', 'Pl', 130.00), 
-- (3, 'Engenheiro Especialista Turbomáquinas', 'Espec', 220.00), (4, 'Coordenador Técnico de Serviços', 'Sr', 145.00), 
-- (5, 'Analista de Engenharia Industrial', 'Pl', 95.00), (6, 'Coordenador de Engenharia Industrial', 'Sr', 155.00), 
-- (7, 'Analista de PD&I', 'Pl', 105.00), (8, 'Engenheiro de PD&I', 'Sr', 175.00), 
-- (9, 'Gerente de Engenharia', 'Sr', 250.00), (10, 'Gerente de Projetos', 'Sr', 230.00), 
-- (11, 'Consultor Especialista PD&I/Eng', 'Espec', 300.00);

-- INSERT IGNORE INTO funcionario (id_funcionario, id_cargo, nome, custo_hora, tipo_vinculo, status) VALUES 
-- (1, 1, 'Paulino Rubião', 165.00, 'CLT', 'Ativo'), (2, 2, 'Eduardo Sedano', 135.00, 'CLT', 'Ativo'), 
-- (3, 3, 'Flavio Natal', 225.00, 'PJ', 'Ativo'), (4, 4, 'Antonio Aguida', 145.00, 'CLT', 'Ativo'), 
-- (5, 4, 'Roberto Souza Costa', 145.00, 'CLT', 'Ativo'), (6, 5, 'Luiz Menezes', 98.00, 'CLT', 'Ativo'), 
-- (7, 6, 'Evandro Lamberti', 160.00, 'CLT', 'Ativo'), (8, 7, 'Clayton Sant''ana', 110.00, 'CLT', 'Ativo'), 
-- (9, 7, 'Igor Alves', 110.00, 'CLT', 'Ativo'), (10, 7, 'Victor Hugo Noronha', 110.00, 'CLT', 'Ativo'), 
-- (11, 8, 'Lucilene Moraes', 180.00, 'CLT', 'Ativo'), (12, 8, 'Gerhard Egwarth', 180.00, 'PJ', 'Ativo'), 
-- (13, 9, 'Daniel Joaquim Pereira', 260.00, 'CLT', 'Ativo'), (14, 10, 'Eduard Müller', 240.00, 'CLT', 'Ativo'), 
-- (15, 11, 'Marco Antônio Carvalho', 310.00, 'PJ', 'Ativo');

-- INSERT IGNORE INTO usuario (id_usuario, nome, email, senha, status) VALUES 
-- (1, 'Admin', 'admin@aeroconcepts.com', 'admin123', 'Ativo');

-- INSERT IGNORE INTO cliente (id_cliente, nome, tipo, cidade, estado, contato) VALUES 
-- (1, 'Funcate', 'Jurídica', 'São José dos Campos', 'SP', 'Contato Comercial'), 
-- (2, 'Voith', 'Jurídica', 'São Paulo', 'SP', 'Departamento de Projetos'), 
-- (3, 'Arauco', 'Jurídica', 'Curitiba', 'PR', 'Suprimentos'), 
-- (4, 'EESC', 'Institucional', 'São Carlos', 'SP', 'Diretoria Técnica'), 
-- (5, 'International Paper', 'Jurídica', 'Mogi Guaçu', 'SP', 'Engenharia'), 
-- (6, 'Suzano', 'Jurídica', 'Salvador', 'BA', 'Gestão de Contratos'), 
-- (7, 'Klabin', 'Jurídica', 'Telêmaco Borba', 'PR', 'Planejamento'), 
-- (8, 'FAB', 'Governo', 'Brasília', 'DF', 'Comando da Aeronáutica'), 
-- (9, 'IAE', 'Governo', 'São José dos Campos', 'SP', 'Diretoria IAE'), 
-- (10, 'Birla Carbon', 'Jurídica', 'Cubatão', 'SP', 'Manutenção Industrial'), 
-- (11, 'Rhodia', 'Jurídica', 'Paulínia', 'SP', 'Compras Técnicas'), 
-- (12, 'Raizen', 'Jurídica', 'Piracicaba', 'SP', 'Projetos Estratégicos'), 
-- (13, 'DCTA', 'Governo', 'São José dos Campos', 'SP', 'Secretaria de Tecnologia');

-- ==============================================================================
-- DADOS DE TESTE: PROJETOS, TAREFAS E ORÇAMENTOS (PARA HISTÓRICO)
-- ==============================================================================

-- 1. PROJETOS (10 EXEMPLOS)
-- INSERT IGNORE INTO projeto (id_projeto, id_usuario, id_cliente, nome, tipo, status, data_criacao) VALUES 
-- (1, 1, 1, 'Modernização Turbina G3', 'Serviço', 'Concluído', '2026-01-10'),
-- (2, 1, 2, 'Análise Estrutural Asa X1', 'P&D', 'Executando', '2026-02-15'),
-- (3, 1, 3, 'Consultoria Eficiência Térmica', 'Consultoria', 'Aprovado', '2026-03-01'),
-- (4, 1, 8, 'Manutenção Hangar 5', 'Serviço', 'Orçado', '2026-03-20'),
-- (5, 1, 9, 'Estudo Aerodinâmico Foguete', 'P&D', 'Concluído', '2026-01-05'),
-- (6, 1, 13, 'Revisão Eletrônica DCTA', 'Serviço', 'Executando', '2026-04-10'),
-- (7, 1, 5, 'Otimização Caldeiras IP', 'Serviço', 'Rascunho', '2026-05-01'),
-- (8, 1, 12, 'Proposta Automação Raízen', 'Consultoria', 'Orçado', '2026-04-25'),
-- (9, 1, 10, 'Instalação Filtros Birla', 'Serviço', 'Concluído', '2026-02-28'),
-- (10, 1, 6, 'Gestão Ativos Suzano', 'Consultoria', 'Aprovado', '2026-03-15');

-- 2. TAREFAS (ITENS DE CUSTO DE MÃO DE OBRA)
-- INSERT IGNORE INTO tarefa (id_projeto, id_funcionario, descricao, horas_estimadas, status) VALUES 
-- (1, 13, 'Coordenação Geral', 40, 'Concluída'),
-- (1, 1, 'Desenvolvimento Elétrico', 80, 'Concluída'),
-- (2, 15, 'Consultoria Especialista', 20, 'Executando'),
-- (2, 8, 'Análise de Dados', 60, 'Executando'),
-- (5, 3, 'Cálculos Turbomáquinas', 100, 'Concluída'),
-- (9, 4, 'Supervisão de Campo', 30, 'Concluída');

-- 3. ORÇAMENTOS (ITENS DE INDICADORES FINANCEIROS)
-- INSERT IGNORE INTO orcamento (id_projeto, custo_base, percentual_impostos, valor_impostos, margem_percentual, valor_margem, valor_final) VALUES 
-- (1, 15000.00, 15, 2250.00, 25, 3750.00, 21000.00),
-- (2, 30000.00, 15, 4500.00, 30, 9000.00, 43500.00),
-- (3, 5000.00, 10, 500.00, 40, 2000.00, 7500.00),
-- (4, 12000.00, 15, 1800.00, 20, 2400.00, 16200.00),
-- (5, 45000.00, 15, 6750.00, 35, 15750.00, 67500.00),
-- (6, 8500.00, 15, 1275.00, 20, 1700.00, 11475.00),
-- (7, 2000.00, 15, 300.00, 20, 400.00, 2700.00),
-- (8, 6000.00, 10, 600.00, 30, 1800.00, 8400.00),
-- (9, 18000.00, 15, 2700.00, 25, 4500.00, 25200.00),
-- (10, 10000.00, 15, 1500.00, 30, 3000.00, 14500.00);

-- ==============================================================================
-- FIM DO SCRIPT DE BACKUP
-- ==============================================================================