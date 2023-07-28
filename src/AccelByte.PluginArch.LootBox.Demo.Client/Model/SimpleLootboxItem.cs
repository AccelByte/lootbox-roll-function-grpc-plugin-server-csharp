// Copyright (c) 2023 AccelByte Inc. All Rights Reserved.
// This is licensed software from AccelByte Inc, for limitations
// and restrictions contact your company contract manager.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccelByte.PluginArch.LootBox.Demo.Client.Model
{
    public class SimpleLootboxItem : SimpleItemInfo
    {
        public string Diff { get; set; } = String.Empty;

        public List<SimpleItemInfo> RewardItems { get; set; } = new();

        public void WriteToConsole()
        {
            Console.WriteLine($"Lootbox Item Id: {Id}");
            Console.WriteLine("Reward Items: ");
            foreach (var item in RewardItems)
                Console.WriteLine($"\t{item.Id} : {item.Sku} : {item.Title}");
        }
    }
}
