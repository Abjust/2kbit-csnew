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
using Net_Codeintp_cs.Modules.Group.Tasks;
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
                if (Json.FileExists("breadfactory") && Json.FileExists("materials") && Json.ObjectExistsInArray("breadfactory", "groups", "groupid", receiver.GroupId))
                {
                    FactoryExpiration.Execute(receiver.GroupId);
                    MaterialsFactory.Produce(receiver.GroupId);
                    BreadFactory.Produce(receiver.GroupId);
                    JObject obj = Json.ReadFile("breadfactory");
                    JObject item = (JObject)obj["groups"]!.Where(x => x.SelectToken("groupid")!.Value<string>()! == receiver.GroupId).FirstOrDefault()!;
                    int supplied_breads = 0;
                    if ((string)item["factory_mode"]! == "normal")
                    {
                        switch (s.Length)
                        {
                            case 2:
                                if (int.TryParse(s[1], out int number) && number >= 1)
                                {
                                    supplied_breads = number;
                                }
                                else
                                {
                                    Logger.Warning($"有面包未能送入该分厂，因为提供的参数有误！\n分厂：{receiver.GroupName} ({receiver.GroupId})");
                                    await TrySend.Quote(receiver, "宁看看，宁写的事数字？小学重读去罢（恼）");
                                }
                                break;
                            case 1:
                                supplied_breads = 1;
                                break;
                            default:
                                Logger.Warning($"有进货订单未被回应，因为提供的参数有误！\n地区：{receiver.GroupName} ({receiver.GroupId})");
                                await TrySend.Quote(receiver, "捏吗，参数有问题让我怎么执行？（恼）");
                                break;
                        }
                        if ((int)item["breads"]! + supplied_breads <= (int)(64 * Math.Pow(4, (int)item["factory_level"]! - 1) * Math.Pow(2, (int)item["storage_level"]!)))
                        {
                            Json.ModifyObjectFromArray("breadfactory", "groups", "groupid", receiver.GroupId, "breads", (int)item["breads"]! + supplied_breads);
                            Logger.Info($"有面包厂接收了 {supplied_breads} 块面包，现在该分厂仓库有 {(int)item["breads"]! + supplied_breads} 块面包！\n分厂：{receiver.GroupName} ({receiver.GroupId})");
                            await TrySend.Quote(receiver, $"现在库存有 {(int)item["breads"]! + supplied_breads} 块面包辣！");
                        }
                        else
                        {
                            Logger.Warning($"有面包厂未能接收 {supplied_breads} 块面包，因为该分厂仓库已满！\n分厂：{receiver.GroupName} ({receiver.GroupId})");
                            await TrySend.Quote(receiver, "库存满了就不要塞面包进来了，老蝉（恼）");
                        }
                    }
                }
                else
                {
                    Logger.Warning($"有面包未能送入该地区，因为该地区尚未兴建面包厂！\n地区：{receiver.GroupName} ({receiver.GroupId})");
                    await TrySend.Quote(receiver, "这b群，踏马连个面包厂都没有（恼）");
                }
            }
        }
    }
}