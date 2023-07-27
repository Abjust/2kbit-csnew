using JiebaNet.Segmenter;
using Mirai.Net.Data.Messages;
using Mirai.Net.Data.Messages.Receivers;
using Mirai.Net.Modules;
using Mirai.Net.Utils.Scaffolds;
using System.Collections.Generic;

namespace Net_Codeintp_cs.Modules.Group
{
    internal class DangerDetection : IModule
    {
        public bool? IsEnable { get; set; }

        public async void Execute(MessageReceiverBase @base)
        {
            var receiver = @base.Concretize<GroupMessageReceiver>();
            List<string> words = new()
                        {
                            "我想自杀",
                            "我想自残",
                            "我想死",
                            "想自杀",
                            "想自残",
                            "我想zs",
                            "我想zc",
                            "我想s",
                            "想zs",
                            "想zc",
                            "想s",
                            "心烦",
                            "我累了",
                            "我一点都不难过",
                            "我很开心啊"
                        };
            List<string> test = new();

            foreach (string word in words)
            {
                // (IEnumerable<string>, IEnumerable<string>) list = Similarity.Divide(word, receiver.MessageChain.GetPlainMessage().ToLower());
                // List<string> all_words = Similarity.GetAllWords(list.Item1, list.Item2);
                // (List<int>, List<int>) lst1 = Similarity.GetWordVector(list.Item1, list.Item2, all_words);
                // double cos = Similarity.CalculateCosine(lst1.Item1, lst1.Item2);

                if (receiver.MessageChain.GetPlainMessage().ToLower() =="djafhljsdhkjflhasdkjlfhkjsadhfk")
                {
                    List<string> poems = new()
                                {
                                    @"
假如生活欺骗了你，
不要悲伤，不要心急！
忧郁的日子里须要镇静：
相信吧，快乐的日子将会来临！
心儿永远向往着未来；
现在却常是忧郁：
一切都是瞬息，一切都将会过去；
而那过去了的，就会成为亲切的怀恋。
",
                                    @"
整个自然都是艺术，不过你不领悟；
一切偶然都是规定，只是你没有看清；
一切不协，是你不理解的和谐；
一切局部的祸，乃是全体的福。
高傲可鄙，只因它不近情理。
凡存在的都合理，乃是清楚的道理。
",
                                    " 感谢科学，它不仅使生活充满快乐与欢欣，并且给生活以支柱和自尊心。",
                                    " 生活本来是个沉重的话题，它带给我们压力责任和使命，但如果我们用好的心态去面对，用宽广的胸怀去接纳，那快乐就会不期而至，伴随左右。心态决定想法，想法决定做法，做法决定结果。",
                                    " 应当赶紧地，充分地生活，因为意外的疾病或悲惨的事故随时都可以突然结束他的生命。",
                                    " 人是用心去活，而不是用脸去活，用心去活无非是认真的去感受生活中的点滴，享受生活的每一次给予。",
                                    " 生活的真谛就是懂得享受生活，而享受生活的真正目的就是使自己的心情达到一种舒畅或平静的状态，做事完全是自觉、自愿而且带着兴趣的。随心所欲并不是指金钱的方向和改变，而是指心灵的自由。",
                                    " 根本不必回头去看咒骂你的人是谁？如果有一条疯狗咬你一口，难道你也要趴下去反咬他一口吗？",
                                    " 世界的设计创造应以人为中心，而不是以谋取金钱，人并非以金钱为对象而生活，人的对象往往是人。",
                                    " 享受生活不需要寻找特殊的日子，因为每一天都是特殊的，享受生活就是享受今天。",
                                    " 应该相信，自己是生活的战胜者。",
                                    @"
从一粒沙看世界，
从一朵花看天堂，
把永恒纳进一个时辰，
把无限握在自己手心。",
                                    " 天若有情人亦老，人间正道是沧桑。"
                                };
                    Random r = new();
                    int random = r.Next(poems.Count);
                    MessageChain messageChain = new MessageChainBuilder()
                        .At(receiver.Sender.Id)
                        .Plain(poems[random])
                        .Build();
                    try
                    {
                        await receiver.QuoteMessageAsync(messageChain);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"出现错误！错误信息：{ex}");
                    }
                }
            }
        }
    }
}
