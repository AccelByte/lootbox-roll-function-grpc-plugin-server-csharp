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
    public class SimpleSectionInfo
    {
        public string Id { get; set; } = String.Empty;

        public List<SimpleItemInfo> Items { get; set; } = new();

        public void WriteToConsole()
        {
            Console.WriteLine($"Section Id: {Id}");
            foreach (var item in Items)
                Console.WriteLine($"\t{item.Id} : {item.Sku} : {item.Title}");
        }
    }
}
