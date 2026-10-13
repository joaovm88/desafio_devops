# Documentação Técnica: Desafio DevOps

## 1. Título e integrantes
**EcoCity Analytics – API de Governança ESG** (C# .NET 8 + MongoDB) — João Victor Moreira

## 2. Descrição do pipeline
Ferramenta: GitHub Actions. Etapas: Build → Testes (quality gate) → Imagem Docker (GHCR) → Deploy Staging (smoke test) → Deploy Produção (após staging e aprovação do environment). Detalhes no README.

## 3. Docker
Arquitetura: API .NET + MongoDB em rede interna, volume para dados. Imagem multi-stage Alpine, usuário não-root. Comandos: `docker compose build`, `docker compose up -d`, `docker compose logs api`.

## 4. Evidências do pipeline
[Print: build] [Print: testes] [Print: deploy] — salvar em `docs/prints/`.

## 5. Evidências dos ambientes
[Print: staging /swagger porta 8081] [Print: produção porta 8082].

## 6. Desafios e soluções
- **Ordem de inicialização Mongo × API**: `healthcheck` + `depends_on: condition: service_healthy`.
- **Credenciais no pipeline**: `.env.example` documenta variáveis; segredos reais ficam em GitHub Environments secrets.
- **Testes sem banco**: repositório abstraído por interface, com implementação em memória nos testes.

## 7. Checklist
Ver README.
