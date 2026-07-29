using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DictMobile.addition
{
	static class IsItPhrase
	{
		static public bool isItPhrase(string str)
		{
			string s1= str.Replace(", ","");
			return s1.Count(c => c == ' ') > 3? true: false;
		}
	}
}
