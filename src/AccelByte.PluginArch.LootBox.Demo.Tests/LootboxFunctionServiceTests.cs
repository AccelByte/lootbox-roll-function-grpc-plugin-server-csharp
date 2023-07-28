// Copyright (c) 2023 AccelByte Inc. All Rights Reserved.
// This is licensed software from AccelByte Inc, for limitations
// and restrictions contact your company contract manager.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

using NUnit.Framework;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

using AccelByte.Platform.Entitlement.Lootbox.V1;
using AccelByte.PluginArch.LootBox.Demo.Server.Services;

namespace AccelByte.PluginArch.LootBox.Demo.Tests
{
    [TestFixture]
    public class LootboxFunctionServiceTests
    {
        private ILogger<LootboxFunctionService> _ServiceLogger;

        public LootboxFunctionServiceTests()
        {
            ILoggerFactory loggerFactory = new NullLoggerFactory();
            _ServiceLogger = loggerFactory.CreateLogger<LootboxFunctionService>();
        }

        [Test]
        public async Task GetRewardsTest()
        {

            var lootBoxService = new LootboxFunctionService(_ServiceLogger);

            RollLootBoxRewardsRequest request = new RollLootBoxRewardsRequest();
            request.UserId = "b52a2364226d436285c1b8786bc9cbd1";
            request.Namespace = "accelbyte";
            request.Quantity = 10;
            request.ItemInfo = new LootBoxItemInfo()
            {
                ItemId = "8a0b8bda28c845f6938cc57540af452e",
                ItemSku = "SKU3170",
                RewardCount = 2
            };

            var lootBoxReward = new LootBoxItemInfo.Types.LootBoxRewardObject()
            {
                Name = "Foods",
                Type = "REWARD",
                Weight = 10,
                Odds = 0
            };
            lootBoxReward.Items.Add(new BoxItemObject()
            {
                ItemId = "8b6016d243264c0f90031600313b8a37",
                ItemSku = "SKU4650",
                Count = 5
            });

            request.ItemInfo.LootBoxRewards.Add(lootBoxReward);

            var response = await lootBoxService.RollLootBoxRewards(request, new UnitTestCallContext());
            Assert.IsNotNull(response);

            Assert.Greater(response.Rewards.Count, 0);
            Assert.AreEqual("8b6016d243264c0f90031600313b8a37", response.Rewards[0].ItemId);
        }
    }
}