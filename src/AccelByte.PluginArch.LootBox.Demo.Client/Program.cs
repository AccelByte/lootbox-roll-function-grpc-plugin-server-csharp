// Copyright (c) 2023 AccelByte Inc. All Rights Reserved.
// This is licensed software from AccelByte Inc, for limitations
// and restrictions contact your company contract manager.

using System;
using System.Collections.Generic;

using CommandLine;

using AccelByte.Sdk.Core.Util;
using AccelByte.PluginArch.LootBox.Demo.Client.Model;
using AccelByte.Sdk.Api.Platform.Operation;

namespace AccelByte.PluginArch.LootBox.Demo.Client
{
    internal class Program
    {
        static int Main(string[] args)
        {
            int exitCode = 0;
            Parser.Default.ParseArguments<ApplicationConfig>(args)
                .WithParsed((config) =>
                {
                    config.FinalizeConfigurations();
                    PlatformWrapper wrapper = new PlatformWrapper(config);

                    Console.WriteLine($"\tBaseUrl: {config.BaseUrl}");
                    Console.WriteLine($"\tClientId: {config.ClientId}");                    
                    Console.WriteLine($"\tUsername: {config.Username}");
                    Console.WriteLine($"\tStore Category: {config.CategoryPath}");
                    if (config.GrpcServerUrl != "")
                        Console.WriteLine($"\tGrpc Target: {config.GrpcServerUrl}");
                    else if (config.ExtendAppName != "")
                        Console.WriteLine($"\tExtend App: {config.ExtendAppName}");
                    else
                    {
                        Console.WriteLine($"\tNO GRPC TARGET SERVER");                        
                        exitCode = 2;
                        return;
                    }   

                    try
                    {
                        Console.Write("Logging in to AccelByte... ");
                        var userInfo = wrapper.Login();
                        Console.WriteLine("[OK]");
                        Console.WriteLine($"User: {userInfo.UserName}");

                        Console.Write("Configuring custom configuration... ");
                        wrapper.ConfigureGrpcTargetUrl();
                        Console.WriteLine("[OK]");
                        try
                        {
                            Console.Write("Creating draft store... ");
                            wrapper.CreateStore();
                            Console.WriteLine("[OK]");

                            Console.Write("Create store category... ");
                            wrapper.CreateCategory(config.CategoryPath);
                            Console.WriteLine("[OK]");

                            Console.Write("Creating lootbox item(s)... ");
                            List<SimpleLootboxItem> sItems = wrapper.CreateLootboxItems(1, 5, config.CategoryPath);
                            Console.WriteLine("[OK]");
                            sItems[0].WriteToConsole();

                            Console.Write("Publishing store changes... ");
                            wrapper.PublishStoreChange();
                            Console.WriteLine("[OK]");

                            try
                            {
                                Console.Write("Granting item entitlement to user... ");
                                string entitlementId = wrapper.GrantEntitlement(userInfo.UserId!, sItems[0].Id, 1);
                                Console.WriteLine("[OK]");

                                Console.Write("Consuming entitlement... ");
                                SimpleLootboxItem lbItemResult = wrapper.ConsumeItemEntitlement(userInfo.UserId!, entitlementId, 1);
                                Console.WriteLine("[OK]");

                                lbItemResult.WriteToConsole();
                            }
                            catch (Exception x)
                            {
                                Console.WriteLine($"Exception: {x.Message}");
                                exitCode = 1;
                            }
                            finally
                            {
                                Console.Write("Removing lootbox item(s)... ");
                                wrapper.DeleteLootboxItems(sItems);
                                Console.WriteLine("[OK]");
                            }
                        }
                        catch (Exception x)
                        {
                            Console.WriteLine($"Exception: {x.Message}");
                            exitCode = 1;
                        }
                        finally
                        {
                            Console.Write("Deleting custom configuration... ");
                            wrapper.DeleteGrpcTargetUrl();
                            Console.WriteLine("[OK]");

                            Console.Write("Deleting store... ");
                            wrapper.DeleteStore();
                            Console.WriteLine("[OK]");
                        }
                    }
                    catch (Exception x)
                    {
                        Console.WriteLine($"Exception: {x.Message}");
                        exitCode = 1;
                    }
                    finally
                    {
                        wrapper.Logout();
                    }
                })
                .WithNotParsed((errors) =>
                {
                    Console.WriteLine("Invalid argument(s)");
                    foreach (var error in errors)
                        Console.WriteLine($"\t{error}");
                    exitCode = 2;
                });

            return exitCode;
        }
    }
}