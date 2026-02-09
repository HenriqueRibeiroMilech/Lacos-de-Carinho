-- Disable foreign key checks to allow truncating tables
SET FOREIGN_KEY_CHECKS = 0;

-- Clean existing data
TRUNCATE TABLE TemplateGiftItems;
TRUNCATE TABLE Categories;

SET FOREIGN_KEY_CHECKS = 1;

-- Categories matching the frontend GiftCategory enum
-- Frontend enum: Cozinha=1, Quarto=2, Banheiro=3, Sala=4, Lavanderia=5, CasaInteligente=6, MesaPosta=7, AreaExterna=8, Experiencias=9, Contribuicoes=10
INSERT INTO Categories (Id, Name) VALUES
(1, '🍳 Cozinha'),
(2, '🛏 Quarto'),
(3, '🚿 Banheiro'),
(4, '🛋 Sala de Estar'),
(5, '🧹 Lavanderia'),
(6, '🏠 Casa Inteligente'),
(7, '🍷 Mesa Posta'),
(8, '🌿 Área Externa'),
(9, '💝 Experiências'),
(10, '💰 Contribuições');

-- Cozinha (Id: 1)
INSERT INTO TemplateGiftItems (Name, Description, CategoryId) VALUES
('Jogo de Panelas Antiaderente', 'Conjunto com 5 peças de alta qualidade', 1),
('Faqueiro 24 Peças', 'Aço inox com design moderno', 1),
('Aparelho de Jantar 20 Peças', 'Cerâmica ou porcelana para servir bem', 1),
('Liquidificador', 'Potente e com copo resistente', 1),
('Batedeira Planetária', 'Para bolos e massas', 1),
('Cafeteira Elétrica', 'Para aquele cafézinho da manhã', 1),
('Fritadeira Air Fryer', 'Sem óleo, mais saúde', 1),
('Micro-ondas', 'Essencial para a cozinha moderna', 1),
('Sanduicheira / Grill', 'Prática para o dia a dia', 1),
('Mixer de Mão', 'Versátil para sopas e vitaminas', 1),
('Torradeira', 'Para pães quentinhos', 1),
('Panela de Pressão Elétrica', 'Praticidade e segurança', 1),
('Processador de Alimentos', 'Multifuncional com várias lâminas', 1),
('Chaleira Elétrica', 'Inox 1.7 litros', 1),
('Forno Elétrico', 'Forno de bancada 44 litros', 1);

-- Quarto (Id: 2)
INSERT INTO TemplateGiftItems (Name, Description, CategoryId) VALUES
('Jogo de Cama Queen', '400 fios 100% algodão', 2),
('Jogo de Cama King', '400 fios percal', 2),
('Travesseiros de Pluma', 'Par de travesseiros macios', 2),
('Edredom Queen', 'Dupla face microfibra', 2),
('Edredom King', 'Pluma sintética aconchegante', 2),
('Manta Decorativa', 'Manta de sofá/cama em tricô', 2),
('Protetor de Colchão', 'Impermeável e lavável', 2),
('Luminária de Cabeceira', 'Par de luminárias modernas', 2),
('Cobertor de Microfibra', 'Leve e quentinho', 2);

-- Banheiro (Id: 3)
INSERT INTO TemplateGiftItems (Name, Description, CategoryId) VALUES
('Jogo de Toalhas de Banho', 'Conjunto 5 peças 100% algodão', 3),
('Roupões de Banho', 'Par de roupões felpudos', 3),
('Kit Organizador Banheiro', 'Para bancada do banheiro', 3),
('Espelho de Aumento', 'Com LED para maquiagem', 3),
('Balança Digital', 'Com bioimpedância', 3),
('Tapetes de Banheiro', 'Kit antiderrapante', 3),
('Saboneteira Automática', 'Dispenser automático', 3);

