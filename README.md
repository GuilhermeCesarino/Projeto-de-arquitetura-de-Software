# BibliotecaMicroservices

Sistema de gerenciamento de biblioteca construído com arquitetura de microsserviços em .NET 8.

---

## Documento de Requisitos

### 1. Propósito do Sistema

O sistema tem como objetivo gerenciar as operações de uma biblioteca, permitindo o cadastro de usuários e livros, além do controle de empréstimos e devoluções. O sistema automatiza a verificação de disponibilidade de exemplares, o controle de empréstimos ativos e a aplicação de bloqueios a usuários com devoluções em atraso.

---

### 2. Usuários do Sistema

| Perfil | Descrição |
|---|---|
| **Bibliotecário** | Responsável por cadastrar livros, gerenciar usuários e registrar empréstimos/devoluções |
| **Leitor** | Usuário cadastrado que pode realizar empréstimos de livros |

---

### 3. Requisitos Funcionais

#### Usuários
- **RF01** — O sistema deve permitir cadastrar um novo usuário informando nome, e-mail e telefone.
- **RF02** — O sistema deve permitir listar todos os usuários cadastrados.
- **RF03** — O sistema deve permitir buscar um usuário pelo seu ID.
- **RF04** — O sistema deve permitir atualizar os dados de um usuário.
- **RF05** — O sistema deve permitir remover um usuário do sistema.
- **RF06** — O sistema deve permitir bloquear um usuário, impedindo-o de realizar novos empréstimos.

#### Livros
- **RF07** — O sistema deve permitir cadastrar um novo livro informando título, autor, ISBN, ano de publicação e quantidade de exemplares.
- **RF08** — O sistema deve permitir listar todos os livros cadastrados.
- **RF09** — O sistema deve permitir buscar um livro pelo seu ID.
- **RF10** — O sistema deve permitir atualizar os dados de um livro.
- **RF11** — O sistema deve permitir remover um livro do sistema.
- **RF12** — O sistema deve controlar a quantidade de exemplares disponíveis de cada livro.

#### Empréstimos
- **RF13** — O sistema deve permitir registrar um empréstimo de livro para um usuário, definindo a data prevista de devolução.
- **RF14** — O sistema deve impedir o empréstimo se o usuário estiver bloqueado.
- **RF15** — O sistema deve impedir o empréstimo se não houver exemplares disponíveis do livro.
- **RF16** — O sistema deve reduzir a quantidade de exemplares disponíveis ao registrar um empréstimo.
- **RF17** — O sistema deve permitir registrar a devolução de um livro, atualizando a disponibilidade.
- **RF18** — O sistema deve permitir listar todos os empréstimos cadastrados.
- **RF19** — O sistema deve permitir listar os empréstimos de um usuário específico.
- **RF20** — O sistema deve verificar empréstimos em atraso e bloquear automaticamente os usuários responsáveis.

---

## Descritivo Técnico

### Tecnologias Utilizadas

- **Plataforma:** .NET 8
- **Banco de dados:** SQLite (um banco por microsserviço)
- **ORM:** Entity Framework Core 8
- **Comunicação entre serviços:** REST (HTTP/JSON via `HttpClient`)
- **Documentação:** Swagger / OpenAPI

---

### Microsserviços

#### 1. Biblioteca.Usuarios — Porta 5001

Responsável pelo cadastro e gerenciamento dos usuários da biblioteca.

**Endpoints:**

| Método | Rota | Descrição |
|---|---|---|
| GET | `/api/Usuarios` | Lista todos os usuários |
| GET | `/api/Usuarios/{id}` | Busca usuário por ID |
| POST | `/api/Usuarios` | Cadastra novo usuário |
| PUT | `/api/Usuarios/{id}` | Atualiza dados do usuário |
| DELETE | `/api/Usuarios/{id}` | Remove usuário |
| PATCH | `/api/Usuarios/{id}/bloquear` | Bloqueia usuário |

**Banco de dados:** `usuarios.db`

---

#### 2. Biblioteca.Livros — Porta 5002

Responsável pelo catálogo de livros e controle de disponibilidade de exemplares.

**Endpoints:**

