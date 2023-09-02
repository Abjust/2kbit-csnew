// 2kbit C# Edition: New，2kbit的 C# 分支版本的优化方案

// Copyright(C) 2023 Abjust 版权所有。

// 本程序是自由软件：你可以根据自由软件基金会发布的GNU Affero通用公共许可证的条款，即许可证的第3版或（您选择的）任何后来的版本重新发布它和/或修改它。。

// 本程序的发布是希望它能起到作用。但没有任何保证；甚至没有隐含的保证。本程序的分发是希望它是有用的，但没有任何保证，甚至没有隐含的适销对路或适合某一特定目的的保证。 参见 GNU Affero通用公共许可证了解更多细节。

// 您应该已经收到了一份GNU Affero通用公共许可证的副本。 如果没有，请参见<https://www.gnu.org/licenses/>。

/**
 * 2kbit C# Edition: New
 * 面包厂批次过期伪自动任务
**/

using Net_Codeintp_cs.Modules.Utils;
using Newtonsoft.Json.Linq;

namespace Net_Codeintp_cs.Modules.Group.Tasks
{
    internal class FactoryExpiration
    {
        public static void Execute(string group)
        {
            // 获取当前时间戳
            long TimeNow = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();
            // 读取面包厂数据
            JObject obj = Json.ReadFile("breadfactory");
            JObject g = (JObject)obj["groups"]!.Where(x => x.SelectToken("groupid")!.Value<string>()! == group).FirstOrDefault()!;
            int expiration = (int)g["expiration"]!;
            // 如果过期时间不为0且当前时间大于过期时间
            if (expiration != 0 && TimeNow >= (long)g["next_expiry"]!)
            {
                // 重置面包厂数据
                Json.ModifyObjectFromArray("breadfactory", "groups", "groupid", group, "breads", 0);
                Json.ModifyObjectFromArray("breadfactory", "groups", "groupid", group, "last_produce", TimeNow);
                Json.ModifyObjectFromArray("breadfactory", "groups", "groupid", group, "next_expiry", TimeNow + expiration * 86400);
                Json.ModifyObjectFromArray("materials", "groups", "groupid", group, "flour", 0);
                Json.ModifyObjectFromArray("materials", "groups", "groupid", group, "egg", 0);
                Json.ModifyObjectFromArray("materials", "groups", "groupid", group, "yeast", 0);
                Json.ModifyObjectFromArray("materials", "groups", "groupid", group, "last_produce", TimeNow);
                Json.ModifyObjectFromArray("materials", "groups", "groupid", group, "next_expiry", TimeNow + expiration * 86400);
            }
        }
    }
}
