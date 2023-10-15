# Sistema de fluxo de caixa
Uma api para que um usuário possa cadastrar suas movimentações de caixa de entrada e saída e ao final do dia ter um relatório consolidado de seu caixa diáriamente

## Funcionalidades
* Executar Login e visualizar o seu perfil em /users
* Cadastrar entradas e saídas do caixa em POST /transactions
* Visualizar suas entradas em GET /transactions
* Visualizar os dados consolidados por dia em /dailycashflow

## Requisitos não funcionais
* Por ser containerizada, há a possibilidade de alta disponibilidade, pois pode ser clusterizado em um cluster docker swarm, em um cluster kubernetes ou utilizando o mecanismos de rodar containeres em cloud
* Alto desempenho por utilizar um banco NoSql como o MongoDb, devido aos seus dados serem lineares e praticamente imutáveis, não há problemas de concorrencia nem de integridade
* Operações assíncronas utilizando mensageria garantem que não haja perda de informação além de proporcionar possibilidades de reprocessamento com facilidade, isso porquê é utilizado a tecnica de event sourcing, indexação de dados processamento de streams com o kafka
* Sistema altamente auditável devido a sua natureza de imutabilidade e orientação a eventos
* Por utilizar microserviços todos os seus componentes são escaláveis individualmente, ajudando no processo de finops e ajuste fino do processamento da aplicação

## Stack Tecnológica

* **C# .Net Core 6.0:** Linguagem de programação C# da microsoft acompanhada de seu framework o .Net Core 
* **Kafka:** Sistema de mensageria baseado em na pattern log appender, altamente clusterizável, escalável e flexível, conta com streaming de dados
* **KsqlDB:** Abstrai o processamento de streaming do kafka em uma linguagem SQL ANSI, facilita muito o processamento de stream e tables do Kafka
* **MongoDB:** Banco de dados NoSql extremamente performático, clusterizável e escalável
* **Python 3.10:** Linguagem de programação extremamente simples e ergonomica, aqui usada para automção de processos administrativos
* **Docker:** Tecnologia de containerização de aplicações, permitindo rodar sub-kernels de forma isolada, proporcionando assim a abstração do SO sem usar virtualização
* **NGINX:** Utilizando o nginx para fazer o gateway das apis, fazendo uso de suas funcionalidades de proxy reverso (proxy_pass) e de balanceamento de carga (upstream)

### Libraries importantes
* **Microsoft.AspNetCore.Authentication.JwtBearer:** Usada para autenticação JWT das apis
* **Confluent.Kafka:** Biblioteca oficial da confluent para acesso ao Kafka
* **MongoDB.Driver:** Biblioteca simples e eficiente para acesso ao MongoDB

## Padrões Arquiteturais
* **Microservices:** Técnica para desacoplar os endpoints em aplicações apartadas, gerando a possibilidade de escalar horizontalmente e individualmente cada componente da aplicação
* **Event Driven Architecture:** Utilizar eventos de negócio para comunicação entre os componentes da aplicação, promovendo desacoplamento e escalabilidade
* **Clean Architecture:** Técnica que visa isolar o domínio da aplicação que por sua vez é isolado do framework, gerando reusabilidade dessas camadas, isso é perfeito para o cenário de microserviços, aonde acaba se promovendo patterns de strategy, adapters e mediators
    ![Clean Architecture](/documentation/clean_arch.png)
* **Clean Code:** Técnicas de promover um código mais limpo para a aplicação, utilizando de conceitos do SOLID entre outros
* **Event Sourcing:** Técnica aonde as alterações nos dados são dadas como eventos em uma linha do tempo, as informações são gravadas dessa forma, permitindo auditar e até mesmo voltar no tempo para reconsolidar o dado final a qualquer momento
  ![Event Sourcing](/documentation/event-sourcing-overview.png)
* **Domain Driven Design:** Conjunto de técnicas para fazer o seu desenvolvimento orientado ao domínio, ou seja, os objetos de negócio, essas técnivas visam promover uma linguagem clara entre o time de desenvolvimento e time de negócio enquanto ajudar a montar os modelos de separação dos componentes baseado no modelo de dados da aplicação
  
  __Building Blocks__<br>
  ![DDD](/documentation/ddd.png)
  
  __Contextos Delimitados__<br>
  ![Bounded Contexts](/documentation/bounded_context.png)

