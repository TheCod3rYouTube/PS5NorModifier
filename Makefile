# Default target
.DEFAULT_GOAL := help

VERSION ?= dev
UART_PROJ :=  "UART-CL By TheCod3r/UART-CL By TheCod3r/UART-CL By TheCod3r.csproj"
PS5_NOR_PROJ := "PS5 NOR Modifier"

# Variables
RESET := \033[0m
BOLD := \033[1m
BLUE := \033[34m
GREEN := \033[32m
YELLOW := \033[33m

## Display this help message
help:
	@echo "${BOLD}Available targets:${RESET}"
	@awk '/^##/ {help = substr($$0, 4); getline; if ($$1 != "") printf "  ${BLUE}%-30s${RESET} %s\n", $$1, help}' $(MAKEFILE_LIST)

## Restore dependencies for a project
restore:
	@echo "${BLUE}Restoring dependencies for ${PROJ} (${ARCH})...${RESET}"
	@dotnet restore "UART-CL By TheCod3r/UART-CL By TheCod3r/UART-CL By TheCod3r.csproj" -r ${ARCH}
	
## Build for all platforms
build: build-windows-ui build-windows-x86 build-macos-x64 build-macos-arm64 build-linux-x64

uart-cl:
	@echo "${BLUE}Building UART-CL for ${ARCH}...${RESET}"
	@dotnet build ${UART_PROJ} --no-restore --configuration Release 
	@mkdir -p "./build/${ARCH}"
	@dotnet publish ${UART_PROJ} --no-restore --configuration Release --self-contained true --runtime ${ARCH} -p:PublishSingleFile=true -o "./build/${ARCH}"
	@echo "${GREEN}Creating UART-CL archive for ${ARCH} v${VERSION}...${RESET}"
	@zip -q -r ./build/UART-CL-${ARCH}-${VERSION}.zip "./build/${ARCH}"
	@echo "UART-CL archive created: ./build/UART-CL-${ARCH}-${VERSION}.zip"

## Build PS5 NOR modifier for Windows
build-windows-ui:
	@make restore PROJ=${UART_PROJ} ARCH=win-x86
	@echo "${BLUE}Building PS5 NOR Modifier for Windows...${RESET}"
	@dotnet publish ${PS5_NOR_PROJ} \
		--configuration Release \
		--runtime win-x86 \
		--self-contained true \
		/p:EnableWindowsTargeting=true \
		/p:PublishSingleFile=true \
		/p:DebugType=None \
		/p:DebugSymbols=false \
		-o "./build/ps5normodifier"
	@echo "${GREEN}Creating PS5 NOR Modifier archive v${VERSION}...${RESET}"
	@zip -q -r PS5NorModifier-${VERSION}.zip "./build/ps5normodifier"
	@mv PS5NorModifier-${VERSION}.zip ./build/
	@echo "PS5 NOR Modifier archive created: ./build/PS5NorModifier-${VERSION}.zip"

## Build UART-CL for Windows
build-windows-x86:
	@make restore PROJ=${UART_PROJ} ARCH=win-x86
	@make uart-cl PROJ=${UART_PROJ} ARCH=win-x86

## Build UART-CL for macOS
build-macos-x64:
	@make restore PROJ=${UART_ROOT} ARCH=osx-x64
	@make uart-cl PROJ=${UART_ROOT} ARCH=osx-x64

## Build UART-CL for Linux
build-macos-arm64:
	@make restore PROJ=${UART_ROOT} ARCH=osx-arm64
	@make uart-cl PROJ=${UART_ROOT} ARCH=osx-arm64

## Build UART-CL for Linux
build-linux-x64:
	@make restore PROJ=${UART_ROOT} ARCH=linux-x64
	@make uart-cl PROJ=${UART_ROOT} ARCH=linux-x64

## Clean build artifacts
clean:
	@echo "${YELLOW}Cleaning build artifacts...${RESET}"
	@rm -rf ./build
	@echo "Cleaned build artifacts"
