# Caixa Eletrônico - Desafio Backend C# (Curso Estartando Devs)

Este projeto é um **desafio proposto pelo curso Estartando Devs**, cujo objetivo era criar um projeto console de caixa eletrônico em C# utilizando **SQLite** como banco de dados local.  

---

## Objetivo

Desenvolver um sistema de caixa eletrônico que permita realizar operações bancárias reais, com persistência de dados no banco SQLite.  

Operações implementadas:  
- Criar conta  
- Depositar  
- Sacar  
- Transferir  
- Consultar saldo  
- Consultar histórico de transações  

---

## 📂 Estrutura de Dados

### Conta
- Número da conta (único, gerado automaticamente)  
- Nome do titular  
- Saldo  

### Transação
- Tipo: Depósito, Saque ou Transferência  
- Valor  
- Data e hora  
- Conta de origem  
- Conta de destino (apenas para transferências)  

---

## 1️⃣ Funcionalidades

### 1. Criar Conta

### 2. Depositar

### 3. Sacar

### 4. Transferir
 
### 5. Consultar Saldo

### 6. Consultar Histórico



## 2️⃣ Persistência (SQLite)

- Banco local leve, armazenando todas as operações  
- Tabelas:
  - **Contas**  
  - **Transacoes**  
- Garantia de que os dados permanecem mesmo após fechar o sistema  

