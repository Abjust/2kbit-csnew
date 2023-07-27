using Mirai.Net.Data.Messages;
using Mirai.Net.Data.Messages.Receivers;
using Mirai.Net.Modules;
using Mirai.Net.Utils.Scaffolds;

namespace Net_Codeintp_cs.Modules.Group
{
    internal class Call : IModule
    {
        const int call_cd = 40;
        static long? last_call;
        public bool? IsEnable { get; set; }

        public async void Execute(MessageReceiverBase @base)
        {
            var receiver = @base.Concretize<GroupMessageReceiver>();
            string[] s = receiver.MessageChain.MiraiCode.TrimEnd().Split(" ");
            string person;
            if (s[0] == "!call" && (last_call == null || DateTimeOffset.UtcNow.ToUnixTimeSeconds() - last_call >= call_cd))
            {
                if (s.Length >= 2)
                {
                    if (s[1].Contains("[mirai:at:"))
                    {
                        person = s[1].Replace("[mirai:at:", "").Replace("]", "");
                    }
                    else
                    {
                        person = s[1];
                    }
                    MessageChain messages = new MessageChainBuilder()
                        .At(person)
                        .Plain(" 2kbit正在呼叫你")
                        .Build();
                    switch (s.Length)
                    {
                        case 3:
                            if (int.TryParse(s[2], out int number))
                            {
                                if (number >= 10)
                                {
                                    number = 10;
                                }
                                else if (number < 1)
                                {
                                    try
                                    {
                                        await receiver.SendMessageAsync("参数错误");
                                    }
                                    catch (Exception ex)
                                    {
                                        Console.WriteLine($"出现错误！错误信息：{ex}");
                                    }
                                    break;
                                }
                                for (int i = 0; i < number; i++)
                                {
                                    try
                                    {
                                        await receiver.SendMessageAsync(messages);
                                    }
                                    catch (Exception ex)
                                    {
                                        Console.WriteLine($"出现错误！错误信息：{ex}");
                                        break;
                                    }
                                    Thread.Sleep(333);
                                }
                                last_call = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                            }
                            else
                            {
                                try
                                {
                                    await receiver.SendMessageAsync("参数错误");
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine($"出现错误！错误信息：{ex}");
                                }
                            }
                            break;
                        case 2:
                            for (int i = 0; i < 5; i++)
                            {
                                try
                                {
                                    await receiver.SendMessageAsync(messages);
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine($"出现错误！错误信息：{ex}");
                                    break;
                                }
                                Thread.Sleep(333);
                            }
                            last_call = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                            break;
                        default:
                            try
                            {
                                await receiver.SendMessageAsync("参数错误");
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"出现错误！错误信息：{ex}");
                            }
                            break;
                    }
                }
                else if (s[0] == "!call")
                {
                    try
                    {
                        await receiver.SendMessageAsync("参数错误");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"出现错误！错误信息：{ex}");
                    }
                }
            }
            else if (s[0] == "!call")
            {
                try
                {
                    await receiver.SendMessageAsync($"CD未到，请别急！CD还剩： {call_cd - (DateTimeOffset.UtcNow.ToUnixTimeSeconds() - last_call)} 秒");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"出现错误！错误信息：{ex}");
                }
            }
        }
    }
}
