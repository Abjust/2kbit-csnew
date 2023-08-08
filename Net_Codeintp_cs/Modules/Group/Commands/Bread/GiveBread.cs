// 2kbit C# Edition: New，2kbit的 C# 分支版本的优化方案

// Copyright(C) 2023 Abjust 版权所有。

// 本程序是自由软件：你可以根据自由软件基金会发布的GNU Affero通用公共许可证的条款，即许可证的第3版或（您选择的）任何后来的版本重新发布它和/或修改它。。

// 本程序的发布是希望它能起到作用。但没有任何保证；甚至没有隐含的保证。本程序的分发是希望它是有用的，但没有任何保证，甚至没有隐含的适销对路或适合某一特定目的的保证。 参见 GNU Affero通用公共许可证了解更多细节。

// 您应该已经收到了一份GNU Affero通用公共许可证的副本。 如果没有，请参见<https://www.gnu.org/licenses/>。

/**
* 2kbit C# Edition: New
* 面包厂模块：给面包
**/

using Mirai.Net.Data.Messages;
using Mirai.Net.Data.Messages.Receivers;
using Mirai.Net.Modules;
using Mirai.Net.Utils.Scaffolds;
using Net_Codeintp_cs.Modules.Utils;
using Newtonsoft.Json.Linq;

namespace Net_Codeintp_cs.Modules.Group.Commands.Bread
{
    internal class GiveBread : IModule
    {
        public bool? IsEnable { get; set; }

