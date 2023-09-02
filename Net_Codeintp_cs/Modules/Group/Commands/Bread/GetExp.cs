// 2kbit C# Edition: New，2kbit的 C# 分支版本的优化方案

// Copyright(C) 2023 Abjust 版权所有。

// 本程序是自由软件：你可以根据自由软件基金会发布的GNU Affero通用公共许可证的条款，即许可证的第3版或（您选择的）任何后来的版本重新发布它和/或修改它。。

// 本程序的发布是希望它能起到作用。但没有任何保证；甚至没有隐含的保证。本程序的分发是希望它是有用的，但没有任何保证，甚至没有隐含的适销对路或适合某一特定目的的保证。 参见 GNU Affero通用公共许可证了解更多细节。

// 您应该已经收到了一份GNU Affero通用公共许可证的副本。 如果没有，请参见<https://www.gnu.org/licenses/>。

/**
* 2kbit C# Edition: New
* 面包厂模块：获取经验
**/

using Mirai.Net.Data.Messages;
using Mirai.Net.Data.Messages.Receivers;
using Mirai.Net.Modules;
using Mirai.Net.Utils.Scaffolds;
using Net_Codeintp_cs.Modules.Utils;
using Newtonsoft.Json.Linq;

namespace Net_Codeintp_cs.Modules.Group.Commands.Bread
{
    internal class GetExp : IModule
    {
        public bool? IsEnable { get; set; }

        public async void Execute(MessageReceiverBase @base)
        {
            GroupMessageReceiver receiver = @base.Concretize<GroupMessageReceiver>();
            if (Json.FileExists("breadfactory") && Json.FileExists("materials"))
            {
                if (Json.ObjectExistsInArray("breadfactory", "groups", $"groupid", receiver.GroupId))
                {
                    JObject obj = Json.ReadFile("breadfactory");
                    JObject item = (JObject)obj["groups"]!.Where(x => x.SelectToken("groupid")!.Value<string>()! == receiver.GroupId).FirstOrDefault()!;
                    // 如果上次达到经验上限距离现在超过24小时
                    if (DateTimeOffset.UtcNow.ToUnixTimeSeconds() - (long)item["last_expfull"]! >= 86400)
                    {
                        // 判断是否是24小时内第一次获得经验
                        bool first_expgain = DateTimeOffset.UtcNow.ToUnixTimeSeconds() - (long)item["last_expgain"]! >= 86400;
                        // 如果还没有达到经验上限
                        if ((int)item["exp_gained_today"]! < (300 * Math.Pow(2, (int)item["factory_level"]! - 1)))
                        {
                            Json.ModifyObjectFromArray("breadfactory", "groups", "groupid", receiver.GroupId, "last_expgain", DateTimeOffset.UtcNow.ToUnixTimeSeconds());
                            Json.ModifyObjectFromArray("breadfactory", "groups", "groupid", receiver.GroupId, "factory_exp", (int)item["factory_exp"]! + 1);
                            Json.ModifyObjectFromArray("breadfactory", "groups", "groupid", receiver.GroupId, "exp_gained_today", first_expgain ? 1 : (int)item["exp_gained_today"]! + 1);
                        }
                        // 如果达到经验上限
                        else
                        {
                            Json.ModifyObjectFromArray("breadfactory", "groups", "groupid", receiver.GroupId, "last_expfull", DateTimeOffset.UtcNow.ToUnixTimeSeconds());
                            try
                            {
                                await receiver.SendMessageAsync("本群已达到今日获取经验上限！");
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
    }
}