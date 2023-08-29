// 2kbit C# Edition: New，2kbit的 C# 分支版本的优化方案

// Copyright(C) 2023 Abjust 版权所有。

// 本程序是自由软件：你可以根据自由软件基金会发布的GNU Affero通用公共许可证的条款，即许可证的第3版或（您选择的）任何后来的版本重新发布它和/或修改它。。

// 本程序的发布是希望它能起到作用。但没有任何保证；甚至没有隐含的保证。本程序的分发是希望它是有用的，但没有任何保证，甚至没有隐含的适销对路或适合某一特定目的的保证。 参见 GNU Affero通用公共许可证了解更多细节。

// 您应该已经收到了一份GNU Affero通用公共许可证的副本。 如果没有，请参见<https://www.gnu.org/licenses/>。

/**
* 2kbit C# Edition: New
* 面包厂模块：拿面包
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
    internal class GetBread : IModule
    {
        public bool? IsEnable { get; set; }

        public async void Execute(MessageReceiverBase @base)
        {
            GroupMessageReceiver receiver = @base.Concretize<GroupMessageReceiver>();
            string[] s = receiver.MessageChain.GetPlainMessage().Split(" ");
            if (s[0] == "!getbread")
            {
                if (Json.FileExists("breadfactory") && Json.FileExists("materials") && Json.ObjectExistsInArray("breadfactory", "groups", "groupid", receiver.GroupId))
                {
                    FactoryExpiration.Execute(receiver.GroupId);
                    MaterialsFactory.Produce(receiver.GroupId);
                    BreadFactory.Produce(receiver.GroupId);
                    JObject obj = Json.ReadFile("breadfactory");
                    JObject item = (JObject)obj["groups"]!.Where(x => x.SelectToken("groupid")!.Value<string>()! == receiver.GroupId).FirstOrDefault()!;
                    List<string> bread_types = new()
                                                        {
                                                            char.ConvertFromUtf32(0x1F35E),
                                                            char.ConvertFromUtf32(0x1F956),
                                                            char.ConvertFromUtf32(0x1F950),
                                                            char.ConvertFromUtf32(0x1F96F),
                                                            char.ConvertFromUtf32(0x1F369)
                                                        };
                    string text = " ";
                    int requested_breads = 0;
                    switch (s.Length)
                    {
                        case 2:
                            if (int.TryParse(s[1], out int number) && number >= 1)
                            {
                                requested_breads = number;
                            }
                            else
                            {
                                Logger.Warning($"有面包厂未能供应面包，因为提供的参数有误！\n分厂：{receiver.GroupName} ({receiver.GroupId})");
                                await TrySend.Quote(receiver, "宁看看，宁写的事数字？小学重读去罢（恼）");
                            }
                            break;
                        case 1:
                            requested_breads = 1;
                            break;
                        default:
                            Logger.Warning($"有供货订单未被回应，因为提供的参数有误！\n地区：{receiver.GroupName} ({receiver.GroupId})");
                            await TrySend.Quote(receiver, "捏吗，参数有问题让我怎么执行？（恼）");
                            break;
                    }
                    if (requested_breads != 0 && (((string)item["factory_mode"]!).Contains("infinite") || (int)item["breads"]! >= requested_breads))
                    {
                        switch (((string)item["factory_mode"]!).Contains("diverse"))
                        {
                            case true:
                                Random rnd = new();
                                int[] fields = new int[bread_types.Count];
                                int sum = 0;
                                for (int i = 0; i < fields.Length; i++)
                                {
                                    double exponent = 0.5 - (i * (0.5 / bread_types.Count));
                                    fields[i] = rnd.Next((int)Math.Floor((requested_breads - sum) * (1 - exponent)) + 1);
                                    sum += fields[i];
                                }
                                fields[^1] = requested_breads - sum;
                                switch (requested_breads)
                                {
                                    case <= 20:
                                        for (int i = 0; i < bread_types.Count; i++)
                                        {
                                            text += string.Join("", Enumerable.Repeat(bread_types[i], fields[i]));
                                        }
                                        break;
                                    default:
                                        for (int i = 0; i < bread_types.Count; i++)
                                        {
                                            text += $"\n{bread_types[i]}*{fields[i]}";
                                        }
                                        break;
                                }
                                break;
                            case false:
                                text = requested_breads switch
                                {
                                    <= 20 => string.Join("", Enumerable.Repeat(char.ConvertFromUtf32(0x1F35E), requested_breads)),
                                    _ => $"{char.ConvertFromUtf32(0x1F35E)} * {requested_breads}",
                                };
                                break;
                        }
                        if (!((string)item["factory_mode"]!).Contains("infinite"))
                        {
                            Json.ModifyObjectFromArray("breadfactory", "groups", "groupid", receiver.GroupId, "breads", (int)item["breads"]! - requested_breads);
                        }
                        Logger.Info($"有面包厂向该地区的一名客户送出了 {requested_breads} 块面包！\n分厂：{receiver.GroupName} ({receiver.GroupId})");
                        await TrySend.Quote(receiver, text);
                    }
                    else
                    {
                        Logger.Warning($"有面包厂未能供应 {requested_breads} 块面包，因为该分厂库存内的面包不够！\n分厂：{receiver.GroupName} ({receiver.GroupId})");
                        await TrySend.Quote(receiver, "仓库里面包都不够你吃的，让我拿空气出来？臭啥比（恼）");
                    }
                }
                else
                {
                    Logger.Warning($"有地区未能响应该地区客户的供货订单，因为该地区尚未兴建面包厂！\n地区：{receiver.GroupName} ({receiver.GroupId})");
                    await TrySend.Quote(receiver, "这b群，踏马连个面包厂都没有（恼）");
                }
            }
        }
    }
}