| Método | Rota | Descrição |
|---|---|---|
| GET | `/api/Livros` | Lista todos os livros |
| GET | `/api/Livros/{id}` | Busca livro por ID |
| POST | `/api/Livros` | Cadastra novo livro |
| PUT | `/api/Livros/{id}` | Atualiza dados do livro |
| DELETE | `/api/Livros/{id}` | Remove livro |
| PATCH | `/api/Livros/{id}/disponibilidade` | Atualiza quantidade disponível |

**Banco de dados:** `livros.db`

---

#### 3. Biblioteca.Emprestimos — Porta 5003

Responsável pelo controle de empréstimos e devoluções. É o microsserviço orquestrador, que se comunica com os outros dois para validar e atualizar informações.

**Endpoints:**

| Método | Rota | Descrição |
|---|---|---|
| GET | `/api/Emprestimos` | Lista todos os empréstimos |
| GET | `/api/Emprestimos/{id}` | Busca empréstimo por ID |
| GET | `/api/Emprestimos/usuario/{usuarioId}` | Lista empréstimos de um usuário |
| POST | `/api/Emprestimos` | Registra novo empréstimo |
| PATCH | `/api/Emprestimos/{id}/devolver` | Registra devolução |
| POST | `/api/Emprestimos/verificar-atrasados` | Verifica e processa atrasos |

**Banco de dados:** `emprestimos.db`

---

### Integrações entre Microsserviços

O microsserviço **Biblioteca.Emprestimos** consome os outros dois por meio de chamadas HTTP REST. As três integrações exigidas pelo trabalho estão implementadas conforme descrito abaixo:

#### Integração 1 — Busca de dados: Empréstimos → Usuários
Ao registrar um novo empréstimo (`POST /api/Emprestimos`), o serviço consulta o microsserviço de Usuários (`GET /api/Usuarios/{id}`) para validar se o usuário existe e se está ativo (não bloqueado).

#### Integração 2 — Busca de dados: Empréstimos → Livros
Ao registrar um novo empréstimo, o serviço consulta o microsserviço de Livros (`GET /api/Livros/{id}`) para validar se o livro existe e se há exemplares disponíveis.

#### Integração 3 — Alteração de dados: Empréstimos → Usuários e Livros
Existem dois fluxos de alteração:
- **Empréstimo/Devolução:** o serviço chama `PATCH /api/Livros/{id}/disponibilidade` para decrementar ou incrementar a quantidade de exemplares disponíveis.
- **Verificação de atrasos:** o serviço chama `PATCH /api/Usuarios/{id}/bloquear` para bloquear usuários com empréstimos em atraso.

---

### Como Executar

Cada microsserviço deve ser iniciado em um terminal separado:

```bash
# Terminal 1 — Usuários
cd Biblioteca.Usuarios
dotnet run

# Terminal 2 — Livros
cd Biblioteca.Livros
dotnet run

# Terminal 3 — Empréstimos
cd Biblioteca.Emprestimos
dotnet run
```

Após iniciar, os Swaggers estarão disponíveis em:
- Usuários: http://localhost:5001/swagger
- Livros: http://localhost:5002/swagger
- Empréstimos: http://localhost:5003/swagger

> **Importante:** os microsserviços de Usuários e Livros devem estar rodando antes de utilizar o serviço de Empréstimos.

---

### Estrutura do Projeto

```
BibliotecaMicroservices.sln
├── Biblioteca.Usuarios/
│   ├── Controllers/UsuariosController.cs
│   ├── DTO/UsuarioDTO.cs
│   ├── Services/ServUsuario.cs
│   ├── Models/Usuario.cs
│   ├── Infra/GeradorDeServicos.cs
│   └── DataContext.cs
│
├── Biblioteca.Livros/
│   ├── Controllers/LivrosController.cs
│   ├── DTO/LivroDTO.cs
│   ├── Services/ServLivro.cs
│   ├── Models/Livro.cs
│   ├── Infra/GeradorDeServicos.cs
│   └── DataContext.cs
│
└── Biblioteca.Emprestimos/
    ├── Controllers/EmprestimosController.cs
    ├── DTO/EmprestimoDTO.cs
    ├── Services/ServEmprestimo.cs
    ├── Models/Emprestimo.cs
    ├── Clients/
    │   ├── UsuarioClient.cs
    │   └── LivroClient.cs
    ├── Infra/GeradorDeServicos.cs
    └── DataContext.cs
```
