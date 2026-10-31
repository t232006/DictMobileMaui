//using Android.Content;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace DictMobile.addition
{
    public static class StringExtension
    {
        public static List<string> BreakTheWord(this string longWord)
        {
            string[] s = longWord.Split(',', ';');
            for (int i = 0; i < s.Length; i++) s[i] = s[i].Trim();
            return s.ToList();
        }
        public static bool ManyWordsComparier(this string s1, string s2)
        {
            List<string> l1 = s1.BreakTheWord();
            List<string> l2 = s2.BreakTheWord();
            return l1.OrderBy(x => x).SequenceEqual(l2.OrderBy(x => x));
        }
    }
}
