# Copyright (c) 2022-2023 AccelByte Inc. All Rights Reserved.
# This is licensed software from AccelByte Inc, for limitations
# and restrictions contact your company contract manager.

SHELL := /bin/bash

BUILDER := grpc-plugin-server-builder
IMAGE_NAME := $(shell basename "$$(pwd)")-app
DOTNETVER := 6.0.302
APP_PATH := AccelByte.PluginArch.LootBox.Demo.Server

.PHONY: build image imagex test

build:
	docker run --rm -u $$(id -u):$$(id -g) \
		-v $$(pwd):/data/ \
		-e HOME="/data/.testrun" -e DOTNET_CLI_HOME="/data/.testrun" \
		mcr.microsoft.com/dotnet/sdk:$(DOTNETVER) \
		sh -c "mkdir -p /data/.testrun && cp -r /data/src /data/.testrun/src && cd /data/.testrun/src && dotnet build && mkdir -p /data/.output && cp -r /data/.testrun/src/$(APP_PATH)/bin/* /data/.output/ && rm -rf /data/.testrun"

image:
	docker build -t ${IMAGE_NAME} .

imagex:
	docker buildx inspect ${IMAGE_NAME}-builder \
			|| docker buildx create --name ${IMAGE_NAME}-builder --use 
	docker buildx build -t ${IMAGE_NAME} --platform linux/arm64,linux/amd64 .
	docker buildx build -t ${IMAGE_NAME} --load .
	#docker buildx rm ${IMAGE_NAME}-builder

imagex_push:
	@test -n "$(IMAGE_TAG)" || (echo "IMAGE_TAG is not set (e.g. 'v0.1.0', 'latest')"; exit 1)
	@test -n "$(REPO_URL)" || (echo "REPO_URL is not set"; exit 1)
	docker buildx inspect $(BUILDER) || docker buildx create --name $(BUILDER) --use
	docker buildx build -t ${REPO_URL}:${IMAGE_TAG} --platform linux/arm64,linux/amd64 --push .
	docker buildx rm --keep-state $(BUILDER)

test:
	docker run --rm -u $$(id -u):$$(id -g) \
		-v $$(pwd):/data/ \
		-e HOME="/data/.testrun" -e DOTNET_CLI_HOME="/data/.testrun" \
		mcr.microsoft.com/dotnet/sdk:$(DOTNETVER) \
		sh -c "mkdir -p /data/.testrun && cp -r /data/src /data/.testrun/src && cd /data/.testrun/src && dotnet test && rm -rf /data/.testrun"

test_functional_local_hosted:
	@test -n "$(ENV_PATH)" || (echo "ENV_PATH is not set"; exit 1)
	docker build --tag lootbox-test-functional -f test/functional/Dockerfile test/functional && \
	docker run --rm -t \
		--env-file $(ENV_PATH) \
		-e DOTNET_CLI_HOME="/data" \
		-e XDG_DATA_HOME="/data" \
		-u $$(id -u):$$(id -g) \
		-v $$(pwd):/data \
		-w /data lootbox-test-functional bash ./test/functional/test-local-hosted.sh

test_functional_accelbyte_hosted:
	@test -n "$(ENV_PATH)" || (echo "ENV_PATH is not set"; exit 1)
ifeq ($(shell uname), Linux)
	$(eval DARGS := -u $$(shell id -u):$$(shell id -g) --group-add $$(shell getent group docker | cut -d ':' -f 3))
endif
	docker build --tag lootbox-test-functional -f test/functional/Dockerfile test/functional && \
	docker run --rm -t \
		--env-file $(ENV_PATH) \
		-e DOTNET_CLI_HOME="/data" \
		-e XDG_DATA_HOME="/data" \
		-e DOCKER_CONFIG=/tmp/.docker \
		$(DARGS) \
		-v /var/run/docker.sock:/var/run/docker.sock \
		-v $$(pwd):/data \
		-w /data lootbox-test-functional bash ./test/functional/test-accelbyte-hosted.sh

ngrok:
	@test -n "$(NGROK_AUTHTOKEN)" || (echo "NGROK_AUTHTOKEN is not set" ; exit 1)
	docker run --rm -it --net=host -e NGROK_AUTHTOKEN=$(NGROK_AUTHTOKEN) ngrok/ngrok:3-alpine \
			tcp 6565	# gRPC server port
