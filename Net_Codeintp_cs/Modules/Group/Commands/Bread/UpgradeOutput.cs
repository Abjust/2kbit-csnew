// 2kbit C# Edition: New，2kbit的 C# 分支版本的优化方案

// Copyright(C) 2023 Abjust 版权所有。

// 本程序是自由软件：你可以根据自由软件基金会发布的GNU Affero通用公共许可证的条款，即许可证的第3版或（您选择的）任何后来的版本重新发布它和/或修改它。。

// 本程序的发布是希望它能起到作用。但没有任何保证；甚至没有隐含的保证。本程序的分发是希望它是有用的，但没有任何保证，甚至没有隐含的适销对路或适合某一特定目的的保证。 参见 GNU Affero通用公共许可证了解更多细节。

// 您应该已经收到了一份GNU Affero通用公共许可证的副本。 如果没有，请参见<https://www.gnu.org/licenses/>。

/**
* 2kbit C# Edition: New
* 面包厂模块：升级产量等级
**/

using Mirai.Net.Data.Messages;
using Mirai.Net.Data.Messages.Receivers;
using Mirai.Net.Modules;
using Mirai.Net.Utils.Scaffolds;
using Net_Codeintp_cs.Modules.Utils;
using Newtonsoft.Json.Linq;

namespace Net_Codeintp_cs.Modules.Group.Commands.Bread
{
    internal class UpgradeOutput : IModule
    {
        public bool? IsEnable { get; set; }

        public async void Execute(MessageReceiverBase @base)
        {
            GroupMessageReceiver receiver = @base.Concretize<GroupMessageReceiver>();
            string[] s = receiver.MessageChain.GetPlainMessage().Split(" ");
            if (s[0] == "!upgrade_output")
            {
                if (Json.FileExists("breadfactory") && Json.FileExists("materials") && Json.ObjectExistsInArray("breadfactory", "groups", $"groupid", receiver.GroupId))
                {
                    JObject obj = Json.ReadFile("breadfactory");
                    JObject item = (JObject)obj["groups"]!.Where(x => x.SelectToken("groupid")!.Value<string>()! == receiver.GroupId).FirstOrDefault()!;
                    if ((int)item["factory_level"]! == Query.breadfactory_maxlevel)
                    {
                        if ((int)item["output_level"]! != 16)
                        {
                            int formula = (int)Math.Ceiling(4800 * Math.Pow(1.21, (int)item["output_level"]!));
                            if ((int)item["factory_exp"]! >= formula)
                            {
                                Json.ModifyObjectFromArray("breadfactory", "groups", "groupid", receiver.GroupId, "output_level", (int)item["output_level"]! + 1);
                                Json.ModifyObjectFromArray("breadfactory", "groups", "groupid", receiver.GroupId, "factory_exp", (int)item["factory_exp"]! - formula);
                                Logger.Warning($"已升级“{receiver.GroupName} ({receiver.GroupId})”的产量等级！");
                                await TrySend.Quote(receiver, $"已将面包厂产量等级升级到 {(int)item["output_level"]! + 1} 级");
                            }
                            else
                            {
                                Logger.Warning($"未尝试升级“{receiver.GroupName} ({receiver.GroupId})”的产量等级，因为该分厂积累的经验值不够！");
                                await TrySend.Quote(receiver, $"升级产量等级需要 {formula} 点经验，但是本群面包厂还差 {formula - (int)item["factory_exp"]!} 点经验");
                            }
                        }
                        else
                        {
                            Logger.Warning($"未尝试升级“{receiver.GroupName} ({receiver.GroupId})”的产量等级，因为该分厂的产量等级已经满级！");
                            await TrySend.Quote(receiver, "本群面包厂产量等级已经满级了！");
                        }
                    }
                    else
                    {
                        Logger.Warning($"未尝试升级“{receiver.GroupName} ({receiver.GroupId})”的产量等级，因为该分厂尚未满级！");
                        await TrySend.Quote(receiver, "这b群，面包厂踏马还没满级（恼）");
                    }
                }
                else
                {
                    Logger.Warning($"未尝试升级“{receiver.GroupName} ({receiver.GroupId})”的产量等级，因为该地区尚未兴建面包厂！");
                    await TrySend.Quote(receiver, "这b群，踏马连个面包厂都没有（恼）");
                }
            }
        }
    }
}