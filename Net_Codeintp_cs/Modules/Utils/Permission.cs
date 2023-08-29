// 2kbit C# Edition: New，2kbit的 C# 分支版本的优化方案

// Copyright(C) 2023 Abjust 版权所有。

// 本程序是自由软件：你可以根据自由软件基金会发布的GNU Affero通用公共许可证的条款，即许可证的第3版或（您选择的）任何后来的版本重新发布它和/或修改它。。

// 本程序的发布是希望它能起到作用。但没有任何保证；甚至没有隐含的保证。本程序的分发是希望它是有用的，但没有任何保证，甚至没有隐含的适销对路或适合某一特定目的的保证。 参见 GNU Affero通用公共许可证了解更多细节。

// 您应该已经收到了一份GNU Affero通用公共许可证的副本。 如果没有，请参见<https://www.gnu.org/licenses/>。

/**
* 2kbit C# Edition: New
* 权限管理模块
**/

namespace Net_Codeintp_cs.Modules.Utils
{
    internal class Permission
    {
        public static List<string>? Ops { get; set; }
        public static List<string>? OpsGlobal { get; set; }
        public static List<string>? Blocklist { get; set; }
        public static List<string>? BlocklistGlobal { get; set; }
        public static List<string>? Ignores { get; set; }
        public static List<string>? IgnoresGlobal { get; set; }
        public static List<string>? OptedOut { get; set; }
        public static bool IsGlobalAdmin(string qq)
        {
            return OpsGlobal!.Contains(qq);
        }
        public static bool IsGroupAdmin(string groupid, string qq)
        {
            return Ops!.Contains($"{groupid}_{qq}") || IsGlobalAdmin(qq);
        }
        public static bool IsBlockedGlobal(string qq)
        {
            return BlocklistGlobal!.Contains(qq);
        }
        public static bool IsBlocked(string groupid, string qq)
        {
            return Blocklist!.Contains($"{groupid}_{qq}") || IsBlockedGlobal(qq);
        }
        public static bool IsIgnoredGlobal(string qq)
        {
            return IgnoresGlobal!.Contains(qq);
        }
        public static bool IsIgnored(string groupid, string qq)
        {
            return Ignores!.Contains($"{groupid}_{qq}") || IsIgnoredGlobal(qq);
        }
        public static bool IsOptedOut(string groupid)
        {
            return OptedOut!.Contains(groupid);
        }
    }
}
