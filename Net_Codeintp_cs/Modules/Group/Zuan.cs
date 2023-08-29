// 2kbit C# Edition: New，2kbit的 C# 分支版本的优化方案

// Copyright(C) 2023 Abjust 版权所有。

// 本程序是自由软件：你可以根据自由软件基金会发布的GNU Affero通用公共许可证的条款，即许可证的第3版或（您选择的）任何后来的版本重新发布它和/或修改它。。

// 本程序的发布是希望它能起到作用。但没有任何保证；甚至没有隐含的保证。本程序的分发是希望它是有用的，但没有任何保证，甚至没有隐含的适销对路或适合某一特定目的的保证。 参见 GNU Affero通用公共许可证了解更多细节。

// 您应该已经收到了一份GNU Affero通用公共许可证的副本。 如果没有，请参见<https://www.gnu.org/licenses/>。

/**
* 2kbit C# Edition: New
* 祖安模块
**/

using Mirai.Net.Data.Messages;
using Mirai.Net.Sessions.Http.Managers;
using Mirai.Net.Utils.Scaffolds;
using Net_Codeintp_cs.Modules.Group.Commands.Admin;
using Net_Codeintp_cs.Modules.Utils;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using RestSharp;

namespace Net_Codeintp_cs.Modules.Group
{
    internal class Zuan
    {
        public static async void Execute(string group, string target)
        {
            string[] words =
                            {
                                 "cnmd",
                                 "你更是歌姬吧嗷",
                                 "你个狗比玩意",
                                 "你是不是被抛上去3次，却只被接住2次？",
                                 "你真是小母牛坐灯泡，牛逼一闪又一闪",
                                 "小嘴像抹了开塞露一样",
                                 "小东西长得真随机",
                                 "我只想骂人，但不想骂你",
                                 "但凡你有点用，也不至于一点用处都没有",
                                 "你还真把自己当个人看了，你也配啊",
                                 "那么丑的脸，就可以看出你是金针菇",
                                 "阁下长得真是天生励志",
                                 "装逼对你来说就像一日三餐的事",
                                 "我怎么敢碰你呢，我怕我买洗手液买穷自己",
                                 "狗咬了你，你还能咬回狗吗",
                                 "你是独一无二的，至少全人类都不希望再有第二个",
                                 "你的智商和喜马拉雅山的氧气一样，稀薄",
                                 "别人的脸是七分天注定，三分靠打扮，你的脸是一分天注定，九分靠滤镜",
                                 "偶尔也要活得强硬一点，软得像滩烂泥一样有什么意思",
                                 "任何人工智能都敌不过阁下这款天然呆",
                                 "我骂你是为了你好，你应该从中学到些什么，比如说自知之明",
                                 "你要好好做自己，反正别的你也做不好",
                                 "如果国家把长相分等级的话，你的长相，都可以吃低保了",
                                 "你没权利看不惯我的生活方式，但你有权抠瞎自己的双眼",
                                 "如果你觉得我哪里不对，请一定要告诉我，反正我也不会改，你别憋出病来",
                                 "你（  ）什么时候（  ）啊",
                                 "四吗玩意，说我是歌姬吧，你怎么不撒泡尿照照镜子看看你自己，狗比玩意",
                                 "握草泥马呀—\r\n我操尼玛啊啊啊啊—\r\n我—操—你—妈—\r\n听到没，我—操—你—妈—"
                            };
            Random r = new();
            bool fetch = r.Next(2) == 1;
            int index = r.Next(words.Length);
            string word = "";
            if (fetch)
            {
                List<string> apis = new()
                {
                    "https://api.qhsou.com/api/zuan.php",
                    "http://api.qemao.com/api/yulu/?type=2"
                };
                Random r1 = new();
                int api_index = r.Next(apis.Count);
                try
                {
                    RestClient client = new(apis[api_index]);
                    RestRequest request = new("", Method.Get)
                    {
                        Timeout = 10000
                    };
                    RestResponse response = await client.ExecuteAsync(request);
                    word = response.Content!;
                }
                catch (Exception e)
                {
                    Logger.Error("API拉取失败！（将使用现有词库）");
                    Logger.Debug($"错误信息：\n{e.Message}");
                    word = words[index];
                }
            }
            else
            {
                word = words[index];
            }
            MessageChain? messageChain = new MessageChainBuilder()
                    .At(target)
                    .Plain($" {word}")
                    .Build();
            try
            {
                await MessageManager.SendGroupMessageAsync(group, messageChain);
            }
            catch (Exception e)
            {
                Logger.Error("群消息发送失败！");
                Logger.Debug($"错误信息：\n{e.Message}");
            }
        }
    }
}
