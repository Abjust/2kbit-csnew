// 2kbit C# Edition: New，2kbit的 C# 分支版本的优化方案

// Copyright(C) 2023 Abjust 版权所有。

// 本程序是自由软件：你可以根据自由软件基金会发布的GNU Affero通用公共许可证的条款，即许可证的第3版或（您选择的）任何后来的版本重新发布它和/或修改它。。

// 本程序的发布是希望它能起到作用。但没有任何保证；甚至没有隐含的保证。本程序的分发是希望它是有用的，但没有任何保证，甚至没有隐含的适销对路或适合某一特定目的的保证。 参见 GNU Affero通用公共许可证了解更多细节。

// 您应该已经收到了一份GNU Affero通用公共许可证的副本。 如果没有，请参见<https://www.gnu.org/licenses/>。

/**
 * 2kbit C# Edition: New
 * 余弦相似度工具模块
**/

using JiebaNet.Segmenter;

namespace Net_Codeintp_cs.Modules.Utils
{
    internal class Similarity
    {
        public static (IEnumerable<string>, IEnumerable<string>) Divide(string a, string b)
        {
            var segmenter = new JiebaSegmenter();
            return (segmenter.Cut(a), segmenter.Cut(b));
        }
        public static List<string> GetAllWords(IEnumerable<string> lst_aa, IEnumerable<string> lst_bb)
        {
            return lst_aa.Union(lst_bb).ToList();
        }
        public static (List<int>, List<int>) GetWordVector(IEnumerable<string> lst_aaa, IEnumerable<string> lst_bbb, List<string> all_word)
        {
            List<int> la = new();
            List<int> lb = new();
            foreach (var word in all_word)
            {
                la.Add(lst_aaa.Where(x => x != null && x == word).Count());
                lb.Add(lst_bbb.Where(x => x != null && x == word).Count());
            }
            return (la, lb);
        }
        public static double CalculateCosine(List<int> la, List<int> lb)
        {
            double coss = la.Zip(lb, (d1, d2) => d1 * d2).Sum() / Math.Sqrt(la.Zip(la, (d1, d2) => d1 * d2).Sum()) * Math.Sqrt(lb.Zip(lb, (d1, d2) => d1 * d2).Sum());
            return coss;
        }
    }
}
