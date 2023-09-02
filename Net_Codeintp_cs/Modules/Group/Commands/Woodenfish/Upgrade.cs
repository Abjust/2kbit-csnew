// 2kbit C# Edition: New，2kbit的 C# 分支版本的优化方案

// Copyright(C) 2023 Abjust 版权所有。

// 本程序是自由软件：你可以根据自由软件基金会发布的GNU Affero通用公共许可证的条款，即许可证的第3版或（您选择的）任何后来的版本重新发布它和/或修改它。。

// 本程序的发布是希望它能起到作用。但没有任何保证；甚至没有隐含的保证。本程序的分发是希望它是有用的，但没有任何保证，甚至没有隐含的适销对路或适合某一特定目的的保证。 参见 GNU Affero通用公共许可证了解更多细节。

// 您应该已经收到了一份GNU Affero通用公共许可证的副本。 如果没有，请参见<https://www.gnu.org/licenses/>。

/**
* 2kbit C# Edition: New
* 木鱼模块：升级木鱼
**/

using Microsoft.International.Converters.TraditionalChineseToSimplifiedConverter;
using Mirai.Net.Data.Messages;
using Mirai.Net.Data.Messages.Receivers;
using Mirai.Net.Modules;
using Mirai.Net.Utils.Scaffolds;
using Net_Codeintp_cs.Modules.Utils;
using Newtonsoft.Json.Linq;

namespace Net_Codeintp_cs.Modules.Group.Commands.Woodenfish
{
    internal class Upgrade : IModule
    {
        public bool? IsEnable { get; set; }

        public async void Execute(MessageReceiverBase @base)
        {
            GroupMessageReceiver receiver = @base.Concretize<GroupMessageReceiver>();
            string[] s = receiver.MessageChain.GetPlainMessage().Split(" ");
            if (ChineseConverter.Convert(s[0], ChineseConversionDirection.TraditionalToSimplified) == "升级木鱼")
            {
                if (Json.FileExists("woodenfish") && Json.ObjectExistsInArray("woodenfish", "players", "playerid", receiver.Sender.Id))
                {
                    JObject obj = Json.ReadFile("woodenfish");
                    JObject item = (JObject)obj["players"]!.Where(x => x.SelectToken("playerid")!.Value<string>()! == receiver.Sender.Id).FirstOrDefault()!;
                    int maxlevel = 200;
                    if ((int)item["ban"]! == 0)
                    {
                        if ((int)item["level"]! < maxlevel)
                        {
                            int needed_e = 0;
                            int upgrades = 0;
                            // 升级所需功德（e值）公式
                            int formula = (int)item["level"]! + 1;
                            switch (s.Length)
                            {
                                case 2:
                                    if (int.TryParse(s[1], out int number))
                                    {
                                        upgrades = number;
                                        if ((int)item["level"]! + number <= maxlevel)
                                        {
                                            for (int i = 0; i < number; i++)
                                            {
                                                needed_e += formula + i;
                                            }
                                        }
                                        else
                                        {
                                            await TrySend.Quote(receiver, "宁踏马再升级就超过满级辣（恼）");
                                        }
                                    }
                                    else
                                    {
                                        await TrySend.Quote(receiver, "宁看看，宁写的事数字？小学重读去罢（恼）");
                                    }
                                    break;
                                case 1:
                                    needed_e = formula;
                                    upgrades = 1;
                                    if (Math.Pow(10, (double)item["ee"]!) + (double)item["e"]! >= needed_e)
                                    {
                                        if ((double)item["e"]! >= needed_e)
                                        {
                                            Json.ModifyObjectFromArray("woodenfish", "players", "playerid", receiver.Sender.Id, "level", (int)item["level"]! + 1);
                                            Json.ModifyObjectFromArray("woodenfish", "players", "playerid", receiver.Sender.Id, "e", (double)item["e"]! - needed_e);
                                        }
                                        else
                                        {
                                            Json.ModifyObjectFromArray("woodenfish", "players", "playerid", receiver.Sender.Id, "level", (int)item["level"]! + 1);
                                            Json.ModifyObjectFromArray("woodenfish", "players", "playerid", receiver.Sender.Id, "ee", Math.Log10(Math.Pow(10, (double)item["ee"]!) + (double)item["e"]! - needed_e));
                                        }
                                    }
                                    break;
                                default:
                                    await TrySend.Quote(receiver, "捏吗，参数有问题让我怎么执行？（恼）");
                                    break;
                            }
                            if (needed_e != 0 && Math.Pow(10, (double)item["ee"]!) + (double)item["e"]! >= needed_e)
                            {
                                if ((double)item["e"]! >= needed_e)
                                {
                                    Json.ModifyObjectFromArray("woodenfish", "players", "playerid", receiver.Sender.Id, "level", (int)item["level"]! + upgrades);
                                    Json.ModifyObjectFromArray("woodenfish", "players", "playerid", receiver.Sender.Id, "e", (double)item["e"]! - needed_e);
                                }
                                else
                                {
                                    Json.ModifyObjectFromArray("woodenfish", "players", "playerid", receiver.Sender.Id, "level", (int)item["level"]! + upgrades);
                                    Json.ModifyObjectFromArray("woodenfish", "players", "playerid", receiver.Sender.Id, "ee", Math.Log10(Math.Pow(10, (double)item["ee"]!) + (double)item["e"]! - needed_e));
                                }
                                await TrySend.Quote(receiver, "木鱼升级成功辣（喜）");
                            }
                            else if (needed_e != 0 && Math.Pow(10, (double)item["ee"]!) + (double)item["e"]! < needed_e)
                            {
                                await TrySend.Quote(receiver, "宁踏马功德不够，升级个毛啊（恼）");
                            }
                        }
                        else
                        {
                            await TrySend.Quote(receiver, "宁踏马已经满级辣（恼）");
                        }
                    }
                    else
                    {
                        await TrySend.Quote(receiver, "宁踏马被佛祖封号辣（恼）");
                    }
                }
                else
                {
                    await TrySend.Quote(receiver, "宁踏马害没注册？快发送“给我木鱼”注册罢！");
                }
            }
        }
    }
}
