#!/usr/bin/env bash
set -euo pipefail

echo "Installing .NET 10 SDK..."

curl -fsSL https://dot.net/v1/dotnet-install.sh -o /tmp/dotnet-install.sh

bash /tmp/dotnet-install.sh \
  --channel 10.0 \
  --install-dir "$HOME/.dotnet"

export DOTNET_ROOT="$HOME/.dotnet"
export PATH="$DOTNET_ROOT:$PATH"

echo "Checking .NET SDK..."
dotnet --version

echo "Publishing Blazor application..."
dotnet publish PPE.LandingPage.csproj \
  -c Release \
  -o dist

echo "Build completed!"
