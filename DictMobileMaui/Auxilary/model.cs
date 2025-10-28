using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace IndDictionary
{
	[Table ("Dict")]
	public class dict
    {
		[PrimaryKey, AutoIncrement, Column("number")]
		public int Number { get; set; }
		[Indexed]
		public string Word { get; set; }
		[Indexed]
		public string Translation { get; set; }
		public int Topic { get; set; }
		[Indexed]
		public string DateRec { get; set; }
		[Column ("Score")]
		public short Grade { get; set; }
		public bool Usersel { get; set; }
		public bool Phrase { get; set; }
		public short Relevation { get; set; }
		bool Spot { get; set; }
    }
	[Table ("topic")]
	public class topic
	{
		[PrimaryKey, AutoIncrement]
		public int id { get; set; }
		public string Name { get; set; }
	}
}
