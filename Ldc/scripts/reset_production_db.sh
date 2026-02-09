#!/bin/bash

# Configuration
DB_USER="ldcuser"
DB_PASS="Hp353637?Hp;"
DB_NAME="ldcdb"
SERVICE_NAME="lacos-api"

echo "⚠️  ATENÇÃO: ESTE SCRIPT IRÁ APAGAR TODO O BANCO DE DADOS DE PRODUÇÃO!"
echo "Você tem certeza que deseja continuar? (y/n)"
read confirmation

if [ "$confirmation" != "y" ]; then
    echo "Operação cancelada."
    exit 1
fi

echo "🛑 Parando o serviço backend..."
sudo systemctl stop $SERVICE_NAME

echo "🗑️  Dropando e recriando o banco de dados..."
mysql -u $DB_USER -p"$DB_PASS" -e "DROP DATABASE IF EXISTS $DB_NAME; CREATE DATABASE $DB_NAME;"

if [ $? -ne 0 ]; then
    echo "❌ Erro ao recriar o banco de dados."
    exit 1
fi

echo "🚀 Iniciando o serviço backend (disparando migrations)..."
sudo systemctl start $SERVICE_NAME

echo "⏳ Aguardando migrations (10 segundos)..."
sleep 10

echo "🌱 Populando sugestões de presentes..."
mysql -u $DB_USER -p"$DB_PASS" $DB_NAME < seed_suggestions.sql

if [ $? -ne 0 ]; then
    echo "❌ Erro ao popular dados."
    exit 1
fi

echo "✅ Banco de dados resetado e populado com sucesso!"
