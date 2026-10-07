#!/usr/bin/env bash
# Gate de calidad: compila y corre las pruebas. Si algo falla, sale con 2
# para que Claude Code bloquee el cierre y le devuelva el error a Claude.
set -uo pipefail

input=$(cat)

# Si Claude ya está continuando por este mismo hook, no volver a bloquear (evita bucles).
if [ "$(printf '%s' "$input" | jq -r '.stop_hook_active // false')" = "true" ]; then
  exit 0
fi

cd "${CLAUDE_PROJECT_DIR:-$(dirname "$0")/../..}" || exit 0
DOTNET="${DOTNET:-$HOME/.dotnet/dotnet}"

if ! out=$("$DOTNET" build app-curso-claude.slnx --nologo -v q 2>&1); then
  { echo "Gate: dotnet build falló."; echo "$out" | tail -n 30; } >&2
  exit 2
fi

if ! out=$("$DOTNET" test app-curso-claude.slnx --no-build --nologo -v q 2>&1); then
  { echo "Gate: dotnet test falló."; echo "$out" | tail -n 30; } >&2
  exit 2
fi

exit 0
