#!/usr/bin/env bash
# Uso: scripts/deploy.sh <staging|production> <imagem>
# Sobe a stack (API + MongoDB) isolada por ambiente e executa smoke test.
# Segredos vêm de variáveis de ambiente (GitHub Environments secrets), nunca do repositório.
set -euo pipefail

ENVIRONMENT="${1:?ambiente (staging|production)}"
export IMAGE="${2:?imagem}"
: "${MONGO_PASSWORD:?defina MONGO_PASSWORD}"

case "$ENVIRONMENT" in
  staging)    export API_PORT=8081 ASPNETCORE_ENVIRONMENT=Staging ;;
  production) export API_PORT=8082 ASPNETCORE_ENVIRONMENT=Production ;;
  *) echo "Ambiente inválido: $ENVIRONMENT" >&2; exit 1 ;;
esac
export MONGO_USER="${MONGO_USER:-ecocity}" MONGO_DB="ecocity_${ENVIRONMENT}"

PROJECT="ecocity-${ENVIRONMENT}"
docker compose -p "$PROJECT" up -d --no-build --wait --wait-timeout 120

echo "Smoke test em http://localhost:${API_PORT}"
for i in $(seq 1 15); do
  if curl -fs "http://localhost:${API_PORT}/health"; then echo; break; fi
  [ "$i" -eq 15 ] && { docker compose -p "$PROJECT" logs; exit 1; }
  sleep 2
done
curl -fs "http://localhost:${API_PORT}/" && echo
