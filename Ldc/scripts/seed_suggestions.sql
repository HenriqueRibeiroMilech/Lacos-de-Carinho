-- Disable foreign key checks to allow truncating tables
SET FOREIGN_KEY_CHECKS = 0;

-- Clean existing data
TRUNCATE TABLE TemplateGiftItems;
TRUNCATE TABLE Categories;

SET FOREIGN_KEY_CHECKS = 1;

INSERT INTO Categories (Id, Name) VALUES
(1, '🍳 Cozinha'),
(2, '🛏 Quarto'),
(3, '🚿 Banheiro'),
(4, '🛋 Sala de Estar'),
(5, '🧹 Lavanderia'),
(6, '🏠 Casa Inteligente'),
(7, '🍷 Mesa Posta'),
(8, '🌿 Área Externa'),
(9, '💻 Escritório'),
(10, '🍸 Bar e Lazer');

-- 1. Cozinha
INSERT INTO TemplateGiftItems (Name, Description, CategoryId) VALUES
('Jogo de Panelas Cerâmica', 'Conjunto premium antiaderente', 1),
('Jogo de Panelas Inox', 'Fundo triplo durável', 1),
('Air Fryer Digital', 'Fritadeira sem óleo', 1),
('Cafeteira Expresso', 'Compatível com cápsulas', 1),
('Batedeira Planetária', 'Para massas e bolos', 1),
('Liquidificador Potente', 'Copo de vidro resistente', 1),
('Processador de Alimentos', 'Fatia e pica legumes', 1),
('Mixer de Mão 3 em 1', 'Com batedor e processador', 1),
('Torradeira Elétrica', 'Níveis de tostagem ajustáveis', 1),
('Sanduicheira Grill', 'Grelhados e lanches', 1),
('Chaleira Elétrica Inox', 'Água quente em minutos', 1),
('Panela de Pressão Elétrica', 'Segurança e timer digital', 1),
('Forno Elétrico de Bancada', 'Assa e gratina (44L)', 1),
('Micro-ondas Inox', 'Funções pré-programadas', 1),
('Jogo de Facas com Cepo', 'Bloco de madeira com facas', 1),
('Kit Utensílios Silicone', 'Não risca as panelas', 1),
('Potes Herméticos Vidro', 'Kit 10 peças para mantimentos', 1),
('Máquina de Waffle', 'Café da manhã especial', 1),
('Moedor de Café', 'Grãos moídos na hora', 1),
('Balança de Cozinha', 'Precisão nas receitas', 1);

-- 2. Quarto
INSERT INTO TemplateGiftItems (Name, Description, CategoryId) VALUES
('Jogo de Cama Queen 400 Fios', 'Algodão egípcio acetinado', 2),
('Jogo de Cama King 300 Fios', 'Percal macio e durável', 2),
('Par Travesseiros NASA', 'Viscoelástico anatômico', 2),
('Edredom Queen Plumas', 'Toque macio e quentinho', 2),
('Kit Colcha/Cobre-leito', 'Estampa moderna dupla face', 2),
('Manta Decorativa Tricô', 'Para o pé da cama', 2),
('Protetor de Colchão', 'Impermeável e silencioso', 2),
('Saia Box', 'Acabamento para a cama', 2),
('Kit Organizador Gavetas', 'Colmeias para roupas íntimas', 2),
('Umidificador de Ar', 'Silencioso com timer', 2),
('Tapetes de Lã', 'Par para lateral da cama', 2);

-- 3. Banheiro
INSERT INTO TemplateGiftItems (Name, Description, CategoryId) VALUES
('Jogo de Toalhas Banhão', '5 peças fio penteado macio', 3),
('Par Roupões Microfibra', 'Saída de banho confortável', 3),
('Tapete Banho Memory Foam', 'Ultra absorvente e macio', 3),
('Kit Acessórios Bancada', 'Porta-sabonete e escovas cerâmica', 3),
('Espelho de Aumento LED', 'Para maquiagem e barbear', 3),
('Balança Digital', 'Controle de peso e bioimpedância', 3),
('Cesto Roupa Bambu', 'Com forro de tecido removível', 3),
('Secador de Cabelo', 'Profissional com íons', 3),
('Organizador Cosméticos', 'Acrílico giratório', 3),
('Lixeira Inox Pedal', 'Design clean e higiênico', 3);

-- 4. Sala de Estar
INSERT INTO TemplateGiftItems (Name, Description, CategoryId) VALUES
('Tapete Sala (2,00 x 2,50)', 'Design geométrico moderno', 4),
('Kit Almofadas Decorativas', '4 capas com enchimento', 4),
('Manta para Sofá', 'Proteção e decoração', 4),
('Abajur de Chão', 'Luminária de piso design', 4),
('Conjunto Quadros', 'Kit composição parede', 4),
('Vaso Decorativo Grande', 'Para arranjos de chão ou mesa', 4),
('Bandeja Espelhada', 'Para decorar o centro de mesa', 4),
('Puff Decorativo', 'Assento extra confortável', 4),
('Relógio de Parede', 'Design minimalista 30cm', 4),
('Difusor de Aromas', 'Varetas para perfumar o ambiente', 4),
('Cortina Voil e Forro', 'Par para varão simples', 4);

