using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace IndDictionary
{
	public class SameNameFileCopy
	{
		FileInfo origin;
		public SameNameFileCopy(FileInfo _origin)
		{
			origin = _origin;
		}
		public void Copy(FileInfo destination)
		{
			StringBuilder s = new StringBuilder();
			byte adder=0;
			if (!destination.Exists) origin.CopyTo(destination.FullName);
			else
				do
				{
					s.Append($"{destination.FullName}({++adder})");
					
				} while (!File.Exists(s.ToString()));
			origin.CopyTo(s.ToString());
		}
	}
}
