-- Disable foreign key checks to allow truncating tables
SET FOREIGN_KEY_CHECKS = 0;

-- Optional: Clean existing data if you want a fresh start (comment out if not needed)
TRUNCATE TABLE TemplateGiftItems;
TRUNCATE TABLE Categories;

SET FOREIGN_KEY_CHECKS = 1;

-- 1. Insert Categories
INSERT INTO Categories (Id, Name) VALUES 
(1, 'Cozinha'),
(2, 'Eletrodomésticos'),
(3, 'Cama, Mesa e Banho'),
(4, 'Decoração'),
(5, 'Móveis'),
(6, 'Eletrônicos'),
(7, 'Lua de Mel (Cotas)'),
(8, 'Lazer e Bar');

-- 2. Insert Template Items associated with Categories

-- Cozinha (Id: 1)
INSERT INTO TemplateGiftItems (Name, Description, CategoryId) VALUES
('Jogo de Panelas Antiaderente', 'Conjunto com 5 peças de alta qualidade', 1),
('Faqueiro 24 Peças', 'Aço inox com design moderno', 1),
('Aparelho de Jantar 20 Peças', 'Cerâmica ou porcelana para servir bem', 1),
('Jogo de Taças de Vinho', 'Conjunto com 6 taças de cristal', 1),
('Jogo de Copos', 'Conjunto com 6 copos de vidro', 1),
('Travessa Refratária', 'Vidro temperado para forno', 1),
('Petisqueira', 'Ideal para receber amigos', 1),
('Fruteira de Mesa', 'Design moderno para centro de mesa', 1),
('Tábua de Corte', 'Bambu ou madeira tratada', 1),
('Escorredor de Louça', 'Aço inox resistente', 1);

-- Eletrodomésticos (Id: 2)
INSERT INTO TemplateGiftItems (Name, Description, CategoryId) VALUES
('Liquidificador', 'Potente e com copo resistente', 2),
('Batedeira', 'Planetária ou comum para bolos e massas', 2),
('Cafeteira Elétrica', 'Para aquele cafézinho da manhã', 2),
('Sanduicheira / Grill', 'Prática para o dia a dia', 2),
('Fritadeira Air Fryer', 'Sem óleo, mais saúde', 2),
('Ferro de Passar a Vapor', 'Base antiaderente', 2),
('Aspirador de Pó', 'Compacto e eficiente', 2),
('Micro-ondas', 'Essencial para a cozinha moderna', 2),
('Mixer de Mão', 'Versátil para sopas e vitaminas', 2),
('Torradeira', 'Para pães quentinhos', 2);

-- Cama, Mesa e Banho (Id: 3)
INSERT INTO TemplateGiftItems (Name, Description, CategoryId) VALUES
('Jogo de Lençol Casal', 'Algodão macio e durável', 3),
('Jogo de Cama Queen/King', 'Conforto e sofisticação', 3),
('Edredom Acolchoado', 'Para noites mais frias', 3),
('Kit de Toalhas de Banho', 'Jogo com 2 toalhas de banho e 2 de rosto', 3),
('Toalha de Mesa', 'Estampada ou lisa para 6 lugares', 3),
('Jogo Americano', 'Kit com 4 ou 6 lugares', 3),
('Protetor de Colchão', 'Impermeável e lavável', 3),
('Travesseiros (Par)', 'Macios e antialérgicos', 3);

-- Decoração (Id: 4)
INSERT INTO TemplateGiftItems (Name, Description, CategoryId) VALUES
('Vaso Decorativo', 'Cerâmica ou vidro para flores', 4),
('Quadro Decorativo', 'Arte para a sala ou quarto', 4),
('Abajur de Mesa', 'Iluminação suave para o quarto', 4),
('Tapete para Sala', 'Conforto e design para o ambiente', 4),
('Espelho com Moldura', 'Para ampliar o ambiente', 4),
('Porta-Retrato Digital', 'Para exibir as fotos do casamento', 4),
('Velas Aromáticas', 'Kit para criar um clima aconchegante', 4);

-- Móveis (Id: 5)
INSERT INTO TemplateGiftItems (Name, Description, CategoryId) VALUES
('Mesa de Cabeceira', 'Par para o quarto do casal', 5),
('Puff Decorativo', 'Versátil para a sala de estar', 5),
('Sapateira', 'Organização prática', 5),
('Carrinho de Bar', 'Para bebidas e decoração', 5);

-- Eletrônicos (Id: 6)
INSERT INTO TemplateGiftItems (Name, Description, CategoryId) VALUES
('Smart TV', 'Para maratonar séries', 6),
('Soundbar', 'Som de cinema em casa', 6),
('Assistente Virtual (Alexa/Google)', 'Automação residencial', 6);

-- Lua de Mel - Cotas (Id: 7)
INSERT INTO TemplateGiftItems (Name, Description, CategoryId) VALUES
('Cota para Jantar Romântico', 'Ajude o casal a ter uma noite inesquecível', 7),
('Cota para Passeio Turístico', 'Para conhecerem lugares incríveis', 7),
('Cota para Diária de Hotel', 'Contribua com a hospedagem', 7),
('Cota para Passagens Aéreas', 'Ajude o casal a voar para o destino', 7),
('Cota para Drinks na Praia', 'Para relaxarem à beira mar', 7),
('Cota de Spa Day', 'Massagem relaxante para o casal', 7);

-- Lazer e Bar (Id: 8)
INSERT INTO TemplateGiftItems (Name, Description, CategoryId) VALUES
('Kit Caipirinha', 'Tábua, copo e socador', 8),
('Balde de Gelo', 'Inox ou acrílico', 8),
('Conjunto de Utensílios para Churrasco', 'Faca, garfo e pegador', 8),
('Cooler Térmico', 'Para levar bebidas para qualquer lugar', 8);