## Desenhos da solução
Temos aqui o desenho ideal da solução, nesses desenhos também estão sendo mencionadas a técnologias ideais para rodar a solução em cloud como o GCP Cloud Run, GKE (Kubernetes), e GCP Cloud Compute, porém as mesmas não estão incorporadas no código nesse momento

* **Componentes:** ![componets](/documentation/cashflow-Containers.png)
* **Infraestrutura:** ![infrastructure](/documentation/cashflow-Infrastructure.png)
* **Diagrama de sequência:** ![sequence](/documentation/cashflow-Fluxo.png)

## Engenharia da solução

O projeto é um monorepo dividido em algumas pastas importantes:

* **microservices** Contém aplicações .net webapi e workers para serem deployados separadamente, todos eles possuem uma referencia para o package Infrastructure que contem as implementações dos repositorios e serviços
  * **Users** Microserviço responsável pelo login e perfil do usuário, responsável por gerar os tokens JWT
  * **Transactions** Microserviço que recebe as movimentações de caixa, persiste e dispara para o kafka
  * **DailyCashFlow** Microserviço que disponibiliza a listagem de consolidados diários para o usuário
  * **DailyCashFlowConsumer** Microserviço que consome o resultado do processamento de stream do ksql e persiste na base mongo para que seja disponibilizado ao usuário
* **packages** Contém toda a logica de negocio dividido em camadas bem definidas
  * **Domain** Possui a definição dos objetos de negócio, aqueles que serão persistidos e usados para compor a lógica de negócio, essa camada deve ser o mais pura possível, ou seja, não possuir acoplamento com frameworks e libs
  * **Application** Contém os serviços que realizam a lógica de negócio, ele possui acesso apenas a camada de domain e define interfaces quando precisa usar libs e processos externos, dessa forma ele se mantem puro e reutilizável com diferentes técnologias e libraries
  * **Infrastructure** Camada de interface pública, faz a cola entre o framework e a camada de negócio (Application), essa camada conhece as abstrações de banco de dados, frameworks, etc, ela expõe a injeção de dependencia como extension do .net para que os microserviços possam utilizar
* **console** Pequena aplicação desenvolvida em python para automatizar tarefas administrativas, como criação de usuários e indices de tabelas mongo
* **ksqldb** Possui os scripts SQL que o KsqlDB vai utilizar para processar as streams e consolidar os dados de fluxo de caixa
* **nginx** Possui as configurações de proxy reverso e balanceamento de carga do nginx
* **tests** Possui os tests unitários da aplicação

## Rodando o projeto

* Instale o docker em sua máquina
* Rode o comando $ docker compose up -d
* Inicialize os indexes $ docker compose run console create-indexes
* Inicialize os indexes $ docker compose run console create-user nome email senha
* O projeto está configurado para rodar na porta 80, portanto deixe essa porta livre
* acesse localhost/users/swagger para acessar a api de usuários, realize o login na rota /login
* Utilize o token gerado para acessar as rotas da api de transactions localhost/transactions/swagger (existe um botão authorize no canto superior direito, use o token gerado)
* para se certificar que os dados estão sendo gerados no kafka, acesso o control center em  localhost:9021, observe os topicos transaction_created e DAILY_CASH_FLOW_TABLE
* Acesse a api de daily cash flow em localhost/dailycashflow para listar o caixa consolidado diário

## Débitos técnicos

* Pouca cobertura de código, foi feita apenas um teste unitário para mostrar o uso do conceito
* O serviço de consumo de dados consolidados está faltando uma melhor abstração do consumo
* Não foi possível manter a camada de domain 100% pura, pois o driver de mongodb do .net é intrusivo e não permite mapeamento externo, todavia foi usado uma tipagem de string no id facilitando a portabilidade um pouco para outros frameworks, é possível também usa-los como DTO em cahamads de internas HTTP
* Não foi usada uma camada de DTO, o layer de transporte é a própria camada de domínio, algumas pessoa podem achar isso ruim, porém eu acho mais produtivo, seria interessante se o framework de serialização permitisse mapeamento externo para mater as classes puras, isso também acaba dificultando melhor documentação no swagger
  