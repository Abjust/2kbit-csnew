using JiebaNet.Segmenter;

namespace Net_Codeintp_cs.Modules
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
            double coss = (la.Zip(lb, (d1, d2) => d1 * d2).Sum()) / (Math.Sqrt(la.Zip(la, (d1, d2) => d1 * d2).Sum())) * (Math.Sqrt(lb.Zip(lb, (d1, d2) => d1 * d2).Sum()));
            return coss;
        }
    }
}
