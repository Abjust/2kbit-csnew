// 2kbit C# Edition: New，2kbit的 C# 分支版本的优化方案

// Copyright(C) 2023 Abjust 版权所有。

// 本程序是自由软件：你可以根据自由软件基金会发布的GNU Affero通用公共许可证的条款，即许可证的第3版或（您选择的）任何后来的版本重新发布它和/或修改它。。

// 本程序的发布是希望它能起到作用。但没有任何保证；甚至没有隐含的保证。本程序的分发是希望它是有用的，但没有任何保证，甚至没有隐含的适销对路或适合某一特定目的的保证。 参见 GNU Affero通用公共许可证了解更多细节。

// 您应该已经收到了一份GNU Affero通用公共许可证的副本。 如果没有，请参见<https://www.gnu.org/licenses/>。

/**
* 2kbit C# Edition: New
* 面包厂模块：查询面包厂信息
**/

using Mirai.Net.Data.Messages;
using Mirai.Net.Data.Messages.Receivers;
using Mirai.Net.Modules;
using Mirai.Net.Utils.Scaffolds;
using Net_Codeintp_cs.Modules.Group.Tasks;
using Net_Codeintp_cs.Modules.Utils;
using Newtonsoft.Json.Linq;

namespace Net_Codeintp_cs.Modules.Group.Commands.Bread
{
    internal class Query : IModule
    {
        public const int breadfactory_maxlevel = 5;
        public bool? IsEnable { get; set; }

        public async void Execute(MessageReceiverBase @base)
        {
            GroupMessageReceiver receiver = @base.Concretize<GroupMessageReceiver>();
            string[] s = receiver.MessageChain.GetPlainMessage().Split(" ");
            if (s[0] == "!query")
            {
                if (Json.FileExists("breadfactory") && Json.FileExists("materials") && Json.ObjectExistsInArray("breadfactory", "groups", "groupid", receiver.GroupId))
                {
                    FactoryExpiration.Execute(receiver.GroupId);
                    MaterialsFactory.Produce(receiver.GroupId);
                    BreadFactory.Produce(receiver.GroupId);
                    int flour = 0, egg = 0, yeast = 0;
                    JObject obj = Json.ReadFile("materials");
                    foreach (JObject item in ((JArray)obj["groups"]!).Cast<JObject>())
                    {
                        if ((string)item["groupid"]! == receiver.GroupId)
                        {
                            flour = (int)item["flour"]!;
                            egg = (int)item["egg"]!;
                            yeast = (int)item["yeast"]!;
                            break;
                        }
                    }
                    obj = Json.ReadFile("breadfactory");
                    bool is_maxlevel = false;
                    string mode = "";
                    string expiry;
                    foreach (JObject item in ((JArray)obj["groups"]!).Cast<JObject>())
                    {
                        if ((string)item["groupid"]! == receiver.GroupId)
                        {
                            if ((int)item["factory_level"]! == breadfactory_maxlevel)
                            {
                                is_maxlevel = true;
                            }
                            switch ((string)item["factory_mode"]!)
                            {
                                case "infinite_diverse":
                                    mode = "多样化无限供应";
                                    break;
                                case "infinite":
                                    mode = "无限供应";
                                    break;
                                case "diverse":
                                    mode = "多样化供应";
                                    break;
                                case "normal":
                                    mode = "单一化供应";
                                    break;
                            }
                            expiry = (int)item["expiration"]! switch
                            {
                                >= 1 => $"{(int)item["expiration"]!} 天",
                                _ => "永不过期",
                            };
                            string properties = @$"
面包厂等级：{(int)item["factory_level"]!} / {breadfactory_maxlevel} 级
面包厂经验：{(int)item["factory_exp"]!} XP
今日已获得经验：{(int)item["exp_gained_today"]!} / {(int)(300 * Math.Pow(2, (int)item["factory_level"]! - 1))} XP
生产（供应）模式：{mode}";
                            if (is_maxlevel)
                            {
                                properties = @$"
面包厂等级： {breadfactory_maxlevel} 级（满级）
库存升级次数：{(int)item["storage_level"]!} 次
生产速度升级次数：{(int)item["speed_level"]!} 次
产量升级次数：{(int)item["output_level"]!} 次
面包厂经验：{(int)item["factory_exp"]!} XP
今日已获得经验：{(int)item["exp_gained_today"]!} / {(int)(300 * Math.Pow(2, (int)item["factory_level"]! - 1))} XP
生产（供应）模式：{mode}";
                            }
                            MessageChain messageChain = new MessageChainBuilder()
                                    .At(receiver.Sender.Id)
                                    .Plain($@"
本群 ({receiver.GroupId}) 面包厂信息如下：
-----面包厂属性-----
{properties.Trim()}
-----面包厂配置-----
面包库存上限：{(int)(64 * Math.Pow(4, (int)item["factory_level"]! - 1)) * Math.Pow(2, (int)item["storage_level"]!)} 块
生产周期：{300 - 20 * ((int)item["factory_level"]! - 1) - 10 * (int)item["speed_level"]!} 秒
每周期最大产量：{(int)Math.Pow(4, (int)item["factory_level"]!) * (int)Math.Pow(2, (int)item["output_level"]!)} 块
批次保质期：{expiry}
-----物品库存-----
现有原材料：{flour} 份面粉、{egg} 份鸡蛋、{yeast} 份酵母
现有面包：{(int)item["breads"]!} / {(int)(64 * Math.Pow(4, (int)item["factory_level"]! - 1)) * Math.Pow(2, (int)item["storage_level"]!)} 块
")
                                    .Build();
                            try
                            {
                                await receiver.QuoteMessageAsync(messageChain);
                                Logger.Info($"已提供分厂“{receiver.GroupName} ({receiver.GroupId})”的完整信息！");
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
                else
                {
                    Logger.Warning($"未能提供地区“{receiver.GroupName} ({receiver.GroupId})”的分厂信息，因为该地区尚未兴建面包厂！");
                    await TrySend.Quote(receiver, "这b群，踏马连个面包厂都没有（恼）");
                }
            }
        }
    }
}
