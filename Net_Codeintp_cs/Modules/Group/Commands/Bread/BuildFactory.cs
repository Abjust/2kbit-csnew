// 2kbit C# Edition: New，2kbit的 C# 分支版本的优化方案

// Copyright(C) 2023 Abjust 版权所有。

// 本程序是自由软件：你可以根据自由软件基金会发布的GNU Affero通用公共许可证的条款，即许可证的第3版或（您选择的）任何后来的版本重新发布它和/或修改它。。

// 本程序的发布是希望它能起到作用。但没有任何保证；甚至没有隐含的保证。本程序的分发是希望它是有用的，但没有任何保证，甚至没有隐含的适销对路或适合某一特定目的的保证。 参见 GNU Affero通用公共许可证了解更多细节。

// 您应该已经收到了一份GNU Affero通用公共许可证的副本。 如果没有，请参见<https://www.gnu.org/licenses/>。

/**
* 2kbit C# Edition: New
* 面包厂模块：建造面包厂
**/

using Mirai.Net.Data.Messages;
using Mirai.Net.Data.Messages.Receivers;
using Mirai.Net.Modules;
using Mirai.Net.Utils.Scaffolds;
using Net_Codeintp_cs.Modules.Utils;
using Newtonsoft.Json.Linq;

namespace Net_Codeintp_cs.Modules.Group.Commands.Bread
{
    internal class BuildFactory : IModule
    {
        public bool? IsEnable { get; set; }

        public async void Execute(MessageReceiverBase @base)
        {
            GroupMessageReceiver receiver = @base.Concretize<GroupMessageReceiver>();
            string[] s = receiver.MessageChain.GetPlainMessage().Split(" ");
            if (s[0] == "!build_factory")
            {
                if (!Json.FileExists("breadfactory"))
                {
                    JObject obj = new(
                        new JProperty("groups",
                        new JArray()));
                    Json.CreateFile("breadfactory", obj);
                }
                if (!Json.FileExists("materials"))
                {
                    JObject obj = new(
                        new JProperty("groups",
                        new JArray()));
                    Json.CreateFile("materials", obj);
                }
                long TimeNow = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();
                if (!Json.ObjectExistsInArray("breadfactory", "groups", "groupid", receiver.GroupId))
                {
                    JObject obj = new(
                        new JProperty("groupid", receiver.GroupId),
                        new JProperty("factory_level", 1),
                        new JProperty("storage_level", 0),
                        new JProperty("speed_level", 0),
                        new JProperty("output_level", 0),
                        new JProperty("factory_mode", "normal"),
                        new JProperty("factory_exp", 0),
                        new JProperty("breads", 0),
                        new JProperty("exp_gained_today", 0),
                        new JProperty("last_expfull", 946656000),
                        new JProperty("last_expgain", 946656000),
                        new JProperty("last_produce", TimeNow),
                        new JProperty("next_expiry", TimeNow - (TimeNow + 8 * 3600) % 86400 + 259200),
                        new JProperty("expiration", 3)
                        );
                    Json.AddObjectToArray("breadfactory", "groups", obj);
                    JObject obj1 = new(
                        new JProperty("groupid", receiver.GroupId),
                        new JProperty("flour", 0),
                        new JProperty("egg", 0),
                        new JProperty("yeast", 0),
                        new JProperty("last_produce", TimeNow),
                        new JProperty("next_expiry", TimeNow - (TimeNow + 8 * 3600) % 86400 + 259200)
                        );
                    Json.AddObjectToArray("materials", "groups", obj1);
                    Logger.Info($"已为地区“{receiver.GroupName} ({receiver.GroupId})”兴建面包厂！\n执行者：{receiver.Sender.Name} ({receiver.Sender.Id})");
                    await TrySend.Quote(receiver, "成功为本群建造面包厂！");
                }
                else
                {
                    Logger.Warning($"未尝试为地区“{receiver.GroupName} ({receiver.GroupId})”兴建面包厂，因为该地区已有面包厂！\n执行者：{receiver.Sender.Name} ({receiver.Sender.Id})");
                    await TrySend.Quote(receiver, "这b群踏马已经有面包厂了（恼）");
                }
            }
        }
    }
}
