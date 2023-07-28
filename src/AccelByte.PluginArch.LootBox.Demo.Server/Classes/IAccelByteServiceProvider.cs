// Copyright (c) 2022-2023 AccelByte Inc. All Rights Reserved.
// This is licensed software from AccelByte Inc, for limitations
// and restrictions contact your company contract manager.

using System;
using System.IdentityModel.Tokens.Jwt;

using AccelByte.Sdk.Core;

namespace AccelByte.PluginArch.LootBox.Demo.Server
{
    public interface IAccelByteServiceProvider
    {
        AccelByteSDK Sdk { get; }

        AppSettingConfigRepository Config { get; }        
    }
}
