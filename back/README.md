# Backend - Arquitetura e Feedback

## 1) Qual o papel da camada Domain?

A camada Domain concentra as regras de negócio e os contratos centrais do sistema.

Ela define:
- Entidades de domínio (ex.: `Book`) com validações de negócio;
- Eventos de domínio (ex.: `BookCreatedDomainEvent`);
- Interfaces/abstrações (repositórios, serviços e integrações como e-mail);
- Exceções de negócio (`NotFound`, `Conflict`, `Validation`) sem acoplamento a framework web ou banco.

Objetivo principal: manter a lógica de negócio isolada, testável e independente de tecnologia.

## 2) Qual o papel da camada Infrastructure?

A camada Infrastructure implementa os contratos definidos no Domain e conecta o sistema com recursos externos.

Ela é responsável por:
- Persistência com EF Core (`SqlDbContext`, mappings, repositórios);
- Configuração de DI e infraestrutura da aplicação;
- Publicação e tratamento de eventos com MediatR;
- Serviço de e-mail fake, preparado para integração real com Azure via configuração.

Objetivo principal: materializar tecnicamente o que o domínio precisa, sem mover regras de negócio para fora do Domain.

## 3) Qual o papel da camada WebAPI?

A camada WebAPI expõe o backend via HTTP e orquestra entrada/saída da aplicação.

Ela cuida de:
- Endpoints REST para CRUD de livros;
- Contratos de request/response e paginação;
- Documentação da API com Swagger (incluindo respostas de sucesso e erro);
- Middleware global para padronização de erros HTTP (400/404/409/500).

Objetivo principal: ser a “porta de entrada” da aplicação, traduzindo HTTP para casos de uso do domínio.

## 4) Ponto(s) de melhoria relevante(s)

### 4.1 Testes unitários
Já foi adicionada base de testes unitários, cobrindo:
- Validações da entidade `Book`;
- Regras do `BookService` (duplicidade, not found, paginação e criação);
- Middleware de erros (400/404/409/500).

Evolução recomendada: incluir testes de integração HTTP + banco (ambiente de teste), para validar pipeline completo.

### 4.2 Swagger
A API já está documentada com respostas de sucesso e erro.

Evolução recomendada: adicionar exemplos de payload (`example`) e descrições mais detalhadas por endpoint para facilitar consumo por frontend e terceiros.

### 4.3 Middleware para validação de códigos HTTP
Já existe middleware central para tratamento de exceções e padronização de resposta de erro.

Evolução recomendada: criar catálogo padronizado de códigos de erro de negócio (ex.: `BOOK_TITLE_DUPLICATE`) para melhorar rastreabilidade e UX no frontend.

### 4.4 Domínio plug-and-play para Azure
O fluxo de evento de criação de livro já está pronto para integração de e-mail:
- Evento de domínio disparado no create;
- Handler de infraestrutura responsável pelo envio;
- `IEmailService` desacoplado;
- Opções de configuração Azure (`ConnectionString`, `SenderAddress`, `RecipientAddress`).

Evolução recomendada: implementar um provider real de Azure Communication Services mantendo o mesmo contrato (`IEmailService`), sem impacto na camada de domínio.
