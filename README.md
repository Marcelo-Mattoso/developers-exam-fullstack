# INSPAND Developers Exam - Full Stack

## Visão Geral

Projeto de avaliação full-stack implementando uma plataforma de gerenciamento de livros com operações CRUD. Algumas funcionalidades foram intencionalmente deixadas não implementadas para você completar.

## Stack Tecnológica

### Backend
- **.NET 10** (SDK 10.0.100)
- **Clean Architecture** com 3 camadas:
  - `Domain` - Lógica de negócio e entidades
  - `Infrastructure` - Acesso a dados e serviços externos
  - `WebAPI` - Endpoints HTTP e controllers
- **Entity Framework Core 10** com SQL Server
- **MediatR** para CQRS e eventos
- **FluentValidation** para validação de domínio

### Frontend
- **Vue 3.5.13** com Options API
- **Vue Router 4.5.0** para roteamento
- **Vite 6** como build tool
- **ESLint** para qualidade de código

## Começando

### Pré-requisitos
- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- [Node.js](https://nodejs.org/) (v18+ recomendado)
- SQL Server 2017+ (ou use Docker - veja abaixo)

### Configuração do Banco de Dados

**Opção 1: Docker (Recomendado)**
```bash
cd back
docker-compose up -d
```

Isso sobe o SQL Server 2017 em `localhost:1433`:
- **Database**: `INSPAND_Exam`
- **Usuário**: `sa`
- **Senha**: `5Gi3re61sXehuSGxMqHX`

**Opção 2: SQL Server Existente**
Atualize a connection string em `back/src/WebAPI/appsettings.json` para apontar para sua instância.

### Rodando o Backend
```bash
cd back
dotnet restore
dotnet build
cd src/WebAPI
dotnet run
```

Backend roda em: `http://localhost:5240`
Swagger UI: `http://localhost:5240/swagger`

### Rodando o Frontend
```bash
cd front
npm install
npm run dev
```

Frontend roda em: `http://localhost:5173`

Chamadas à API em `/api/*` são automaticamente redirecionadas para o backend.

## O Desafio

Construir um sistema de gerenciamento de livros com operações CRUD completas.

### Histórias de Usuário

**Como administrador, desejo:**
- Visualizar uma lista de todos os livros na home page (com paginação)
- Criar novos livros
- Editar livros existentes
- Excluir livros

### Campos Obrigatórios

Cada livro deve ter:
- **Título** (10-100 caracteres)
- **Autor** (10-100 caracteres)
- **Descrição** (máximo 1024 caracteres)

### Regras de Validação

- ❌ Não permitir títulos duplicados (exibir mensagem de erro)
- ❌ Título deve ter entre 10-100 caracteres (exibir mensagem de validação)
- ❌ Autor deve ter entre 10-100 caracteres (exibir mensagem de validação)
- ❌ Descrição deve ter no máximo 1024 caracteres (exibir erro quando exceder)

### Bônus (Opcional)

Disparar um evento de domínio para enviar um e-mail para `developers@inspand.com.br` quando um livro for criado.

> **Observação**: Apenas implemente o disparo do evento - não é necessário implementar o envio real do e-mail. O `IEmailService` já está preparado.

## Feedback Esperado

Junto com sua implementação, forneça seu entendimento sobre a arquitetura:

1. **Qual o papel da camada Domain?**
2. **Qual o papel da camada Infrastructure?**
3. **Qual o papel da camada WebAPI?**
4. **Aponte um ponto de melhoria** que considere relevante para o projeto.

## Tempo Limite

Você tem **5 horas** para completar o desafio e fornecer o feedback.

## O Que Avaliamos

- ✅ Qualidade e organização do código
- ✅ Entendimento dos princípios de Clean Architecture
- ✅ Implementação de validações
- ✅ Design da API
- ✅ Integração Frontend/Backend
- ✅ Seu feedback e insights

**Não se preocupe se não conseguir finalizar tudo** - valorizamos qualidade acima de conclusão e queremos ver como você aborda problemas.

---

**Boa sorte!** 🚀

*Powered by INSPAND Developers*