-- 5. Lavanderia
INSERT INTO TemplateGiftItems (Name, Description, CategoryId) VALUES
('Ferro a Vapor', 'Base cerâmica antiaderente', 5),
('Passadeira a Vapor', 'Portátil para higienizar', 5),
('Aspirador Vertical', '2 em 1 sem fio', 5),
('Mop Giratório', 'Balde com centrífuga', 5),
('Tábua de Passar', 'Estrutura reforçada', 5),
('Varal de Chão Aço', 'Com abas dobráveis', 5),
('Organizadores Lavanderia', 'Cestos e caixas plásticas', 5),
('Escada Alumínio 5 Degraus', 'Leve e segura', 5),
('Lavadora Alta Pressão', 'Para áreas externas e pisos', 5);

-- 6. Casa Inteligente
INSERT INTO TemplateGiftItems (Name, Description, CategoryId) VALUES
('Smart Speaker com Tela', 'Assistente visual 5 polegadas', 6),
('Smart Speaker Mini', 'Assistente de voz compacto', 6),
('Kit Lâmpadas Wi-Fi RGB', 'Controle de cor pelo app', 6),
('Robô Aspirador Passa Pano', 'Limpeza autônoma inteligente', 6),
('Fechadura Digital', 'Senha e biometria', 6),
('Câmera Wi-Fi Interna', 'Monitoramento pelo celular', 6),
('Controle Universal Smart', 'Comanda TV e Ar pelo app', 6),
('Tomada Inteligente', 'Automatize qualquer aparelho', 6),
('Chromecast / Fire Stick', 'Transforma TV em Smart', 6),
('Soundbar Bluetooth', 'Som de cinema para TV', 6);

-- 7. Mesa Posta
INSERT INTO TemplateGiftItems (Name, Description, CategoryId) VALUES
('Aparelho Jantar 30 Peças', 'Porcelana branca clássica', 7),
('Faqueiro Inox 101 Peças', 'Completo com estojo', 7),
('Jogo Taças Vinho', '6 peças cristal ecológico', 7),
('Jogo Copos Água', '6 peças vidro design', 7),
('Sousplats Rattan', 'Kit 6 unidades', 7),
('Jogo Americano', 'Kit 6 lugares impermeável', 7),
('Guardanapos Tecido', 'Linho misto (Kit 6)', 7),
('Anéis de Guardanapo', 'Detalhe dourado/prata', 7),
('Petisqueira Giratória', 'Bambu com cerâmica', 7),
('Tábua Queijos e Frios', 'Com espátulas inclusas', 7),
('Rechaud Cerâmica', 'Mantém alimentos quentes', 7),
('Boleira com Tampa', 'Vidro ou acrílico', 7),
('Garrafa Térmica Mesa', 'Design moderno nórdico', 7);

-- 8. Área Externa
INSERT INTO TemplateGiftItems (Name, Description, CategoryId) VALUES
('Churrasqueira Portátil', 'Carvão com tampa', 8),
('Kit Churrasqueiro', 'Faca, garfo e pegador', 8),
('Tábua Corte Grande', 'Madeira maciça teca', 8),
('Caixa Térmica Cooler', 'Para bebidas em festas', 8),
('Rede de Descanso', 'Tecido algodão resistente', 8),
('Kit Jardinagem', 'Ferramentas básicas', 8),
('Mangueira Mágica', 'Expansível até 15m', 8),
('Lanternas Solares', 'Espetos para jardim', 8),
('Caixa Som Bluetooth', 'Resistente à água (JBL/Similar)', 8);

-- 9. Escritório
INSERT INTO TemplateGiftItems (Name, Description, CategoryId) VALUES
('Cadeira de Escritório', 'Ergonômica com tela mesh', 9),
('Mesa Escrivaninha', 'Estilo industrial ou clean', 9),
('Luminária de Mesa', 'Articulada estilo Pixar', 9),
('Monitor LED 24"', 'Full HD para trabalho', 9),
('Suporte Monitor Articulado', 'Pistão a gás', 9),
('Kit Teclado e Mouse Sem Fio', 'Conforto e praticidade', 9),
('Mousepad Grande (Deskpad)', 'Couro ecológico ou tecido', 9),
('Organizador de Mesa', 'Porta-canetas e papéis', 9),
('Fragmentadora de Papel', 'Segurança para documentos', 9),
('Quadro de Avisos/Planner', 'Para organização semanal', 9),
('Fones de Ouvido Noise Cancelling', 'Foco total no trabalho', 9);

-- 10. Bar e Lazer
INSERT INTO TemplateGiftItems (Name, Description, CategoryId) VALUES
('Adega Climatizada 8 Garrafas', 'Para vinhos na temperatura certa', 10),
('Carrinho de Bar', 'Com rodinhas para sala', 10),
('Kit Caipirinha Completo', 'Coqueteleira, socador e tábua', 10),
('Jogo Copos Whisky', 'Cristal com fundo grosso', 10),
('Balde de Gelo Inox', 'Térmico com pinça', 10),
('Abridor de Vinho Elétrico', 'Praticidade para abrir garrafas', 10),
('Decanter Design', 'Para aerar vinhos tintos', 10),
('Jogo Taças Cerveja/Tulipa', 'Degustação de cervejas especiais', 10),
('Tapete para Bar', 'Emborrachado para balcão', 10),
('Porta-Copos (Bolachas)', 'Kit design criativo', 10);
