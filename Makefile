# Default target
.DEFAULT_GOAL := help

VERSION ?= dev

# Variables
RESET := \033[0m
BOLD := \033[1m
BLUE := \033[34m

## Display this help message
help:
	@echo "${BOLD}Available targets:${RESET}"
	@awk '/^##/ {help = substr($$0, 4); getline; if ($$1 != "") printf "  ${BLUE}%-15s${RESET} %s\n", $$1, help}' $(MAKEFILE_LIST)

## Build the project
build:
	@echo "${BLUE}Restoring dependencies...${RESET}"
	@dotnet restore /p:EnableWindowsTargeting=true "PS5 NOR Modifier"
	@echo "\n${BLUE}Building...${RESET}"
	@dotnet build "PS5 NOR Modifier"  /p:EnableWindowsTargeting=true --configuration Release --no-restore
	@dotnet publish /p:EnableWindowsTargeting=true -c Release -r win-x86 --self-contained "PS5 NOR Modifier"
	@echo "\n${BLUE}Preparing release...${RESET}"
	@zip -q -r PS5NorModifier-${VERSION}.zip "PS5 NOR Modifier/bin/Release/net6.0-windows/win-x86/"
	@echo "Application archive created: ./PS5NorModifier-${VERSION}.zip"