        public async void Execute(MessageReceiverBase @base)
        {
            GroupMessageReceiver receiver = @base.Concretize<GroupMessageReceiver>();
            string[] s = receiver.MessageChain.GetPlainMessage().Split(" ");
            if (s[0] == "!givebread")
            {
                switch (s.Length)
                {
                    case 2:
                        if (Json.FileExists("breadfactory") && Json.FileExists("materials") && Json.ObjectExistsInArray("breadfactory", "groups", "groupid", receiver.GroupId))
                        {
                            JObject obj = Json.ReadFile("breadfactory");
                            JObject item = (JObject)obj["groups"]!.Where(x => x.SelectToken("groupid")!.Value<string>()! == receiver.GroupId).FirstOrDefault()!;
                            if ((string)item["factory_mode"]! == "normal")
                            {
                                if (int.TryParse(s[1], out int number) && number >= 1)
                                {
                                    if ((int)item["breads"]! + number <= (int)(64 * Math.Pow(4, (int)item["factory_level"]! - 1) * Math.Pow(2, (int)item["storage_level"]!)))
                                    {
                                        Json.ModifyObjectFromArray("breadfactory", "groups", "groupid", receiver.GroupId, "breads", (int)item["breads"]! + number);
                                        Logger.Info($"有面包厂接收了 {number} 块面包，现在该分厂仓库有 {(int)item["breads"]! + number} 块面包！\n分厂：{receiver.GroupName} ({receiver.GroupId})");
                                        try
                                        {
                                            MessageChain messagechain = new MessageChainBuilder()
                                                .At(receiver.Sender.Id)
                                                .Plain($" 现在库存有 {(int)item["breads"]! + number} 块面包辣！")
                                                .Build();
                                            await receiver.SendMessageAsync(messagechain);
                                            break;
                                        }
                                        catch (Exception e)
                                        {
                                            Logger.Error("群消息发送失败！");
                                            Logger.Debug($"错误信息：\n{e.Message}");
                                        }
                                    }
                                    else
                                    {
                                        Logger.Warning($"有面包厂未能接收 {number} 块面包，因为该分厂仓库已满！\n分厂：{receiver.GroupName} ({receiver.GroupId})");
                                        try
                                        {
                                            await receiver.SendMessageAsync("抱歉，库存已经满了。。。");
                                            break;
                                        }
                                        catch (Exception e)
                                        {
                                            Logger.Error("群消息发送失败！");
                                            Logger.Debug($"错误信息：\n{e.Message}");
                                        }
                                    }
                                }
                                else
                                {
                                    Logger.Warning($"有面包未能送入该分厂，因为提供的参数有误！\n分厂：{receiver.GroupName} ({receiver.GroupId})");
                                    try
                                    {
                                        await receiver.SendMessageAsync("请输入正确的数字！");
                                    }
                                    catch (Exception e)
                                    {
                                        Logger.Error("群消息发送失败！");
                                        Logger.Debug($"错误信息：\n{e.Message}");
                                    }
                                }
                            }
                            else
                            {
                                Logger.Warning($"有面包未能送入该分厂，因为生产模式不允许！\n分厂：{receiver.GroupName} ({receiver.GroupId})");
                                try
                                {
                                    await receiver.SendMessageAsync("请将生产模式设置成“单一化供应”，否则你不能给面包厂面包！");
                                }
                                catch (Exception e)
                                {
                                    Logger.Error("群消息发送失败！");
                                    Logger.Debug($"错误信息：\n{e.Message}");
                                }
                            }
                        }
                        else
                        {
                            Logger.Warning($"有面包未能送入该地区，因为该地区尚未兴建面包厂！\n地区：{receiver.GroupName} ({receiver.GroupId})");
                            try
                            {
                                await receiver.SendMessageAsync("本群还没有面包厂！");
                            }
                            catch (Exception e)
                            {
                                Logger.Error("群消息发送失败！");
                                Logger.Debug($"错误信息：\n{e.Message}");
                            }
                        }
                        break;
                    case 1:
                        if (Json.FileExists("breadfactory") && Json.FileExists("materials") && Json.ObjectExistsInArray("breadfactory", "groups", "groupid", receiver.GroupId))
                        {
                            JObject obj = Json.ReadFile("breadfactory");
                            JObject item = (JObject)obj["groups"]!.Where(x => x.SelectToken("groupid")!.Value<string>()! == receiver.GroupId).FirstOrDefault()!;
                            if ((string)item["factory_mode"]! == "normal")
                            {
                                if ((int)item["breads"]! + 1 <= (int)(64 * Math.Pow(4, (int)item["factory_level"]! - 1) * Math.Pow(2, (int)item["storage_level"]!)))
                                {
                                    Json.ModifyObjectFromArray("breadfactory", "groups", "groupid", receiver.GroupId, "breads", (int)item["breads"]! + 1);
                                    Logger.Info($"有面包厂接收了 1 块面包，现在该分厂仓库有 {(int)item["breads"]! + 1} 块面包！\n分厂：{receiver.GroupName} ({receiver.GroupId})");
                                    try
                                    {
                                        MessageChain messagechain = new MessageChainBuilder()
                                        .At(receiver.Sender.Id)
                                            .Plain($" 现在库存有 {(int)item["breads"]! + 1} 块面包辣！")
                                            .Build();
                                        await receiver.SendMessageAsync(messagechain);
                                        break;
                                    }
                                    catch (Exception e)
                                    {
                                        Logger.Error("群消息发送失败！");
                                        Logger.Debug($"错误信息：\n{e.Message}");
                                    }
                                }
                                else
                                {
                                    Logger.Warning($"有面包厂未能接收 1 块面包，因为该分厂仓库已满！\n分厂：{receiver.GroupName} ({receiver.GroupId})");
                                    try
                                    {
                                        await receiver.SendMessageAsync("抱歉，库存已经满了。。。");
                                        break;
                                    }
                                    catch (Exception e)
                                    {
                                        Logger.Error("群消息发送失败！");
                                        Logger.Debug($"错误信息：\n{e.Message}");
                                    }
                                }
                            }
                            else
                            {
                                Logger.Warning($"有面包未能送入该分厂，因为生产模式不允许！\n分厂：{receiver.GroupName} ({receiver.GroupId})");
                                try
                                {
                                    await receiver.SendMessageAsync("请将生产模式设置成“单一化供应”，否则你不能给面包厂面包！");
                                }
                                catch (Exception e)
                                {
                                    Logger.Error("群消息发送失败！");
                                    Logger.Debug($"错误信息：\n{e.Message}");
                                }
                            }
                        }
                        else
                        {
                            Logger.Warning($"有面包未能送入该地区，因为该地区尚未兴建面包厂！\n地区：{receiver.GroupName} ({receiver.GroupId})");
                            try
                            {
                                await receiver.SendMessageAsync("本群还没有面包厂！");
                            }
                            catch (Exception e)
                            {
                                Logger.Error("群消息发送失败！");
                                Logger.Debug($"错误信息：\n{e.Message}");
                            }
                        }
                        break;
                    default:
                        Logger.Warning($"有进货订单未被回应，因为提供的参数有误！\n地区：{receiver.GroupName} ({receiver.GroupId})");
                        try
                        {
                            await receiver.SendMessageAsync("参数错误");
                        }
                        catch (Exception e)
                        {
                            Logger.Error("群消息发送失败！");
                            Logger.Debug($"错误信息：\n{e.Message}");
                        }
                        break;
                }
            }
        }
    }
}