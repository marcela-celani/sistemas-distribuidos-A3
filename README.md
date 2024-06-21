# Projeto de Sistemas Distribuídos
Universidade Potiguar - Curso de Análise e Desenvolvimento de Sistemas  
Disciplina: Sistemas Distribuídos

## Objetivo
Desenvolver uma aplicação distribuída (aplicação desktop), utilizando a tecnologia .NET (C#), com comunicação entre os componentes e a realização de operações básicas (CRUD) com bando de dados MongoDB.

## Índice
- [Descrição do Projeto](#descrição-do-projeto)
- [Funcionalidades](#funcionalidades)
- [Requisitos](#requisitos)
- [Instalação](#instalação)
- [Uso](#uso)
- [Contribuição](#contribuição)
- [Autores](#autores)

## Descrição do Projeto
Este projeto consiste em uma aplicação distribuída para cadastro de tarefas. A aplicação permite a comunicação entre API e banco de dados, realizando operações CRUD (Create, Read, Update, Delete) e gerenciada pelo Swagger.

## Funcionalidades
- **Cadastro de Tarefas**: Adicionar novas tarefas ao sistema.
- **Visualização de Tarefas**: Listar todas as tarefas cadastradas.
- **Edição de Tarefas**: Atualizar informações de tarefas existentes.
- **Exclusão de Tarefas**: Remover tarefas do sistema.
- **Comunicação entre Componentes**: Permitir a interação e troca de dados entre diferentes partes da aplicação.
- **Suporte a Banco de Dado**: Integrado com MongoDB.

Autenticação e Autorização:

- Apenas usuários cadastrados podem autenticar no sistema através do endpoint de login.
- O sistema utiliza tokens JWT para autenticação, garantindo que apenas usuários válidos e autenticados possam acessar recursos protegidos.

Gerenciamento de Itens:

- Apenas usuários autenticados podem criar novos itens.
- Cada item criado é associado ao usuário que o criou, garantindo que apenas o dono do item possa modificá-lo ou excluí-lo.
- 
Validações de Dados:

- Todos os dados recebidos através dos endpoints são validados para garantir integridade e consistência.
- Validações incluem verificação de formatos de dados corretos (como e-mails válidos) e restrições de tamanho ou tipo de dados.

## Requisitos
- **Tecnologias**: .NET (C#)
- **Banco de Dados**: MongoDB
- **Ambiente de Desenvolvimento**:
  - Para .NET: Visual Studio
- **Outros**: Git para controle de versão

## Instalação

### Pré-requisitos
- Instale o [Visual Studio](https://visualstudio.microsoft.com/) ou [Visual Studio Code](https://code.visualstudio.com/) para .NET.
- Instale o [Git](https://git-scm.com/).

### Passos
1. Clone o repositório:
    ```bash
    git clone https://github.com/marcela-celani/sistemas-distribuidos-A3.git
    ```
2. Navegue para o diretório do projeto:
    ```bash
    cd web-api
    ```
3. Instale as dependências.

## Uso

### Executando a Aplicação
1. Abra o projeto no seu ambiente de desenvolvimento (Visual Studio ou Visual Studio Code).
2. Execute a aplicação e abra o link em seu navegador utilizando localhost.

## Contribuição
1. Faça um fork do projeto.
2. Crie uma branch para sua feature (`git checkout -b feature/nome-da-feature`).
3. Commit suas mudanças (`git commit -m 'Adicionei uma nova feature'`).
4. Faça o push para a branch (`git push origin feature/nome-da-feature`).
5. Abra um Pull Request.

## Autores
- Marcela Celani - Desenvolvedora
- Lorena Celani - Desenvolvedora
- Wanfranklin Alves - Orientador - Universidade Potiguar
