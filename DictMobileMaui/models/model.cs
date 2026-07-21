using DictMobile.Auxilary;
using SQLite;

namespace DictMobile.models
{
	[Table ("Dict")]
	public class dict
    {
        private string dateRec;
        [PrimaryKey, AutoIncrement, Column("Number")]
		public int id { get; set; }
		[Indexed]
		public string Word { get; set; }
		[Indexed]
		public string Translation { get; set; }
		public int? Topic { get; set; }
		[Indexed]
		public string DateRec { get=>dateRec; set { dateRec = value.Remove(10); }  }
		[Column ("Score")]
		public byte Grade { get; set; }
		public bool Usersel { get; set; }
		public bool? Phrase { get; set; }
		public byte Relevation { get; set; }
		public bool IsDeleted { get; set; }
		public string Modification_Time { get; set; }
		public uint? DBID { get; set; }
		bool Spot { get; set; }
    }
	[Table ("topic")]
	public class topic
	{
		[PrimaryKey, AutoIncrement]
		public int id { get; set; }
		public string Name { get; set; }
		public bool IsDeleted { get; set; }
		public string Modification_Time { get; set; }
		public uint? DBID { get; set; }
	}
}
