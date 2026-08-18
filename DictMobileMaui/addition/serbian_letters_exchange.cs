using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DictMobile.addition
{
    static class serbian_letters_exchange
    {
        public static string MakeSerbian(string text)
        {
            Dictionary<string, string> replacement = new()
        {
            {"z^", "ž"},
            {"Z^", "Ž"},
            {"c`", "ć"},
            {"C`", "Ć"},
            {"c^", "č"},
            {"C^", "Č"},
            {"s^", "š"},
            {"S^", "Š"},
            {"d~", "đ"},
            {"D~", "Đ"}
        };
            string pattern = string.Join("|", replacement.Keys.Select(Regex.Escape));
            return Regex.Replace(text, pattern, m => replacement[m.Value]);

        }
    }
}
