# ChronoSync

![Lacuna Dev Admission](https://cdn.lacunasoftware.com/lab/img/probes.png)

ChronoSync is a .NET console application for the Lacuna Dev Admission challenge. It connects to the Luma APIs, synchronizes probe clocks using encoded timestamps, and processes jobs until the server returns `Done`.

## English

### What it does

- Starts a test context and retrieves an access token.
- Loads the probe list and creates one synchronized clock per probe.
- Supports all required timestamp encodings: `Iso8601`, `Ticks`, `TicksBinary`, and `TicksBinaryBigEndian`.
- Computes offset and round-trip using the four-timestamp sync model.
- Processes jobs until the API reports completion.
- Handles `Unauthorized`, `Fail`, and `ProbeUnreachable` according to the challenge rules.
- Supports the level 2 scenario with solar-wind retries and time dilation.

### Design

The project is intentionally modular:

- `Clients`: API integration with Luma.
- `Codecs`: timestamp encoding and decoding strategies.
- `Clocks`: synchronized probe clock state and registry.
- `Services`: workflow orchestration, probe synchronization, and job processing.
- `Configuration`: strongly typed application settings.
- `Helpers`: shared tick and time utilities.

### Workflow

1. Start a new test context.
2. Fetch the probes.
3. Sync each probe clock using the encoding declared by the probe.
4. Take jobs and answer them with the probe-specific encoded timestamp and round-trip.
5. Restart the context when the API tells us the session is no longer valid.

### Configuration

Edit `src/ChronoSync/appsettings.json`:

```json
{
  "Luma": {
    "BaseUrl": "https://luma.lacuna.cc",
    "Username": "your-username",
    "Email": "your-email@example.com",
    "Level": 1
  }
}
```

Set `Level` to `2` to enable the stronger solar-wind scenario.

### Run

```bash
dotnet run --project src/ChronoSync/ChronoSync.csproj
```

Build the solution with:

```bash
dotnet build ChronoSync.sln
```

### Validation

The solution compiles successfully with:

```bash
dotnet build c:\Users\HP\ChronoSync\ChronoSync.sln
```

### Project Scripts

See [TEST_SCRIPTS.txt](TEST_SCRIPTS.txt) for the copy-ready command list to validate the project end to end.

### Notes

- The app uses `DateTimeOffset.UtcNow.Ticks` as the canonical timestamp source.
- `ProbeUnreachable` waits 5 seconds before retrying.
- Time dilation is supported through the optional `timeDilationFactor` field.
- This repository should remain private unless the challenge instructions say otherwise.

---

## Português BR

ChronoSync é uma aplicação de console em .NET para o desafio Lacuna Dev Admission. Ela se conecta às APIs da Luma, sincroniza os relógios dos probes usando timestamps codificados e processa os jobs até o servidor retornar `Done`.

### O que o projeto faz

- Inicia um contexto de teste e obtém o access token.
- Carrega a lista de probes e cria um relógio sincronizado para cada um.
- Suporta todos os encodings exigidos: `Iso8601`, `Ticks`, `TicksBinary` e `TicksBinaryBigEndian`.
- Calcula offset e round-trip pelo modelo de quatro timestamps.
- Processa jobs até a API indicar conclusão.
- Trata `Unauthorized`, `Fail` e `ProbeUnreachable` conforme as regras do desafio.
- Suporta o cenário de level 2 com retries por ventos solares e dilatação de tempo.

### Estrutura

O projeto foi organizado de forma modular:

- `Clients`: integração com a API da Luma.
- `Codecs`: estratégias de encode e decode de timestamps.
- `Clocks`: estado do relógio sincronizado por probe e registry.
- `Services`: orquestração do fluxo, sincronização dos probes e processamento dos jobs.
- `Configuration`: configurações fortemente tipadas.
- `Helpers`: utilitários compartilhados de ticks e tempo.

### Fluxo

1. Inicia um novo contexto de teste.
2. Busca os probes.
3. Sincroniza cada relógio usando o encoding informado pelo probe.
4. Toma jobs e responde com o timestamp codificado do probe e o round-trip.
5. Reinicia o contexto quando a API indicar que a sessão ficou inválida.

### Configuração

Edite `src/ChronoSync/appsettings.json`:

```json
{
  "Luma": {
    "BaseUrl": "https://luma.lacuna.cc",
    "Username": "your-username",
    "Email": "your-email@example.com",
    "Level": 1
  }
}
```

Altere `Level` para `2` para ativar o cenário com ventos solares mais fortes.

### Execução

```bash
dotnet run --project src/ChronoSync/ChronoSync.csproj
```

Para compilar a solução:

```bash
dotnet build ChronoSync.sln
```

### Validação

A solução compila com sucesso com:

```bash
dotnet build c:\Users\HP\ChronoSync\ChronoSync.sln
```

### Scripts do Projeto

Veja [TEST_SCRIPTS.txt](TEST_SCRIPTS.txt) para o conjunto de comandos prontos para validar o projeto inteiro.

### Observações

- O app usa `DateTimeOffset.UtcNow.Ticks` como fonte principal de timestamp.
- `ProbeUnreachable` aguarda 5 segundos antes de tentar novamente.
- A dilatação de tempo é suportada pelo campo opcional `timeDilationFactor`.
- Este repositório deve permanecer privado, salvo orientação em contrário.

# ChronoSync

![Lacuna Dev Admission](https://cdn.lacunasoftware.com/lab/img/probes.png)

ChronoSync is a .NET console application built for the Lacuna Dev Admission challenge. It communicates with the Luma APIs, synchronizes probe clocks using encoded timestamps, and processes jobs until the server returns the final Done response.

## Highlights

- End-to-end orchestration from Start to Job processing
- Timestamp codec abstraction for Iso8601, Ticks, TicksBinary, and TicksBinaryBigEndian
- Probe clock synchronization with NTP-style offset and round-trip calculations
- Automatic retry for recoverable cases such as Unauthorized, Fail, and ProbeUnreachable
- Level 2 support with solar-wind resilience and time dilation awareness
- Clean dependency injection wiring and terminal-friendly execution

## Architecture

The project is intentionally small and modular:

- Clients: API communication with Luma
- Codecs: timestamp encoding and decoding strategies
- Clocks: synchronized probe clock state and registry
- Services: workflow orchestration, probe sync, and job processing
- Configuration: strongly typed app settings for Luma credentials and base URL
- Helpers: shared tick and time utilities

## How It Works

1. Start a test context through the Start API.
2. Load the probe list.
3. Build one synchronized clock per probe based on its declared encoding.
4. Keep taking jobs and answer them with the probe-specific encoded timestamp and the latest computed round-trip.
5. Restart the context automatically when the API returns recoverable codes that invalidate the current session.

## Requirements Coverage

This solution addresses the challenge requirements as follows:

- Start API: implemented through the shared Luma client.
- Probe listing: implemented through the shared Luma client.
- Timestamp handling: implemented with dedicated codecs and `DateTimeOffset.UtcNow.Ticks`.
- Sync algorithm: implemented in the probe sync service using the four timestamp model.
- Job processing: implemented in the job processor service.
- Level 2 support: the start route switches to `/api/start/2` when configured.
- ProbeUnreachable: handled with a 5 second retry delay.
- Time dilation: supported through the optional `timeDilationFactor` field on probes.

## Configuration

Edit `src/ChronoSync/appsettings.json`:

```json
{
  "Luma": {
    "BaseUrl": "https://luma.lacuna.cc",
    "Username": "your-username",
    "Email": "your-email@example.com",
    "Level": 1
  }
}
```

Set `Level` to `2` if you want the stronger solar winds challenge mode.

## Running

```bash
dotnet run --project src/ChronoSync/ChronoSync.csproj
```

Build the solution with:

```bash
dotnet build ChronoSync.sln
```

## Implementation Notes

- `WorkflowService` is the top-level coordinator.
- `ProbeSyncService` handles the sync protocol and computes offset and round-trip.
- `JobProcessorService` retrieves jobs and sends answers using the synchronized probe clock.
- `ProbeClock` keeps track of the synchronized state for each probe and exposes the encoded current timestamp.
- `ServiceCollectionExtensions.AddChronoSync(...)` registers everything needed by the console host.

## Validation

The solution currently builds successfully with:

```bash
dotnet build c:\Users\HP\ChronoSync\ChronoSync.sln
```

## License

This repository was created for the Lacuna Software admission challenge and should remain private unless the challenge instructions say otherwise.

---

# ChronoSync

ChronoSync é uma aplicação de console em .NET criada para o desafio Lacuna Dev Admission. Ela se comunica com as APIs da Luma, sincroniza relógios de probes usando timestamps codificados e processa jobs até receber a resposta final Done.

## Destaques

- Orquestração ponta a ponta do Start até o processamento de jobs
- Abstração de codecs para Iso8601, Ticks, TicksBinary e TicksBinaryBigEndian
- Sincronização de relógio com cálculo de offset e round-trip no estilo NTP
- Retry automático para casos recuperáveis como Unauthorized, Fail e ProbeUnreachable
- Suporte ao Level 2 com tolerância a ventos solares e dilatação de tempo
- Injeção de dependência limpa e execução amigável via terminal

## Arquitetura

O projeto foi organizado de forma enxuta e modular:

- Clients: comunicação com a API da Luma
- Codecs: estratégias de encode e decode de timestamp
- Clocks: estado do relógio sincronizado por probe e registry central
- Services: orquestração do fluxo, sincronização e processamento de jobs
- Configuration: configurações fortemente tipadas para credenciais e base URL
- Helpers: utilitários compartilhados de ticks e tempo

## Como Funciona

1. Inicia um contexto de teste via Start API.
2. Carrega a lista de probes.
3. Cria um relógio sincronizado para cada probe de acordo com o encoding informado.
4. Continua consumindo jobs e responde com o timestamp codificado do probe e o round-trip mais recente.
5. Reinicia automaticamente o contexto quando a API retorna códigos recuperáveis que invalidam a sessão.

## Cobertura Dos Requisitos

Esta solução atende aos requisitos do desafio da seguinte forma:

- Start API: implementada pelo client compartilhado da Luma.
- Listagem de probes: implementada pelo client compartilhado da Luma.
- Tratamento de timestamp: implementado com codecs dedicados e `DateTimeOffset.UtcNow.Ticks`.
- Algoritmo de sync: implementado no service de sincronização usando quatro timestamps.
- Processamento de jobs: implementado no service de jobs.
- Suporte ao Level 2: a rota de start muda para `/api/start/2` quando configurada.
- ProbeUnreachable: tratado com espera de 5 segundos antes de tentar novamente.
- Dilatação de tempo: suportada pelo campo opcional `timeDilationFactor` nos probes.

## Configuração

Edite `src/ChronoSync/appsettings.json`:

```json
{
  "Luma": {
    "BaseUrl": "https://luma.lacuna.cc",
    "Username": "your-username",
    "Email": "your-email@example.com",
    "Level": 1
  }
}
```

Altere `Level` para `2` se quiser executar o modo com ventos solares mais severos.

## Execução

```bash
dotnet run --project src/ChronoSync/ChronoSync.csproj
```

Para compilar a solução:

```bash
dotnet build ChronoSync.sln
```

## Notas De Implementação

- `WorkflowService` é o orquestrador principal.
- `ProbeSyncService` executa o protocolo de sync e calcula offset e round-trip.
- `JobProcessorService` busca jobs e envia as respostas usando o relógio sincronizado do probe.
- `ProbeClock` mantém o estado sincronizado de cada probe e gera o timestamp atual codificado.
- `ServiceCollectionExtensions.AddChronoSync(...)` registra tudo o que o host de console precisa.

## Validação

A solução compila com sucesso com:

```bash
dotnet build c:\Users\HP\ChronoSync\ChronoSync.sln
```

## Licença

Este repositório foi criado para o desafio de admissão da Lacuna Software e deve permanecer privado, salvo orientação em contrário.
