// Copyright (c) 2023 AccelByte Inc. All Rights Reserved.
// This is licensed software from AccelByte Inc, for limitations
// and restrictions contact your company contract manager.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;

using Grpc.Core;
using AccelByte.Platform.Entitlement.Lootbox.V1;

namespace AccelByte.PluginArch.LootBox.Demo.Server.Services
{
    public class LootboxFunctionService : AccelByte.Platform.Entitlement.Lootbox.V1.LootBox.LootBoxBase
    {
        private readonly ILogger<LootboxFunctionService> _Logger;

        public LootboxFunctionService(ILogger<LootboxFunctionService> logger)
        {
            _Logger = logger;
        }

        public override Task<RollLootBoxRewardsResponse> RollLootBoxRewards(RollLootBoxRewardsRequest request, ServerCallContext context)
        {
            _Logger.LogInformation("Received RollLootBoxRewards request.");

            var rewards = request.ItemInfo.LootBoxRewards;
            _Logger.LogInformation($"Item: {request.ItemInfo.ItemId}");

            int rewardWeightSum = 0;
            foreach (var reward in rewards)
                rewardWeightSum += reward.Weight;

            Random rand = new Random();

            List<RewardObject> result = new List<RewardObject>();
            for (int i = 0; i < request.Quantity; i++)
            {
                int selectedIdx = 0;
                for (double r = rand.NextDouble() * rewardWeightSum; selectedIdx < rewards.Count - 1; selectedIdx++)
                {
                    r -= rewards[selectedIdx].Weight;
                    if (r <= 0.0)
                        break;
                }

                var selectedReward = rewards[selectedIdx];
                int itemCount = selectedReward.Items.Count;

                int selectedItemIdx = (int)Math.Round(rand.NextDouble() * (double)(itemCount - 1));
                BoxItemObject selectedItem = selectedReward.Items[selectedItemIdx];

                var rewardObject = new RewardObject()
                {
                    ItemId = selectedItem.ItemId,
                    ItemSku = selectedItem.ItemSku,
                    Count = selectedItem.Count
                };
                _Logger.LogInformation($"Reward: {rewardObject.ItemId}");

                result.Add(rewardObject);
            }

            RollLootBoxRewardsResponse response = new RollLootBoxRewardsResponse();
            response.Rewards.AddRange(result);

            return Task.FromResult(response);
        }
    }
}