-- Sala de Estar (Id: 4)
INSERT INTO TemplateGiftItems (Name, Description, CategoryId) VALUES
('Almofadas Decorativas', 'Kit de 4 para sofá', 4),
('Cortinas Blackout', 'Par para a sala', 4),
('Tapete Grande', '2x2.5m para sala', 4),
('Abajur de Piso', 'Luminária de pé moderna', 4),
('Quadros Decorativos', 'Conjunto moderno', 4),
('Vasos Decorativos', 'Cerâmica para flores', 4),
('Relógio de Parede', 'Design moderno', 4),
('Puff Organizador', 'Com espaço de armazenamento', 4);

-- Lavanderia (Id: 5)
INSERT INTO TemplateGiftItems (Name, Description, CategoryId) VALUES
('Ferro de Passar a Vapor', 'Base antiaderente', 5),
('Tábua de Passar', 'Com suporte para ferro', 5),
('Vaporizador de Roupas', 'Portátil para roupas', 5),
('Cesto de Roupa Suja', 'Com divisórias', 5),
('Aspirador de Pó', 'Compacto e eficiente', 5);

-- Casa Inteligente (Id: 6)
INSERT INTO TemplateGiftItems (Name, Description, CategoryId) VALUES
('Assistente Virtual', 'Echo Dot ou Google Nest', 6),
('Lâmpadas Inteligentes', 'Kit smart Wi-Fi RGB', 6),
('Tomadas Inteligentes', 'Kit Wi-Fi com timer', 6),
('Câmera de Segurança', 'Wi-Fi com visão noturna', 6),
('Fechadura Digital', 'Com senha e biometria', 6),
('Robô Aspirador', 'Com mapeamento inteligente', 6),
('Smart TV', 'Para maratonar séries', 6),
('Soundbar', 'Som de cinema em casa', 6);

-- Mesa Posta (Id: 7)
INSERT INTO TemplateGiftItems (Name, Description, CategoryId) VALUES
('Jogo de Taças de Vinho', 'Conjunto com 6 taças de cristal', 7),
('Jogo de Copos', 'Conjunto com 6 copos de vidro', 7),
('Jogo Americano', 'Kit para 6 lugares', 7),
('Sousplat', 'Conjunto decorativo', 7),
('Porta-Guardanapos', 'Conjunto de argolas', 7),
('Fruteira de Mesa', 'Metal ou cerâmica', 7),
('Balde de Gelo', 'Inox com pegador', 7),
('Decanter', 'Para vinho em cristal', 7),
('Conjunto de Xícaras', 'Porcelana para chá/café', 7),
('Travessa Refratária', 'Vidro temperado para forno', 7),
('Petisqueira', 'Ideal para receber amigos', 7);

-- Área Externa (Id: 8)
INSERT INTO TemplateGiftItems (Name, Description, CategoryId) VALUES
('Churrasqueira Portátil', 'A carvão portátil', 8),
('Kit Churrasco', 'Facas e utensílios', 8),
('Cadeiras de Praia', 'Par reclináveis', 8),
('Guarda-Sol', 'Com proteção UV', 8),
('Cooler Térmico', 'Para levar bebidas', 8),
('Kit Caipirinha', 'Tábua, copo e socador', 8),
('Conjunto Jardim', 'Mesa e cadeiras', 8);

-- Experiências (Id: 9)
INSERT INTO TemplateGiftItems (Name, Description, CategoryId) VALUES
('Jantar Romântico', 'Voucher para restaurante especial', 9),
('Spa Day', 'Dia de spa para o casal', 9),
('Passeio de Balão', 'Voo para duas pessoas', 9),
('Curso de Culinária', 'Aula para o casal', 9),
('Degustação de Vinhos', 'Experiência em vinícola', 9),
('Noite no Hotel', 'Diária em hotel romântico', 9);

-- Contribuições (Id: 10)
INSERT INTO TemplateGiftItems (Name, Description, CategoryId) VALUES
('Lua de Mel', 'Contribuição para viagem', 10),
('Cota para Diária de Hotel', 'Contribua com a hospedagem', 10),
('Cota para Passagens Aéreas', 'Ajude o casal a voar', 10),
('Reforma da Casa', 'Contribuição para o lar', 10);
