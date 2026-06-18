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

