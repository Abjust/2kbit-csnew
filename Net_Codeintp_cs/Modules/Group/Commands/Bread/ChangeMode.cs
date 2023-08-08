// 2kbit C# Edition: New，2kbit的 C# 分支版本的优化方案

// Copyright(C) 2023 Abjust 版权所有。

// 本程序是自由软件：你可以根据自由软件基金会发布的GNU Affero通用公共许可证的条款，即许可证的第3版或（您选择的）任何后来的版本重新发布它和/或修改它。。

// 本程序的发布是希望它能起到作用。但没有任何保证；甚至没有隐含的保证。本程序的分发是希望它是有用的，但没有任何保证，甚至没有隐含的适销对路或适合某一特定目的的保证。 参见 GNU Affero通用公共许可证了解更多细节。

// 您应该已经收到了一份GNU Affero通用公共许可证的副本。 如果没有，请参见<https://www.gnu.org/licenses/>。

/**
* 2kbit C# Edition: New
* 面包厂模块：切换生产模式
**/

using Mirai.Net.Data.Messages;
using Mirai.Net.Data.Messages.Receivers;
using Mirai.Net.Modules;
using Mirai.Net.Utils.Scaffolds;
using Net_Codeintp_cs.Modules.Utils;
using Newtonsoft.Json.Linq;

namespace Net_Codeintp_cs.Modules.Group.Commands.Bread
{
    internal class ChangeMode : IModule
    {
        public bool? IsEnable { get; set; }

        public async void Execute(MessageReceiverBase @base)
        {
            GroupMessageReceiver receiver = @base.Concretize<GroupMessageReceiver>();
            string[] s = receiver.MessageChain.GetPlainMessage().Split(" ");
            if (s[0] == "!change_mode")
            {
                if (Json.FileExists("breadfactory") && Json.FileExists("materials") && Json.ObjectExistsInArray("breadfactory", "groups", "groupid", receiver.GroupId))
                {
                    JObject obj = Json.ReadFile("breadfactory");
                    JObject item = (JObject)obj["groups"]!.Where(x => x.SelectToken("groupid")!.Value<string>()! == receiver.GroupId).FirstOrDefault()!;
                    if (((string)item["factory_mode"]!).Contains("infinite"))
                    {
                        switch (s[1])
                        {
                            case "normal":
                                Json.ModifyObjectFromArray("breadfactory", "groups", "groupid", receiver.GroupId, "factory_mode", typeof(string), "normal");
                                Logger.Info($"有面包厂已将生产模式切换为：“单一化供应”！\n分厂：{receiver.GroupName} ({receiver.GroupId})");
                                try
                                {
                                    await receiver.SendMessageAsync("已将本群生产模式修改为：单一化供应");
                                }
                                catch (Exception e)
                                {
                                    Logger.Error("群消息发送失败！");
                                    Logger.Debug($"错误信息：\n{e.Message}");
                                }
                                break;
                            default:
                                Logger.Warning($"有分厂未能修改生产模式，因为当前生产模式是“无限供应”或其变体！\n分厂：{receiver.GroupName} ({receiver.GroupId})");
                                try
                                {
                                    await receiver.SendMessageAsync("由于当前面包厂生产模式是“无限供应”或其变体，因此仅允许切换到“单一化供应”！");
                                }
                                catch (Exception e)
                                {
                                    Logger.Error("群消息发送失败！");
                                    Logger.Debug($"错误信息：\n{e.Message}");
                                }
                                break;
                        }
                    }
                    else
                    {
                        if ((int)item["breads"]! == 0 || ((string)item["factory_mode"]!).Contains("infinite"))
                        {
                            switch (s[1])
                            {
                                case "infinite_diverse":
                                    Json.ModifyObjectFromArray("breadfactory", "groups", "groupid", receiver.GroupId, "factory_mode", typeof(string), "infinite_diverse");
                                    Logger.Info($"有面包厂已将生产模式切换为：“多样化无限供应”！\n分厂：{receiver.GroupName} ({receiver.GroupId})");
                                    try
                                    {
                                        await receiver.SendMessageAsync("已将本群生产模式修改为：多样化无限供应");
                                    }
                                    catch (Exception e)
                                    {
                                        Logger.Error("群消息发送失败！");
                                        Logger.Debug($"错误信息：\n{e.Message}");
                                    }
                                    break;
                                case "infinite":
                                    Json.ModifyObjectFromArray("breadfactory", "groups", "groupid", receiver.GroupId, "factory_mode", typeof(string), "infinite");
                                    Logger.Info($"有面包厂已将生产模式切换为：“无限供应”！\n分厂：{receiver.GroupName} ({receiver.GroupId})");
                                    try
                                    {
                                        await receiver.SendMessageAsync("已将本群生产模式修改为：无限供应");
                                    }
                                    catch (Exception e)
                                    {
                                        Logger.Error("群消息发送失败！");
                                        Logger.Debug($"错误信息：\n{e.Message}");
                                    }
                                    break;
                                case "diverse":
                                    Json.ModifyObjectFromArray("breadfactory", "groups", "groupid", receiver.GroupId, "factory_mode", typeof(string), "diverse");
                                    Logger.Info($"有面包厂已将生产模式切换为：“多样化供应”！\n分厂：{receiver.GroupName} ({receiver.GroupId})");
                                    try
                                    {
                                        await receiver.SendMessageAsync("已将本群生产模式修改为：多样化供应");
                                    }
                                    catch (Exception e)
                                    {
                                        Logger.Error("群消息发送失败！");
                                        Logger.Debug($"错误信息：\n{e.Message}");
                                    }
                                    break;
                                case "normal":
                                    Json.ModifyObjectFromArray("breadfactory", "groups", "groupid", receiver.GroupId, "factory_mode", typeof(string), "normal");
                                    Logger.Info($"有面包厂已将生产模式切换为：“单一化供应”！\n分厂：{receiver.GroupName} ({receiver.GroupId})");
                                    try
                                    {
                                        await receiver.SendMessageAsync("已将本群生产模式修改为：单一化供应");
                                    }
                                    catch (Exception e)
                                    {
                                        Logger.Error("群消息发送失败！");
                                        Logger.Debug($"错误信息：\n{e.Message}");
                                    }
                                    break;
                                default:
                                    Logger.Warning($"有分厂未能修改生产模式，因为提供的参数有误！\n分厂：{receiver.GroupName} ({receiver.GroupId})");
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
                        else
                        {
                            Logger.Warning($"有分厂未能修改生产模式，因为面包厂库存不是空的！\n分厂：{receiver.GroupName} ({receiver.GroupId})");
                            try
                            {
                                await receiver.SendMessageAsync("切换生产模式之前，必须先清空面包厂库存！");
                            }
                            catch (Exception e)
                            {
                                Logger.Error("群消息发送失败！");
                                Logger.Debug($"错误信息：\n{e.Message}");
                            }
                        }
                    }
                }
                else
                {
                    Logger.Warning($"生产模式切换请求未能执行，因为该地区尚未兴建面包厂！\n分厂：{receiver.GroupName} ({receiver.GroupId})");
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
            }
        }
    }
}