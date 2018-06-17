PROJECT_NAME := DotN64

COMPILER := msbuild
COMPILER_CONFIGURATION := Release

OUTPUT_DIRECTORY := bin/$(COMPILER_CONFIGURATION)
OUTPUT_BINARY := DotN64.Desktop.exe

BRANCH_NAME := $(shell git rev-parse --abbrev-ref HEAD)
VERSION = $(shell $(OUTPUT_DIRECTORY)/$(OUTPUT_BINARY) --help | head -1 | awk '{print $$2}' | cut -c 2-)

DEPLOY_SERVER := 192.168.1.51
DEPLOY_PATH := /var/www/nabile.duckdns.org/$(PROJECT_NAME)/Download

all:
	cd .. && nuget restore
	$(COMPILER) .. /p:Configuration=$(COMPILER_CONFIGURATION)
	chmod +x $(OUTPUT_DIRECTORY)/$(OUTPUT_BINARY)

deploy: all
	ssh $(DEPLOY_SERVER) 'mkdir -p $(DEPLOY_PATH)/$(BRANCH_NAME)'
	ssh $(DEPLOY_SERVER) 'cd $(DEPLOY_PATH) && ls -d */ | cut -f 1 -d "/" > streams'
	cd $(OUTPUT_DIRECTORY) && zip -r - . | ssh $(DEPLOY_SERVER) 'cat > $(DEPLOY_PATH)/$(BRANCH_NAME)/$(PROJECT_NAME).zip'
	echo $(VERSION) | ssh $(DEPLOY_SERVER) 'cat > $(DEPLOY_PATH)/$(BRANCH_NAME)/version'

.PHONY: all deploy
