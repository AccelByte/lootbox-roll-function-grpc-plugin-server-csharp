# Copyright (c) 2023 AccelByte Inc. All Rights Reserved.
# This is licensed software from AccelByte Inc, for limitations
# and restrictions contact your company contract manager.

SHELL := /bin/bash
DOTNETVER := 6.0.302

run:
	@test -n "$(ENV_FILE_PATH)" || (echo "ENV_FILE_PATH is not set" ; exit 1)
	docker run -t --rm \
		-u $$(id -u):$$(id -g) \
		-v $$(pwd):/data/ \
		-w /data \
		-e HOME="/data" -e DOTNET_CLI_HOME="/data" \
		--env-file $(ENV_FILE_PATH) \
		mcr.microsoft.com/dotnet/sdk:$(DOTNETVER) \
		dotnet run