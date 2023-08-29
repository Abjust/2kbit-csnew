// 2kbit C# Edition: New，2kbit的 C# 分支版本的优化方案

// Copyright(C) 2023 Abjust 版权所有。

// 本程序是自由软件：你可以根据自由软件基金会发布的GNU Affero通用公共许可证的条款，即许可证的第3版或（您选择的）任何后来的版本重新发布它和/或修改它。。

// 本程序的发布是希望它能起到作用。但没有任何保证；甚至没有隐含的保证。本程序的分发是希望它是有用的，但没有任何保证，甚至没有隐含的适销对路或适合某一特定目的的保证。 参见 GNU Affero通用公共许可证了解更多细节。

// 您应该已经收到了一份GNU Affero通用公共许可证的副本。 如果没有，请参见<https://www.gnu.org/licenses/>。

/**
* 2kbit C# Edition: New
* 面包厂模块：设置保质期
**/

using Mirai.Net.Data.Messages;
using Mirai.Net.Data.Messages.Receivers;
using Mirai.Net.Modules;
using Mirai.Net.Utils.Scaffolds;
using Net_Codeintp_cs.Modules.Utils;
using Newtonsoft.Json.Linq;

namespace Net_Codeintp_cs.Modules.Group.Commands.Bread
{
    internal class SetExpiry : IModule
    {
        public bool? IsEnable { get; set; }

        public async void Execute(MessageReceiverBase @base)
        {
            GroupMessageReceiver receiver = @base.Concretize<GroupMessageReceiver>();
            string[] s = receiver.MessageChain.GetPlainMessage().Split(" ");
            if (s[0] == "!set_expiry")
            {
                if (Json.FileExists("breadfactory") && Json.FileExists("materials") && Json.ObjectExistsInArray("breadfactory", "groups", "groupid", receiver.GroupId))
                {
                    JObject obj = Json.ReadFile("breadfactory");
                    JObject item = (JObject)obj["groups"]!.Where(x => x.SelectToken("groupid")!.Value<string>()! == receiver.GroupId).FirstOrDefault()!;
                    if (s.Length == 2 && int.TryParse(s[1], out int expiry) && expiry != (int)item["expiration"]!)
                    {
                        if (expiry == 0 || ((expiry >= 1) && (expiry <= 30)))
                        {
                            long TimeNow = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();
                            Json.ModifyObjectFromArray("breadfactory", "groups", "groupid", receiver.GroupId, "expiration", expiry);
                            Json.ModifyObjectFromArray("breadfactory", "groups", "groupid", receiver.GroupId, "breads", 0);
                            Json.ModifyObjectFromArray("materials", "groups", "groupid", receiver.GroupId, "flour", 0);
                            Json.ModifyObjectFromArray("materials", "groups", "groupid", receiver.GroupId, "egg", 0);
                            Json.ModifyObjectFromArray("materials", "groups", "groupid", receiver.GroupId, "yeast", 0);
                            if (expiry == 0)
                            {
                                await TrySend.Quote(receiver, "已将面包厂保质期设置为：永不过期\n（本批次库存已自动清空）");
                            }
                            else
                            {
                                Json.ModifyObjectFromArray("breadfactory", "groups", "groupid", receiver.GroupId, "last_produce", TimeNow);
                                Json.ModifyObjectFromArray("breadfactory", "groups", "groupid", receiver.GroupId, "next_expiry", TimeNow + expiry * 86400);
                                Json.ModifyObjectFromArray("materials", "groups", "groupid", receiver.GroupId, "last_produce", TimeNow);
                                Json.ModifyObjectFromArray("materials", "groups", "groupid", receiver.GroupId, "next_expiry", TimeNow + expiry * 86400);
                                await TrySend.Quote(receiver, $"已将面包厂保质期设置为：{expiry} 天\n（本批次库存已自动清空）");
                            }
                        }
                        else
                        {
                            await TrySend.Quote(receiver, "保质期只能介于1~30天，也可设置为0（永不过期）");
                        }
                    }
                    else
                    {
                        await TrySend.Quote(receiver, "捏吗，参数有问题让我怎么执行？（恼）（注意：新设置的过期时间不能和现在的一样）");
                    }
                }
                else
                {
                    await TrySend.Quote(receiver, "这b群，踏马连个面包厂都没有（恼）");
                }
            }
        }
    }
}
