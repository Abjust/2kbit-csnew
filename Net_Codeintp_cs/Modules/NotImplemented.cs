using Mirai.Net.Sessions.Http.Managers;

namespace Net_Codeintp_cs.Modules
{
    internal class NotImplemented
    {
        public static async void Do(string group, string command)
        {
            Console.WriteLine($"指令“{command}”尚未实现！");
            try
            {
                await MessageManager.SendGroupMessageAsync(group, $"指令“{command}”的功能已在计划内，但是尚未实现！");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"出现错误！错误信息：{ex}");
            }
        }
    }
}
