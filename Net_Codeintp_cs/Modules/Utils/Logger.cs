// 2kbit C# Edition: New，2kbit的 C# 分支版本的优化方案

// Copyright(C) 2023 Abjust 版权所有。

// 本程序是自由软件：你可以根据自由软件基金会发布的GNU Affero通用公共许可证的条款，即许可证的第3版或（您选择的）任何后来的版本重新发布它和/或修改它。。

// 本程序的发布是希望它能起到作用。但没有任何保证；甚至没有隐含的保证。本程序的分发是希望它是有用的，但没有任何保证，甚至没有隐含的适销对路或适合某一特定目的的保证。 参见 GNU Affero通用公共许可证了解更多细节。

// 您应该已经收到了一份GNU Affero通用公共许可证的副本。 如果没有，请参见<https://www.gnu.org/licenses/>。

/**
 * 2kbit C# Edition: New
 * 日志工具模块
**/

using NLog;
using NLog.Conditions;
using NLog.Targets;

namespace Net_Codeintp_cs.Modules.Utils
{
    internal class Logger
    {
        // NLog 日志对象
        private static readonly NLog.Logger logger = LogManager.GetCurrentClassLogger();

        static Logger()
        {
            // 默认日志等级
            LogLevel defaultLevel = LogLevel.Info;
            // 如果是debug模式就输出debug信息
            if (BotMain.DebugEnabled)
            {
                defaultLevel = LogLevel.Debug;
            }
            // 控制台输出
            ColoredConsoleTarget ConsoleTarget = new()
            {
                Layout = "${longdate}|${level}|${message:withexception=true}"
            };
            // 高亮规则
            // 如果是debug就用灰色，info就用绿色，warning就用黄色，error就用红色
            ConsoleRowHighlightingRule HighlightingDebug = new()
            {
                Condition = ConditionParser.ParseExpression("level == LogLevel.Debug"),
                ForegroundColor = ConsoleOutputColor.DarkGray,
            };
            ConsoleRowHighlightingRule HighlightingInfo = new()
            {
                Condition = ConditionParser.ParseExpression("level == LogLevel.Info"),
                ForegroundColor = ConsoleOutputColor.Green
            };
            ConsoleRowHighlightingRule HighlightingWarning = new()
            {
                Condition = ConditionParser.ParseExpression("level == LogLevel.Warning"),
                ForegroundColor = ConsoleOutputColor.Yellow
            };
            ConsoleRowHighlightingRule HighlightingError = new()
            {
                Condition = ConditionParser.ParseExpression("level == LogLevel.Error"),
                ForegroundColor = ConsoleOutputColor.Red
            };
            // 添加高亮规则
            ConsoleTarget.RowHighlightingRules.Add(HighlightingDebug);
            ConsoleTarget.RowHighlightingRules.Add(HighlightingInfo);
            ConsoleTarget.RowHighlightingRules.Add(HighlightingWarning);
            ConsoleTarget.RowHighlightingRules.Add(HighlightingError);
            // 加载配置
            LogManager.Setup().LoadConfiguration(builder =>
            {
                builder.ForLogger().FilterMinLevel(defaultLevel).WriteTo(ConsoleTarget);
                builder.ForLogger().FilterMinLevel(defaultLevel).WriteToFile(fileName: "2kbit-log.txt");
            });
        }
        // 输出错误信息
        public static void Error(object error)
        {
            logger.Error(error);
        }
        // 输出警告信息
        public static void Warning(object warning)
        {
            logger.Warn(warning);
        }
        // 输出信息
        public static void Info(object info)
        {
            logger.Info(info);
        }
        // 输出调试信息
        public static void Debug(object debug)
        {
            logger.Debug(debug);
        }
    }
}
